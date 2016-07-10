using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;
using WorldServer.Game;

namespace WorldServer.Network
{
    public class WorldSocket
    {
        public bool listenWorldSocket = true;
        private TcpListener worldListener;

        public bool Start()
        {
            try
            {
                worldListener = new TcpListener(IPAddress.Parse(Globals.SERVER_IP), Globals.WORLD_PORT);
                worldListener.Start();

                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                Log.Message();

                return false;
            }
        }

        public void StartConnectionThread()
        {
            new Thread(AcceptConnection).Start();
        }

        protected void AcceptConnection()
        {
            while (listenWorldSocket)
            {
                Thread.Sleep(1);
                if (worldListener.Pending())
                {
                    WorldManager World = new WorldManager();
                    World.Socket = worldListener.AcceptSocket();

                    Thread NewThread = new Thread(World.Recieve);
                    NewThread.Start();
                }
            }
        }

        protected void Dispose()
        {
            listenWorldSocket = false;
            worldListener.Stop();
        }
    }
}
