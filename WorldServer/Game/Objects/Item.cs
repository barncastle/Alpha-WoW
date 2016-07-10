using Common.Constants;
using Common.Database;
using Common.Network.Packets;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using WorldServer.Game.Structs;
using WorldServer.Storage;

namespace WorldServer.Game.Objects
{
    [Table("character_inventory")]
    public class Item : WorldObject
    {
        public uint Entry;
        public ulong Owner;
        public ulong Creator;
        public ulong Contained;
        public InventoryTypes Type;
        public uint Bag;
        public uint CurrentSlot;
        public uint StackCount = 1;
        public ItemTemplate Template;
        public bool IsContainer { get { return Type == InventoryTypes.BAG; } }
        public bool IsSoulbound { get; private set; } = false;
        public bool IsEquipmentPos { get { return CurrentSlot < (uint)InventorySlots.SLOT_BAG1; } }
        public uint[] SpellCharges = new uint[5];
        public uint EquipSlot = 0;
        public ulong Player = 0;

        public Item()
        {
            this.Guid = Globals.ITEM_GUID + 1;
            while (Database.Items.ContainsKey(this.Guid))
                this.Guid++;

            Globals.ITEM_GUID = this.Guid;
            this.ObjectType |= ObjectTypes.TYPE_ITEM;
        }

        public Item(uint entry)
        {
            this.Guid = Globals.ITEM_GUID + 1;
            while (Database.Items.ContainsKey(this.Guid))
                this.Guid++;

            Globals.ITEM_GUID = this.Guid;
            this.ObjectType |= ObjectTypes.TYPE_ITEM;
            this.Template = Database.ItemTemplates.TryGet(entry);
            this.DisplayID = this.Template.DisplayID;
            this.EquipSlot = PrefInvSlot();
            this.SpellCharges = this.Template.SpellCharges;
            this.Entry = entry;
        }

        public Item(ref MySqlDataReader dr)
        {
            this.Guid = Convert.ToUInt64(dr["item"]) | (ulong)HIGH_GUID.HIGHGUID_ITEM;
            this.Owner = Convert.ToUInt64(dr["owner"]);
            this.Bag = Convert.ToUInt32(dr["bag"]);
            this.CurrentSlot = Convert.ToUInt32(dr["slot"]);
            this.Entry = Convert.ToUInt32(dr["item_template"]);
            this.StackCount = Convert.ToUInt32(dr["stackcount"]);
            this.Player = Convert.ToUInt64(dr["player"]);
            this.SpellCharges = new[] { Convert.ToUInt32(dr["SpellCharges1"]),
                                        Convert.ToUInt32(dr["SpellCharges2"]),
                                        Convert.ToUInt32(dr["SpellCharges3"]),
                                        Convert.ToUInt32(dr["SpellCharges4"]),
                                        Convert.ToUInt32(dr["SpellCharges5"]) };
        }

        public PacketWriter CreateItem()
        {
            //Ensure values are right
            this.ObjectType |= ObjectTypes.TYPE_ITEM;

            if (this.Template == null)
                this.Template = Database.ItemTemplates.TryGet(this.Entry);

            this.DisplayID = this.Template.DisplayID;
            this.Type = (InventoryTypes)this.Template.InvType;
            this.SpellCharges = this.Template.SpellCharges;
            this.EquipSlot = PrefInvSlot();

            //List management
            Database.Items.TryAdd(this);
            return this.BuildUpdate();
        }

        public override PacketWriter BuildUpdate(UpdateTypes type = UpdateTypes.UPDATE_FULL, bool self = false)
        {
            PacketWriter packet = null;
            switch (type)
            {
                case UpdateTypes.UPDATE_FULL:
                    packet = CreateObject(false);
                    break;
                default:
                    packet = new PacketWriter(Opcodes.SMSG_UPDATE_OBJECT);
                    break;
            }

            UpdateClass uc = new UpdateClass();
            uc.UpdateValue<ulong>(ObjectFields.OBJECT_FIELD_GUID, this.Guid);
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_TYPE, (uint)this.ObjectType);
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_ENTRY, this.Entry);
            uc.UpdateValue<float>(ObjectFields.OBJECT_FIELD_SCALE_X, 1f);
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_PADDING, 0);
            uc.UpdateValue<ulong>(ItemFields.ITEM_FIELD_OWNER, this.Owner);
            uc.UpdateValue<ulong>(ItemFields.ITEM_FIELD_CREATOR, this.Creator);
            uc.UpdateValue<ulong>(ItemFields.ITEM_FIELD_CONTAINED, this.Contained);
            uc.UpdateValue<uint>(ItemFields.ITEM_FIELD_STACK_COUNT, this.StackCount);
            uc.UpdateValue<uint>(ItemFields.ITEM_FIELD_FLAGS, this.Template.Flags);

            for (int i = 0; i < 5; i++)
                uc.UpdateValue<uint>(ItemFields.ITEM_FIELD_SPELL_CHARGES, this.SpellCharges[i], i);

            if (this.IsContainer) //Is a container
            {
                var c = (Container)this;
                if (c.IsBackpack) //Just incase
                    return null;

                uc.UpdateValue<uint>(ContainerFields.CONTAINER_FIELD_NUM_SLOTS, c.TotalSlots);

                for (int i = 0; i < Container.MAX_BAG_SLOTS; i++)
                {
                    ulong iguid = c.Slots.ContainsKey((uint)i) ? c.Slots[(uint)i] : 0;
                    uc.UpdateValue<ulong>(ContainerFields.CONTAINER_FIELD_SLOT_1, iguid, i * 2);
                }
            }

            switch (type)
            {
                case UpdateTypes.UPDATE_PARTIAL:
                    return UpdateObject(ref uc);
                case UpdateTypes.UPDATE_FULL:
                    uc.BuildPacket(ref packet, true);
                    return packet;
                default:
                    return null;
            }
        }

        public void SetSoulboundState(bool isBagPos)
        {
            IsSoulbound = (isBagPos && Template.Bonding == (uint)ItemBondingTypes.BIND_WHEN_EQUIPPED) ||
                          (Template.Bonding == (uint)ItemBondingTypes.BIND_QUEST_ITEM) ||
                          (Template.Bonding == (uint)ItemBondingTypes.BIND_WHEN_PICKED_UP);
        }

        public Item Clone(uint stackcount = 0)
        {
            Item item = Database.ItemTemplates.CreateItemOrContainer(this.Entry);
            item.Owner = this.Owner;
            item.Creator = this.Creator;
            item.Type = this.Type;

            if (stackcount > 0)
                item.StackCount = stackcount;
            else
                item.StackCount = this.StackCount;
            return item;
        }

        #region Database Functions
        public void OnDbLoad()
        {
            this.Template = Database.ItemTemplates.TryGet(this.Entry);
            this.DisplayID = this.Template.DisplayID;
            this.Type = (InventoryTypes)this.Template.InvType;
            this.SpellCharges = this.Template.SpellCharges;
            this.EquipSlot = PrefInvSlot();
        }

        public void Save()
        {
            List<string> columns = new List<string>() {
                "item", "owner", "bag", "slot", "item_template", "stackcount", "player",
                "SpellCharges1", "SpellCharges2", "SpellCharges3", "SpellCharges4", "SpellCharges5"
            };

            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@item", this.Guid & ~(ulong)HIGH_GUID.HIGHGUID_ITEM),
                new MySqlParameter("@owner", this.Owner),
                new MySqlParameter("@bag", this.Bag),
                new MySqlParameter("@slot", this.CurrentSlot),
                new MySqlParameter("@item_template", this.Entry),
                new MySqlParameter("@stackcount", this.StackCount),
                new MySqlParameter("@player", this.Player),
                new MySqlParameter("@SpellCharges1", this.SpellCharges[0]),
                new MySqlParameter("@SpellCharges2", this.SpellCharges[1]),
                new MySqlParameter("@SpellCharges3", this.SpellCharges[2]),
                new MySqlParameter("@SpellCharges4", this.SpellCharges[3]),
                new MySqlParameter("@SpellCharges5", this.SpellCharges[4])
            };

            BaseContext.SaveEntity("character_inventory", columns, parameters, Globals.CONNECTION_STRING);
        }
        #endregion

        protected uint PrefInvSlot()
        {
            int[] slotTypes = new int[(int)InventoryTypes.NUM_TYPES]{
                (int)InventorySlots.SLOT_INBACKPACK, // NONE EQUIP
	            (int)InventorySlots.SLOT_HEAD,
                (int)InventorySlots.SLOT_NECK,
                (int)InventorySlots.SLOT_SHOULDERS,
                (int)InventorySlots.SLOT_SHIRT,
                (int)InventorySlots.SLOT_CHEST,
                (int)InventorySlots.SLOT_WAIST,
                (int)InventorySlots.SLOT_LEGS,
                (int)InventorySlots.SLOT_FEET,
                (int)InventorySlots.SLOT_WRISTS,
                (int)InventorySlots.SLOT_HANDS,
                (int)InventorySlots.SLOT_FINGERL,
                (int)InventorySlots.SLOT_TRINKETL,
                (int)InventorySlots.SLOT_MAINHAND, // 1h
	            (int)InventorySlots.SLOT_OFFHAND, // shield
	            (int)InventorySlots.SLOT_RANGED,
                (int)InventorySlots.SLOT_BACK,
                (int)InventorySlots.SLOT_MAINHAND, // 2h
	            (int)InventorySlots.SLOT_BAG1,
                (int)InventorySlots.SLOT_TABARD,
                (int)InventorySlots.SLOT_CHEST, // robe
	            (int)InventorySlots.SLOT_MAINHAND, // mainhand
	            (int)InventorySlots.SLOT_OFFHAND, // offhand
	            (int)InventorySlots.SLOT_MAINHAND, // held
	            (int)InventorySlots.SLOT_INBACKPACK, // ammo
	            (int)InventorySlots.SLOT_RANGED, // thrown
	            (int)InventorySlots.SLOT_RANGED // rangedright
            };

            return (uint)slotTypes[this.Template.InvType];
        }
    }
}
