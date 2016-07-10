using Common.Constants;
using Common.Network.Packets;
using System.Linq;
using WorldServer.Storage;

namespace WorldServer.Game.Objects.PlayerExtensions.Trade
{
    public static class TradeExtension
    {
        public static void SendTradeStatus(this Player p, TradeStatuses status)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_TRADE_STATUS);
            switch (status)
            {
                case TradeStatuses.TRADE_STATUS_PROPOSED:
                    pkt.WriteUInt32((uint)status);
                    pkt.WriteUInt64(0);
                    break;
                case TradeStatuses.TRADE_STATUS_INITIATED:
                    pkt.WriteUInt32((uint)status);
                    break;
                case TradeStatuses.TRADE_STATUS_FAILED:
                    pkt.WriteUInt32((uint)status);
                    pkt.WriteUInt32(0);
                    pkt.WriteUInt8(0);
                    pkt.WriteUInt32(0);
                    break;
                default:
                    pkt.WriteUInt32((uint)status);
                    break;
            }

            p.Client.Send(pkt);
        }

        public static void SendUpdateTrade(this Player p, bool trader_state)
        {
            if (p == null)
                return;

            TradeData data = trader_state ? p.TradeInfo.TraderData : p.TradeInfo;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_TRADE_STATUS_EXTENDED);
            pkt.WriteUInt8((byte)(trader_state ? 1 : 0));
            pkt.WriteUInt32(TradeData.TRADE_SLOT_COUNT);
            pkt.WriteUInt32(data.Money);
            pkt.WriteUInt32(0); //proposedEnchantmentSlot ??
            pkt.WriteUInt32(0); //proposedEnchantmentSpellID ??

            for (byte i = 0; i < TradeData.TRADE_SLOT_COUNT; i++)
            {
                pkt.WriteUInt8(i);
                Item item = data.GetItem(i);

                if (item != null)
                {
                    pkt.WriteUInt32(item.Entry);
                    pkt.WriteUInt32(item.Template.DisplayID);
                    pkt.WriteUInt32(item.StackCount);
                    pkt.WriteUInt32(0); //data << uint32(item->GetEnchantmentId(PERM_ENCHANTMENT_SLOT));
                    pkt.WriteUInt64(0); //data << item->GetGuidValue(ITEM_FIELD_CREATOR);
                }
                else
                    for (byte j = 0; j < 6; ++j)
                        pkt.WriteUInt32(0);
            }

            p.Client.Send(pkt);
        }

        public static void SendTradeRequest(this Player p, ulong guid)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_TRADE_STATUS);
            pkt.WriteUInt32((uint)TradeStatuses.TRADE_STATUS_PROPOSED);
            pkt.WriteUInt64(guid);
            p.Client.Send(pkt);
        }
    }

    public class TradeData
    {
        //TODO double check if enchanting is done by trade window / even exists?
        public const int TRADE_SLOT_COUNT = 7;
        public Player Trader { get; set; }
        public Player Player { get; set; }
        public TradeData TraderData { get { return Trader.TradeInfo; } }
        public bool Accepted { get; private set; } = false;
        public uint Money { get; private set; } = 0;

        private ulong[] items = new ulong[TRADE_SLOT_COUNT];

        public TradeData(Player p, Player t)
        {
            Player = p;
            Trader = t;
            Player.IsTrading = true;
        }

        public Item GetItem(uint slot)
        {
           return Database.Items.TryGet(items[slot]);
        }

        public void SetItem(uint slot, Item item)
        {
            if (items[slot] == item.Guid)
                return;

            Player.QueryItemCheck(item.Entry);
            Trader.QueryItemCheck(item.Entry);
            items[slot] = item.Guid;

            SetAccepted(false);
            TraderData.SetAccepted(false);

            Trader.SendUpdateTrade(true);
            Player.SendUpdateTrade(false);
        }

        public void ClearItem(uint slot)
        {
            items[slot] = 0;

            Trader.SendUpdateTrade(true);
            Player.SendUpdateTrade(false);
        }

        public void SetMoney(uint money)
        {
            this.Money = money;

            SetAccepted(false);
            TraderData.SetAccepted(false);

            Trader.SendUpdateTrade(true);
            Player.SendUpdateTrade(false);
        }

        public void SetAccepted(bool state)
        {
            this.Accepted = state;

            if (!state)
            {
                Player.SendTradeStatus(TradeStatuses.TRADE_STATUS_STATE_CHANGED);
                Trader.SendTradeStatus(TradeStatuses.TRADE_STATUS_STATE_CHANGED);
            }

        }

        public void Clear()
        {
            this.Player.IsTrading = false;
            this.Player = null;
            this.Trader = null;
        }
    }
}
