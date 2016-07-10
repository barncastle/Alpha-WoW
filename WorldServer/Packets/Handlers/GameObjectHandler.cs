using Common.Constants;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game.Managers;
using WorldServer.Game.Objects;
using WorldServer.Network;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class GameObjectHandler
    {
        public static void HandleGameObjectQueryOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            uint entry = packet.ReadUInt32();
            ulong guid = packet.ReadUInt64();
            if (Database.GameObjects.ContainsKey(guid))
                manager.Send(Database.GameObjects.TryGet(guid).QueryDetails());
        }

        public static void HandleGameObjectUseOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            if (!Database.GameObjects.ContainsKey(guid))
                return;

            GameObject go = Database.GameObjects.TryGet(guid);
            if (go.Template.Type == (uint)GameObjectTypes.TYPE_GENERIC)
                return;

            go.Use(manager.Character);
        }
    }
}
