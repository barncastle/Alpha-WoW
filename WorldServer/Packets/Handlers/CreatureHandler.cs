using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldServer.Game;
using WorldServer.Game.Objects;
using WorldServer.Network;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class CreatureHandler
    {
        public static void HandleCreatureQuery(ref PacketReader packet, ref WorldManager manager)
        {
            uint Entry = packet.ReadUInt32();
            ulong Guid = packet.ReadUInt64();
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_CREATURE_QUERY_RESPONSE);

            if (Database.Creatures.ContainsKey(Guid))
            {
                Creature mob = Database.Creatures.TryGet(Guid);
                manager.Send(mob.Template.QueryDetails());
                manager.Character.QueriedObjects.Add(mob.Entry);
            }
            else
            {
                pw.WriteUInt32(Entry | 0x80000000);
                manager.Send(pw);
            }
        }
    }
}
