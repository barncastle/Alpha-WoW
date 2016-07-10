using Common.Network.Packets;
using System;
using System.Linq;
using WorldServer.Game;
using WorldServer.Game.Objects;
using WorldServer.Game.Structs;
using WorldServer.Network;
using WorldServer.Storage;
using WorldServer.Game.Objects.PlayerExtensions;
using WorldServer.Game.Objects.PlayerExtensions.Quests;
using Common.Constants;

namespace WorldServer.Packets.Handlers
{
    public class ItemHandler
    {

        public static void HandleItemQuerySingle(ref PacketReader packet, ref WorldManager manager)
        {
            uint entry = packet.ReadUInt32();
            manager.Character.QueryItemCheck(entry);
        }

        public static void HandleDestroyItem(ref PacketReader packet, ref WorldManager manager)
        {
            byte bag = packet.ReadUInt8();
            byte sourceslot = packet.ReadUInt8();
            byte count = packet.ReadUInt8();

            uint bagslot = (uint)InventorySlots.SLOT_INBACKPACK;

            Item item = manager.Character.Inventory.GetBag(bagslot)?.GetItem(sourceslot);
            if (item == null)
                return;

            if (Database.Items.TryRemove(item.Guid))
            {
                manager.Character.CheckQuestItemRemove(item.Entry, count);
                manager.Character.Inventory.GetBag(bagslot)?.RemoveItemInSlot(sourceslot);

                PacketWriter writer = new PacketWriter(Opcodes.CMSG_DESTROYITEM);
                writer.WriteUInt64(item.Guid); //Item GUID
                manager.Send(writer);
                manager.Character.Dirty = true;
            }
        }

        public static void HandleSwapItem(ref PacketReader packet, ref WorldManager manager)
        {
            byte dstbag = packet.ReadUInt8();
            byte dstslot = packet.ReadUInt8();
            byte srcbag = packet.ReadUInt8();
            byte srcslot = packet.ReadUInt8();

            manager.Character.SwapItem(srcbag, srcslot, dstbag, dstslot);
        }

        public static void HandleSwapInventoryItem(ref PacketReader packet, ref WorldManager manager)
        {
            byte srcslot = packet.ReadUInt8();
            byte dstslot = packet.ReadUInt8();
            byte bag = (byte)InventorySlots.SLOT_INBACKPACK;
            manager.Character.SwapItem(bag, srcslot, bag, dstslot);
        }

        public static void HandleAutoEquipItem(ref PacketReader packet, ref WorldManager manager)
        {
            byte srcbag = packet.ReadUInt8();
            byte srcslot = packet.ReadUInt8();
            byte dstslot = 0;

            byte srcbagslot = (byte)(srcbag == 255 ? 23 : srcbag);
            Item srcItem = manager.Character.Inventory.GetBag(srcbagslot)?.GetItem(srcslot);
            if (srcItem == null) //No item to move - cheat?
                return;

            dstslot = (byte)srcItem.EquipSlot;
            manager.Character.SwapItem(srcbagslot, srcslot, (byte)InventorySlots.SLOT_INBACKPACK, dstslot);
        }

        public static void HandleAutostoreBagItem(ref PacketReader packet, ref WorldManager manager)
        {
            byte srcbag = packet.ReadUInt8();
            byte srcslot = packet.ReadUInt8();
            byte dstbag = packet.ReadUInt8();

            uint srcbagslot = (uint)(srcbag == 255 ? 23 : srcbag);
            uint dstbagslot = (uint)(dstbag == 255 ? 23 : dstbag);

            Item item = manager.Character.Inventory.GetBag(srcbagslot)?.GetItem(srcslot);
            Container container = manager.Character.Inventory.GetBag(dstbagslot);
            if (item == null || container == null)
                return;

            if (container.IsFull)
            {
                manager.Character.SendEquipError(InventoryError.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT, item, null);
                return;
            }

            if (item.IsContainer && !((Container)item).IsEmpty)
            {
                manager.Character.SendEquipError(InventoryError.EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS, item, null);
                return;
            }

            if (manager.Character.InCombat && item.IsEquipmentPos)
            {
                manager.Character.SendEquipError(InventoryError.EQUIP_ERR_NOT_IN_COMBAT, item, null);
                return;
            }

            manager.Character.Inventory.GetBag(srcbagslot).RemoveItem(item);
            manager.Character.Inventory.GetBag(dstbagslot).AddItem(item);
            manager.Character.Dirty = true;
        }

        public static void HandleSplitItemOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            byte srcbag = packet.ReadUInt8();
            byte srcslot = packet.ReadUInt8();
            byte dstbag = packet.ReadUInt8();
            byte dstslot = packet.ReadUInt8();
            byte count = packet.ReadUInt8();

            if ((srcbag == dstbag && srcslot == dstslot) || count == 0)
                return;

            manager.Character.SplitItem(srcbag, srcslot, dstbag, dstslot, count);
        }
    }
}
