using Common.Constants;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game.Managers;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.PlayerExtensions;
using WorldServer.Game.Objects.PlayerExtensions.Trade;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Network;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class TradeHandler
    {
        public static void HandleInitiateTradeOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            Player self = manager.Character;
            Player other = Database.Players.TryGet(guid);

            //if (self.IsTrading)
            //    self.TradeInfo.Clear();

            if (other == null || other?.IsDead == true || other?.LoggedIn == false)
            {
                self.SendTradeStatus(TradeStatuses.TRADE_STATUS_PLAYER_NOT_FOUND);
                return;
            }

            if (self.IsDead)
            {
                self.SendTradeStatus(TradeStatuses.TRADE_STATUS_DEAD);
                return;
            }

            if (other.IsTrading)
            {
                self.SendTradeStatus(TradeStatuses.TRADE_STATUS_ALREADY_TRADING);
                return;
            }

            if (self.IsEnemyTo(other))
            {
                self.SendTradeStatus(TradeStatuses.TRADE_STATUS_WRONG_FACTION);
                return;
            }

            self.TradeInfo = new TradeData(self, other);
            other.TradeInfo = new TradeData(other, self);

            self.SendTradeRequest(other.Guid);
            other.SendTradeRequest(self.Guid);
        }

        public static void HandleBeginTradeOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            if (!manager.Character.IsTrading)
                return;

            manager.Character.SendTradeStatus(TradeStatuses.TRADE_STATUS_INITIATED);
            manager.Character.TradeInfo.Trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_INITIATED);
        }

        public static void HandleCancelTradeOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            if (!manager.Character.IsTrading)
                return;

            manager.Character.TradeInfo.Trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
            manager.Character.TradeInfo.Trader.TradeInfo.Clear();

            manager.Character.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
            manager.Character.TradeInfo.Clear();

        }

        public static void HandleSetTradeItemOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            byte tradeSlot = packet.ReadUInt8();
            byte bag = packet.ReadUInt8();
            byte slot = packet.ReadUInt8();

            if (!manager.Character.IsTrading)
                return;

            Item item = manager.Character.GetItem(bag, slot);
            if (item == null)
                return;

            if (tradeSlot >= TradeData.TRADE_SLOT_COUNT)
            {
                manager.Character.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                return;
            }

            manager.Character.TradeInfo.SetItem(tradeSlot, item);
        }

        public static void HandleSetTradeGoldOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            uint gold = packet.ReadUInt32();

            if (!manager.Character.IsTrading)
                return;

            manager.Character.TradeInfo.SetMoney(gold);
        }

        public static void HandleClearTradeItemOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            byte tradeslot = packet.ReadUInt8();

            if (tradeslot > TradeData.TRADE_SLOT_COUNT)
                return;

            manager.Character.TradeInfo.ClearItem(tradeslot);
        }

        public static void HandleAcceptTradeOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            Player player = manager.Character;
            Player trader = manager.Character.TradeInfo.Trader;
            TradeData thistrade = manager.Character.TradeInfo;
            TradeData othertrade = manager.Character.TradeInfo.TraderData;
            Item[] myitems = new Item[TradeData.TRADE_SLOT_COUNT];
            Item[] otheritems = new Item[TradeData.TRADE_SLOT_COUNT];

            thistrade.SetAccepted(true);

            if (thistrade.Money > player.Money)
            {
                ChatManager.Instance.SendNotification(player, "You do not have enough gold");
                return;
            }

            if (othertrade.Money > trader.Money)
            {
                ChatManager.Instance.SendNotification(trader, "You do not have enough gold");
                return;
            }

            for (byte i = 0; i < TradeData.TRADE_SLOT_COUNT; i++)
            {
                if (thistrade.GetItem(i)?.IsSoulbound == true)
                {
                    player.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                    trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                    return;
                }

                if (othertrade.GetItem(i)?.IsSoulbound == true)
                {
                    player.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                    trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                    return;
                }
            }

            if (othertrade.Accepted)
            {
                //TODO spell cast checks

                //Inventory checks
                for (byte i = 0; i < TradeData.TRADE_SLOT_COUNT; i++)
                {
                    if (othertrade.GetItem(i) != null)
                    {
                        if (!trader.Inventory.CanStoreItem(othertrade.GetItem(i).Entry, othertrade.GetItem(i).StackCount))
                        {
                            player.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                            trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                            return;
                        }
                    }

                    if (thistrade.GetItem(i) != null)
                    {
                        if (!player.Inventory.CanStoreItem(thistrade.GetItem(i).Entry, thistrade.GetItem(i).StackCount))
                        {
                            player.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                            trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_CANCELLED);
                            return;
                        }
                    }
                }

                //Move items
                for (byte i = 0; i < TradeData.TRADE_SLOT_COUNT; i++)
                {
                    player.AddItem(othertrade.GetItem(i));
                    trader.AddItem(thistrade.GetItem(i));
                    trader.RemoveItem(othertrade.GetItem(i));
                    player.RemoveItem(thistrade.GetItem(i));
                }

                //Add money
                player.Money -= thistrade.Money;
                player.Money += othertrade.Money;
                trader.Money -= othertrade.Money;
                trader.Money += thistrade.Money;

                //Do spell


                trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_COMPLETE);
                player.SendTradeStatus(TradeStatuses.TRADE_STATUS_COMPLETE);

                player.TradeInfo.Clear();
                trader.TradeInfo.Clear();
                trader.Dirty = true;
                player.Dirty = true;
            }
            else
                trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_ACCEPTED);

        }

        public static void HandleUnacceptTradeOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            if (!manager.Character.IsTrading)
                return;

            manager.Character.TradeInfo.SetAccepted(false);
        }
    }
}
