using Common.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class NetHandler
    {
        public static void HandlePing(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_PONG);
            writer.WriteUInt32(packet.ReadUInt32());
            manager.Send(writer);
        }
    }
}
