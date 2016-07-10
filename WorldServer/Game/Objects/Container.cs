using Common.Constants;
using System.Collections.Generic;
using WorldServer.Storage;
using System.Linq;
using Common.Helpers;

namespace WorldServer.Game.Objects
{
    public class Container : Item
    {
        public const uint MAX_BAG_SLOTS = 20;
        public uint TotalSlots;
        public Dictionary<ulong, Item> Items;
        public SortedDictionary<uint, ulong> Slots;

        public bool IsBackpack { get; private set; }
        public bool IsFull { get { return this.Slots.Count >= this.TotalSlots; } }
        public bool IsEmpty { get { return this.Slots.Count == 0; } }

        public Container(uint entry, bool nonitem = false)
        {
            this.ObjectType |= ObjectTypes.TYPE_CONTAINER;
            this.Items = new Dictionary<ulong, Item>();
            this.Slots = new SortedDictionary<uint, ulong>();

            if (!nonitem)
            {
                this.Entry = entry;
                this.Template = Database.ItemTemplates.TryGet(entry);
                this.TotalSlots = this.Template.ContainerSlots;
                this.EquipSlot = PrefInvSlot();
            }

            this.IsBackpack = nonitem;
        }

        public bool AddItem(Item item, uint slot)
        {
            if (item == null || this.IsFull || slot > this.TotalSlots)
                return false;
            if (item == this)
                return false;
            
            item.Contained = (IsBackpack ? this.Owner : this.Guid);
            item.Owner = this.Owner;
            item.CurrentSlot = slot;
            item.SetSoulboundState(IsBagPos(slot));

            if (!this.Items.ContainsKey(item.Guid))
                this.Items.Add(item.Guid, item);

            if (!this.Slots.ContainsKey(slot))
                this.Slots.Add(slot, item.Guid); //New item
            else
                this.Slots[slot] = item.Guid; //Replace existing

            return true;
        }

        public bool AddItem(Item item)
        {
            if (item == null)
                return false;

            int slot = NextSlot();
            return (slot < 0 ? false : AddItem(item, (uint)slot));
        }

        public Item GetItem(byte slot)
        {
            if (this.Slots.ContainsKey(slot))
                if (this.Items.ContainsKey(this.Slots[slot]))
                    return this.Items[this.Slots[slot]];

            return null;
        }

        public bool RemoveItem(Item itm)
        {
            if (itm == null)
                return false;

            if (this.Items.ContainsKey(itm.Guid) && this.Slots.ContainsValue(itm.Guid))
            {
                this.Items.Remove(itm.Guid);
                this.Slots.Remove(itm.CurrentSlot);
                return true;
            }

            return false;
        }

        public bool RemoveItem(uint entry)
        {
            ulong key = this.Items.Values.FirstOrDefault(x => x.Entry == entry)?.Guid ?? 0;
            if (this.Items.ContainsKey(key))
            {
                this.Items.Remove(key);
                return true;
            }

            return false;
        }

        public bool RemoveItemInSlot(uint slot)
        {
            if (!this.Slots.ContainsKey(slot))
                return false;

            return RemoveItem(this.Items[this.Slots[slot]]);
        }

        public bool HasItem(uint entry)
        {
            return this.Items.Values.Any(x => x.Entry == entry);
        }

        public bool HasItem(Item item)
        {
            if (item == null)
                return false;

            return this.Items.Values.Any(x => x == item);
        }


        private int NextSlot()
        {
            uint startslot = (IsBackpack ? (uint)InventorySlots.SLOT_INBACKPACK : 0);
            for (uint i = startslot; i < this.TotalSlots; i++)
                if (!this.Slots.ContainsKey(i))
                    return (int)i;

            return -1;
        }

        private bool IsBagPos(uint slot)
        {
            return ((slot >= (uint)InventorySlots.SLOT_BAG1 && slot < (uint)InventorySlots.SLOT_INBACKPACK) ||
                    (slot >= 63 && slot < 69));
        }
    }
}
