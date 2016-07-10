using System;
using System.Net.Sockets;
using System.Linq;
using Common.Logging;
using Common.Network.Packets;
using WorldServer.Game;
using WorldServer.Storage;

namespace WorldServer.Network
{
    public class RealmManager
    {
        public static RealmSocket RealmSession;
        public Socket realmSocket;
        public Socket proxySocket;

        public void HandleProxyConnection(RealmManager Session)
        {
            Log.Message();
            Log.Message(LogType.NORMAL, "Begin redirection to WorldServer.");

            PacketWriter proxyWriter = new PacketWriter();
            proxyWriter.WriteString(Globals.SERVER_IP + ":" + Globals.WORLD_PORT);

            Session.Send(proxyWriter, proxySocket);
            proxySocket.Close();

            Log.Message(LogType.NORMAL, "Successfully redirected to WorldServer");
            Log.Message();
        }
        
        public void HandleRealmList(RealmManager Session)
        {
            PacketWriter realmWriter = new PacketWriter();
            realmWriter.WriteUInt8(1);
            realmWriter.WriteString("|cFF00FFFFAlpha Test Realm");
            realmWriter.WriteString(Globals.SERVER_IP + ":" + Globals.PROXY_PORT);
            realmWriter.WriteUInt32((uint)Database.Players.Values.Count(x => x.IsOnline));

            Session.Send(realmWriter, realmSocket);
            realmSocket.Close();
        }

        public void RecieveRealm()
        {
            HandleRealmList(this);
        }

        public void RecieveProxy()
        {
            HandleProxyConnection(this);
        }

        public void Send(PacketWriter writer, Socket socket)
        {
            byte[] buffer = writer.ReadDataToSend(true);

            try
            {
                socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                Log.Message();
                socket.Close();
            }
        }
    }
}
