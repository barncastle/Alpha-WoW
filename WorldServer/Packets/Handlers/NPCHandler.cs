using Common.Constants;
using Common.Database.DBC;
using Common.Database.DBC.Structures;
using Common.Helpers;
using Common.Network.Packets;
using System.Linq;
using WorldServer.Game;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Game.Structs;
using WorldServer.Network;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class NPCHandler
    {
        public static void HandleListInventoryOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            Creature npc = Database.Creatures.TryGet(guid);

            if (npc == null)
                return;

            if (npc.IsDead || manager.Character.Location.Distance(npc.Location) > Globals.INTERACT_DISTANCE || manager.Character.IsDead)
                return;

            if (npc.IsEnemyTo(manager.Character))
                return;
            
            manager.Send(npc.ListInventory(manager.Character));
        }

        public static void HandleSellItemOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong vendorGUID = packet.ReadUInt64();
            ulong itemGUID = packet.ReadUInt64();
            uint count = packet.ReadByte();
            Player c = manager.Character;

            if (count < 1)
                count = 1;

            if (!Database.Items.ContainsKey(itemGUID)) //Item doesnt exist
                return;

            if (!c.Inventory.HasItem(Database.Items.TryGet(itemGUID)))
                return;

            Creature npc = Database.Creatures.TryGet(vendorGUID);
            if (npc == null)
                return;
            if (npc.IsDead == true || c.Location.Distance(npc.Location) > Globals.INTERACT_DISTANCE || c.IsDead) //Basic other checks
                return;
            if (npc.IsEnemyTo(manager.Character))
                return;

            Item item = c.Inventory.GetItem(itemGUID);
            if (item.IsSoulbound)
            {
                c.SendSellError(SellResults.SELL_ERR_CANT_SELL_ITEM, npc, itemGUID); //Doesnt own the item
                return;
            }

            //Check non empty bag - todo

            if (count > item.StackCount) //Check we have enough to sell
            {
                c.SendSellError(SellResults.SELL_ERR_CANT_SELL_ITEM, npc, itemGUID);
                return;
            }

            if (item.Template.SellPrice <= 0) //No sell price
            {
                c.SendSellError(SellResults.SELL_ERR_CANT_SELL_ITEM, npc, itemGUID);
                return;
            }

            if (count < item.StackCount) //Selling less than we have
                item.StackCount = (item.StackCount - count); //Simply update the stack count
            else
                c.Inventory.RemoveItem(item); //Delete the item from the character, there is no buy back slot in alpha!

            c.Money += (item.Template.SellPrice * count);
            manager.Character.Dirty = true;
        }

        public static void HandleBuyItemOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong vendor = packet.ReadUInt64();
            uint item = packet.ReadUInt32();
            byte count = packet.ReadUInt8();
            uint cost = 0;
            if (count < 1)
                count = 1;

            Player c = manager.Character;
            Creature npc = Database.Creatures.TryGet(vendor);
            if (npc == null)
                return;
            if (npc.IsEnemyTo(manager.Character))
                return;

            if (!Database.ItemTemplates.ContainsKey(item)) //Item doesnt exist
            {
                c.SendBuyError(BuyResults.BUY_ERR_CANT_FIND_ITEM, npc, item);
                return;
            }

            ItemTemplate itm = Database.ItemTemplates.TryGet(item);
            VendorItem vitm = npc.VendorLoot.Where(vi => vi.Item == item).First();
            cost = itm.BuyPrice * count; //Enough info to calc cost

            if (c.IsDead || npc.IsDead || vitm.Entry == 0) //Dead or doesnt sell that item
            {
                c.SendBuyError(BuyResults.BUY_ERR_CANT_FIND_ITEM, npc, item);
                return;
            }

            if (vitm.MaxCount > 0 && vitm.CurCount < count) //Vendor hasn't enough of them
            {
                c.SendBuyError(BuyResults.BUY_ERR_ITEM_SOLD_OUT, npc, item);
                return;
            }

            if (itm.LevelReq > c.Level) //Not high enough level
            {
                c.SendBuyError(BuyResults.BUY_ERR_RANK_REQUIRE, npc, item);
                return;
            }

            if (c.Money < cost)
            {
                c.SendBuyError(BuyResults.BUY_ERR_NOT_ENOUGHT_MONEY, npc, item);
                return;
            }

            if (c.AddItem(item, count))
            {
                npc.UpdateInventoryItem(item, count);
                c.Money -= cost;
                manager.Send(npc.ListInventory(manager.Character));
                manager.Character.Dirty = true;
            }
            else
            {
                c.SendBuyError(BuyResults.BUY_ERR_CANT_CARRY_MORE, npc, item);
                return;
            }

        }

        public static void HandleBuyItemInSlotOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong vendor = packet.ReadUInt64();
            uint item = packet.ReadUInt32();
            ulong bag = packet.ReadUInt64();
            byte bagslot = packet.ReadUInt8();
            byte count = packet.ReadUInt8();

            if (count < 1)
                count = 1;

            Player c = manager.Character;
            Creature npc = Database.Creatures.TryGet(vendor);
            if (npc == null || npc?.IsEnemyTo(manager.Character) == true)
                return;

            ItemTemplate itm = Database.ItemTemplates.TryGet(item); 
            VendorItem vitm = npc.Template.VendorItems.Where(vi => vi.Item == item).First();
            uint cost = itm.BuyPrice * count; //Enough info to calc cost

            if (itm == null) //Item doesnt exist
            {
                c.SendBuyError(BuyResults.BUY_ERR_CANT_FIND_ITEM, npc, item);
                return;
            }

            Container container = c.Inventory.GetBag(bag);
            if (container == null || container?.IsFull == true)
            {
                c.SendBuyError(BuyResults.BUY_ERR_CANT_CARRY_MORE, npc, item);
                return;
            }
            
            if (c.IsDead || npc.IsDead || vitm.Entry == 0) //Dead or doesnt sell that item
            {
                c.SendBuyError(BuyResults.BUY_ERR_CANT_FIND_ITEM, npc, item);
                return;
            }

            if (vitm.MaxCount > 0 && vitm.CurCount < count) //Vendor hasn't enough of them
            {
                c.SendBuyError(BuyResults.BUY_ERR_ITEM_SOLD_OUT, npc, item);
                return;
            }

            if (itm.LevelReq > c.Level) //Not high enough level
            {
                c.SendBuyError(BuyResults.BUY_ERR_RANK_REQUIRE, npc, item);
                return;
            }

            if (c.Money < cost)
            {
                c.SendBuyError(BuyResults.BUY_ERR_NOT_ENOUGHT_MONEY, npc, item);
                return;
            }

            if (c.AddItemInSlot(item, bagslot))
            {
                c.SwapItem(0, 0, (byte)bag, bagslot);
                c.Money -= cost;
                manager.Character.Dirty = true;
            }
            else
            {
                c.SendBuyError(BuyResults.BUY_ERR_CANT_CARRY_MORE, npc, item);
                return;
            }
        }

        public static void HandleTrainerList(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();

            if (guid == manager.Character.Guid) //Player talents
            {
                Player cha = Database.Players.TryGet(guid);
                if (cha != null)
                    cha.SendTalentList();
            }
            else //NPC vendor
            {
                Creature npc = Database.Creatures.TryGet(guid);
                if (npc == null)
                    return;
                if (npc.IsEnemyTo(manager.Character))
                    return;
                if (!npc.IsTrainerOfType(manager.Character, true))
                    return;

                npc.SendSpellList(manager.Character);
            }

        }

        public static void HandleTrainerBuySpellOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            Player player = manager.Character;
            ulong guid = packet.ReadUInt64();
            uint spellID = packet.ReadUInt32();

            Spell spell = null;
            if (!DBC.Spell.TryGetValue(spellID, out spell)) //Only use those with spells
                return;

            if (guid == player.Guid) //Talent purchase
            {
                foreach (SkillLineAbility ability in DBC.SkillLineAbility.Values)
                {
                    //No spell!
                    if (ability.m_spell != spellID) 
                        continue;

                    //Race Class exclude
                    if (ability.m_excludeClass.HasFlag(player.ClassMask) || ability.m_excludeRace.HasFlag(player.RaceMask)) 
                        continue;

                    if (player.TalentPoints >= 10 && player.Level >= spell.baseLevel)
                    {
                        player.Talents.Add(ability.m_ID);
                        player.TalentPoints -= 10;

                        //TODO ADD SPELL TO SPELLBOOK

                        player.SendBuySpellSucceed(guid, spellID);
                        manager.Character.Dirty = true;
                        player.SendPlaySpellVisual(guid, 0xB3);

                        player.Spells.Add(spellID, new PlayerSpell(spell));
                    }

                    break;
                }

                player.SendTalentList();
            }
            else if (Database.Creatures.ContainsKey(guid)) //NPC Spell purchase
            {
                Creature creature = Database.Creatures.TryGet(guid);

                if (!creature.Template.VendorSpells.ContainsKey(spellID))
                    return;
                if (creature.IsEnemyTo(manager.Character))
                    return;

                VendorSpell vendorspell = creature.Template.VendorSpells[spellID];
                if (!player.CanPurchaseSpell(vendorspell))
                    return;

                player.Money -= vendorspell.Cost;

                player.SendBuySpellSucceed(guid, spellID);
                manager.Character.Dirty = true;
                player.SendPlaySpellVisual(guid, 0xB3);

                player.Spells.Add(spellID, new PlayerSpell(spell));

                creature.SendSpellList(player);
            }
        }

        public static void HandleBankerActivateOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();

            Creature npc = Database.Creatures.TryGet(guid);
            if (npc == null)
                return;
            if (npc.IsDead == true || manager.Character.Location.Distance(npc.Location) > Globals.INTERACT_DISTANCE || manager.Character.IsDead) //Basic other checks
                return;
            if (npc.IsEnemyTo(manager.Character) || !npc.NPCFlags.HasFlag((byte)NpcFlags.NPC_FLAG_BANKER))
                return;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_SHOW_BANK);
            pkt.WriteUInt64(guid);
            manager.Send(pkt);
        }

        public static void HandleBuyBankSlotOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();

            Creature npc = Database.Creatures.TryGet(guid);
            if (npc == null)
                return;
            if (npc.IsDead == true || manager.Character.Location.Distance(npc.Location) > Globals.INTERACT_DISTANCE || manager.Character.IsDead) //Basic other checks
                return;
            if (npc.IsEnemyTo(manager.Character) || !npc.NPCFlags.HasFlag((byte)NpcFlags.NPC_FLAG_BANKER))
                return;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_BUY_BANK_SLOT_RESULT);
            uint nextslot = (uint)manager.Character.BankSlots + 1;
            
            if(!DBC.BankBagSlotPrices.ContainsKey(nextslot))
            {
                pkt.WriteUInt32((uint)BankSlotErrors.BANKSLOT_ERROR_FAILED_TOO_MANY);
                manager.Send(pkt);
                return;
            }

            uint price = DBC.BankBagSlotPrices[nextslot].m_Cost;
            if(price > manager.Character.Money)
            {
                pkt.WriteUInt32((uint)BankSlotErrors.BANKSLOT_ERROR_INSUFFICIENT_FUNDS);
                manager.Send(pkt);
                return;
            }

            manager.Character.Money -= price;
            manager.Character.BankSlots++;
            pkt.WriteUInt32((uint)BankSlotErrors.BANKSLOT_ERROR_OK);
            manager.Send(pkt);

            manager.Character.Dirty = true;
        }
    }
}
