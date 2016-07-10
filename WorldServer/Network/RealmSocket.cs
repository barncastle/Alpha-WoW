using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;
using WorldServer.Game;

namespace WorldServer.Network
{
    public class RealmSocket
    {
        public bool listenRealmSocket = true;
        public bool listenProxySocket = true;
        private TcpListener realmListener;
        private TcpListener proxyListener;

        public bool Start()
        {
            try
            {
                realmListener = new TcpListener(IPAddress.Parse(Globals.SERVER_IP), Globals.REALM_PORT); 
                realmListener.Start();

                proxyListener = new TcpListener(IPAddress.Parse(Globals.SERVER_IP), Globals.PROXY_PORT);
                proxyListener.Start();

                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                Log.Message();

                return false;
            }
        }

        public void StartRealmThread()
        {
            new Thread(AcceptRealmConnection).Start();
        }

        public void StartProxyThread()
        {
            new Thread(AcceptProxyConnection).Start();
        }

        protected void AcceptRealmConnection()
        {
            while (listenRealmSocket)
            {
                Thread.Sleep(1);
                if (realmListener.Pending())
                {
                    RealmManager Realm = new RealmManager();
                    Realm.realmSocket = realmListener.AcceptSocket();

                    Thread NewThread = new Thread(Realm.RecieveRealm);
                    NewThread.Start();
                }
            }
        }

        protected void AcceptProxyConnection()
        {
            while (listenProxySocket)
            {
                Thread.Sleep(1);
                if (proxyListener.Pending())
                {
                    RealmManager Proxy = new RealmManager();
                    Proxy.proxySocket = proxyListener.AcceptSocket();

                    Thread NewThread = new Thread(Proxy.RecieveProxy);
                    NewThread.Start();
                }
            }
        }

        public void Dispose()
        {
            listenRealmSocket = false;
            listenProxySocket = false;
            realmListener.Stop();
            proxyListener.Stop();
        }
    }
}
