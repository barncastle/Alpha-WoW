using Common.Constants;
using Common.Network.Packets;
using WorldServer.Game.Managers;
using WorldServer.Network;
using WorldServer.Storage;
using WorldServer.Game.Objects.PlayerExtensions.Loot;
using Common.Helpers;

namespace WorldServer.Packets.Handlers
{
    public class LootHandler
    {
        public static void HandleLoot(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            if (manager.Character.IsDead || !Database.Creatures.ContainsKey(guid))
                return;
            
            manager.Character.CurrentLootTarget = guid;
            Flag.SetFlag(ref manager.Character.UnitFlags, (uint)UnitFlags.UNIT_FLAG_LOOTING);
            manager.Character.SendLoot(guid);
            
            GridManager.Instance.SendSurrounding(manager.Character.BuildUpdate(), manager.Character);
        }

        public static void HandleLootRelease(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            if (Database.Creatures.ContainsKey(guid))
                Database.Creatures.TryGet(guid).SendLootRelease(manager.Character);
        }

        public static void HandleLootMoney(ref PacketReader packet, ref WorldManager manager)
        {
            manager.Character.LootMoney();
        }
        
        public static void HandleAutoStoreLootItem(ref PacketReader packet, ref WorldManager manager)
        {
            byte slot = packet.ReadUInt8();
            manager.Character.LootItems(slot);
        }

    }
}
