using Common.Constants;
using Common.Network.Packets;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldServer.Game.Objects;
using WorldServer.Storage;
using System;
using WorldServer.Game.Structs;
using Common.Network;

namespace WorldServer.Game.Managers
{
    public class InventoryManager
    {
        private readonly Player player;
        private Dictionary<uint, Container> Containers;

        public HashSet<uint> QueriedItems;
        public Container Backpack { get { return this.Containers[(uint)InventorySlots.SLOT_INBACKPACK]; } }

        public InventoryManager(Player player)
        {
            this.player = player;
            this.QueriedItems = new HashSet<uint>();
            this.Containers = new Dictionary<uint, Container>();

            Container backpack = new Container(0, true) { TotalSlots = (uint)InventorySlots.SLOT_BANK_END, Owner = player.Guid };

            Containers.Add((uint)InventorySlots.SLOT_INBACKPACK, backpack);
        }

        public void BuildPacket(ref UpdateClass uc)
        {
            foreach (var itm in Backpack.Items.Values)
                uc.UpdateValue<ulong>(PlayerFields.PLAYER_FIELD_INV_SLOT_1, itm.Guid, (int)(itm.CurrentSlot * 2));
        }

        public void SendInventoryUpdate(bool self = false)
        {
            foreach (Container cnt in this.Containers.Values)
            {
                if (!cnt.IsBackpack)
                {
                    PacketWriter pkt = cnt.CreateItem();
                    if (self)
                    {
                        player.Client.Send(pkt, true);
                        player.QueryItemCheck(cnt.Entry);
                    }
                    else
                    {
                        GridManager.Instance.SendSurrounding(pkt, player);
                        Parallel.ForEach(GridManager.Instance.GetSurroundingObjects(player, true).Cast<Player>(), p =>
                        {
                            p.QueryItemCheck(cnt.Entry);
                        });
                    }
                }

                foreach (Item itm in cnt.Items.Values)
                {
                    PacketWriter pkt = itm.CreateItem();
                    if (self)
                    {
                        player.Client.Send(pkt, true);
                        player.QueryItemCheck(itm.Entry);
                    }
                    else
                    {
                        GridManager.Instance.SendSurrounding(pkt, player);
                        Parallel.ForEach(GridManager.Instance.GetSurroundingObjects(player, true).Cast<Player>(), p =>
                        {
                            p.QueryItemCheck(itm.Entry);
                        });
                    }
                }
            }
        }


        public bool AddBag(uint slot, Container item)
        {
            if (!IsBagPos((uint)InventorySlots.SLOT_INBACKPACK, slot))
                return false;

            item.Player = player.Guid;
            Containers.Add(slot, item);
            return true;
        }

        public bool RemoveBag(uint slot)
        {
            if (!IsBagPos((uint)InventorySlots.SLOT_INBACKPACK, slot) || !Containers.ContainsKey(slot))
                return false;

            Containers.Remove(slot);
            return true;
        }

        public Container GetBag(uint slot)
        {
            if (Containers.ContainsKey(slot))
                return Containers[slot];

            return null;
        }

        public Container GetBag(ulong guid)
        {
            return this.Containers.Values.FirstOrDefault(x => x.Guid == guid);
        }


        public bool AddItem(Item item)
        {
            item.Player = player.Guid;
            foreach (var cont in this.Containers.Values)
            {
                if (!cont.IsFull && cont.AddItem(item))
                {
                    this.UpdateSurrounding(item);
                    player.QueryItemCheck(item.Entry);
                    return true;
                }
            }

            return false;
        }

        public bool AddItem(Item item, uint slot, InventorySlots bag)
        {
            if (!this.Containers.ContainsKey((uint)bag))
                return false;

            item.Player = player.Guid;
            item.Bag = (uint)bag;
            if (GetBag((uint)bag)?.AddItem(item, slot) == true)
            {
                this.UpdateSurrounding(item);
                player.QueryItemCheck(item.Entry);
                return true;
            }

            return false;
        }

        public bool RemoveItem(Item item)
        {
            foreach (Container cont in this.Containers.Values)
                if (cont.RemoveItem(item))
                    return true;

            return false;
        }

        public bool RemoveItem(uint entry)
        {
            foreach (Container cont in this.Containers.Values)
                if (cont.RemoveItem(entry))
                    return true;

            return false;
        }

        public bool CanStoreItem(uint entry, uint count, bool bank = false)
        {
            int amount = (int)count;
            ItemTemplate template = Database.ItemTemplates.TryGet(entry);
            if (template == null)
                return false;

            //Have we reached our unique limit?
            if (template.MaxCount > 0 && GetEntryCount(entry) >= template.MaxCount)
                return false;

            //Check backpack first based on Bank or not
            if (!bank)
            {
                for (uint i = (uint)InventorySlots.SLOT_ITEM_START; i < (uint)InventorySlots.SLOT_ITEM_END; i++)
                    if (!Backpack.Slots.ContainsKey(i))
                        amount -= (int)template.MaxStackCount; //Free slot
            }
            else
            {
                for (uint i = (uint)InventorySlots.SLOT_BANK_ITEM_START; i < (uint)InventorySlots.SLOT_BANK_ITEM_END; i++)
                    if (!Backpack.Slots.ContainsKey(i))
                        amount -= (int)template.MaxStackCount; //Free slot
            }
            if (amount <= 0) return true;

            //Check other containers
            foreach (Container bag in Containers.Values)
            {
                if (bag.IsBackpack)
                    continue;

                if ((bank && bag.CurrentSlot < (uint)InventorySlots.SLOT_BANK_BAG_1) ||
                    (!bank && bag.CurrentSlot >= (uint)InventorySlots.SLOT_BANK_BAG_1))
                    continue;

                amount -= (int)((bag.TotalSlots - bag.Slots.Count) * template.MaxStackCount); //Free slots * Stackable
                if (amount <= 0) return true;
            }

            return false;
        }


        public uint GetEntryCount(uint entry)
        {
            uint count = 0;
            foreach (Container cont in this.Containers.Values)
                foreach (Item item in cont.Items.Values)
                    if (item.Entry == entry)
                        count += item.StackCount;

            return count;
        }

        public bool HasItem(Item item)
        {
            foreach (Container cont in this.Containers.Values)
                if (cont.Items.ContainsKey(item.Guid))
                    return true;

            return false;
        }

        public Item GetItem(ulong guid)
        {
            foreach (Container cont in this.Containers.Values)
                if (cont.Items.ContainsKey(guid))
                    return cont.Items[guid];

            return null;
        }

        public bool HasOffhandWeapon()
        {
            Item weapon = Backpack.GetItem((byte)InventoryTypes.WEAPONOFFHAND);
            return weapon != null;
        }

        public void SetBaseAttackTime()
        {
            player.BaseAttackTime = 1;
            Item weapon = Backpack.GetItem((byte)InventorySlots.SLOT_MAINHAND);
            if (weapon != null)
                player.BaseAttackTime = weapon.Template.WeaponSpeed;
        }

        public void Save()
        {
            foreach (Container container in this.Containers.Values)
            {
                if (!container.IsBackpack)
                    container.Save();

                foreach (Item item in container.Items.Values)
                    item.Save();
            }
        }


        public static bool IsBagPos(uint bagslot, uint slot)
        {
            if (bagslot == (uint)InventorySlots.SLOT_INBACKPACK && (slot >= (uint)InventorySlots.SLOT_BAG1 && slot < (uint)InventorySlots.SLOT_INBACKPACK))
                return true;
            if (bagslot == (uint)InventorySlots.SLOT_INBACKPACK && (slot >= (uint)InventorySlots.SLOT_BANK_BAG_1 && slot < (uint)InventorySlots.SLOT_BANK_END))
                return true;

            return false;
        }

        public static bool IsBankSlot(uint bagslot, uint slot)
        {
            if (bagslot == 23 && slot >= (uint)BankSlots.BANK_SLOT_ITEM_START && slot < (uint)BankSlots.BANK_SLOT_ITEM_END)
                return true;
            if (bagslot == 23 && slot >= (uint)BankSlots.BANK_SLOT_BAG_START && slot < (uint)BankSlots.BANK_SLOT_BAG_END)
                return true;
            if (bagslot >= (uint)BankSlots.BANK_SLOT_BAG_START && bagslot < (uint)BankSlots.BANK_SLOT_BAG_END)
                return true;

            return false;
        }

        public static bool IsEquipmentPos(uint bagslot, uint slot)
        {
            return bagslot == (uint)InventorySlots.SLOT_INBACKPACK && slot < (uint)InventorySlots.SLOT_BAG1;
        }

        public static bool IsInventoryPos(uint bagslot, uint slot)
        {
            return bagslot == (uint)InventorySlots.SLOT_INBACKPACK && slot >= (uint)InventorySlots.SLOT_ITEM_START && slot < (uint)InventorySlots.SLOT_ITEM_END;
        }

        private void UpdateSurrounding(Item item)
        {
            GridManager.Instance.SendSurrounding(item.CreateItem(), player);
            Parallel.ForEach(GridManager.Instance.GetSurroundingObjects(player, true).Cast<Player>(), p =>
            {
                p.QueryItemCheck(item.Entry);
            });
        }

        private uint SlotIndex(uint slot)
        {
            if (IsBagPos((uint)InventorySlots.SLOT_INBACKPACK, slot))
                return (uint)Math.Abs(23 - slot);

            return 0;
        }
    }
}
