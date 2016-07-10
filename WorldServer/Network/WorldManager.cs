using System;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;
using Common.Network.Packets;
using WorldServer.Packets;
using WorldServer.Game;
using WorldServer.Game.Objects;
using System.Threading.Tasks;
using WorldServer.Storage;
using WorldServer.Game.Managers;
using System.Collections.Generic;

namespace WorldServer.Network
{
    public class WorldManager
    {
        public ulong Id;
        public Account Account;
        public Socket Socket;
        public Player Character;
        public static WorldSocket WorldSession;
        byte[] buffer = null;

        public void OnData()
        {
            PacketReader pkt = new PacketReader(buffer);

            if (Enum.IsDefined(typeof(Opcodes), pkt.Opcode))
                Log.Message(LogType.DUMP, "Recieved OPCODE: {0}, LENGTH: {1}", pkt.Opcode, pkt.Size);
            else
                Log.Message(LogType.DUMP, "UNKNOWN OPCODE: {0}, LENGTH: {1}", pkt.Opcode, pkt.Size);

            Log.Message();
            PacketManager.InvokeHandler(pkt, this, pkt.Opcode);
            Log.Message();
        }

        public void Recieve()
        {
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_AUTH_CHALLENGE);
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            this.Send(writer);

            while (WorldSession.listenWorldSocket)
            {
                Thread.Sleep(1);
                if (Socket.Connected && Socket.Available > 0)
                {
                    buffer = new byte[Socket.Available];
                    Socket.Receive(buffer, buffer.Length, SocketFlags.None);

                    OnData();
                }
            }

            CloseSocket();
        }

        public void Send(PacketWriter packet, bool SuppressLog = false)
        {
            if (packet == null)
                return;

            byte[] buffer = packet.ReadDataToSend();

            try
            {
                Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(FinishSend), Socket);

                if (!SuppressLog)
                {
                    Log.Message(LogType.DUMP, "Send {0}.", packet.Opcode);
                    Log.Message();
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
                Log.Message();

                CloseSocket();
            }
        }

        public void CloseSocket()
        {
            if (this.Character != null && this.Character.LoggedIn)
                Character.Logout();

            Socket.Close();
        }

        public void FinishSend(IAsyncResult result)
        {
            Socket.EndSend(result);
        }
    }
}
