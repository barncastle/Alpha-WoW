using System.Collections.Generic;
using Common.Network.Packets;
using WorldServer.Network;
using Common.Logging;

namespace WorldServer.Packets
{
    public static class PacketManager
    {
        public static Dictionary<Opcodes, HandlePacket> OpcodeHandlers = new Dictionary<Opcodes, HandlePacket>();
        public delegate void HandlePacket(ref PacketReader packet, ref WorldManager manager);

        public static void DefineOpcodeHandler(Opcodes opcode, HandlePacket handler)
        {
            OpcodeHandlers[opcode] = handler;
        }

        public static bool InvokeHandler(PacketReader reader, WorldManager manager, Opcodes opcode)
        {
            if (OpcodeHandlers.ContainsKey(opcode))
            {
                OpcodeHandlers[opcode].Invoke(ref reader, ref manager);
                return true;
            }
            else
            {
                Log.Message(LogType.ERROR, "UNPROGRAMMED OPCODE: {0} - {1}", opcode, (byte)opcode);
                return false;
            }
        }
    }
}
