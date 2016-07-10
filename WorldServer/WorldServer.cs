using System;
using Common.Logging;
using WorldServer.Game.Commands;
using WorldServer.Network;
using WorldServer.Packets;
using WorldServer.Game;
using Common.Database;

namespace WorldServer
{
    class WorldServer
    {
        static void Main()
        {
            Log.Message(LogType.INIT, "                 ALPHA WOW                 ");
            Log.Message(LogType.INIT, "             REALM/PROXY/WORLD             ");
            Log.Message();

            Log.Message(LogType.NORMAL, "Starting Alpha WoW Server...");

            Globals.Initialize();
            Log.Message();

            RealmManager.RealmSession = new RealmSocket();
            WorldManager.WorldSession = new WorldSocket();
            if (WorldManager.WorldSession.Start() && RealmManager.RealmSession.Start())
            {
                RealmManager.RealmSession.StartRealmThread();
                RealmManager.RealmSession.StartProxyThread();
                Log.Message(LogType.NORMAL, "RealmProxy listening on {0} port {1}/{2}.", Globals.SERVER_IP, Globals.REALM_PORT, Globals.PROXY_PORT);
                Log.Message(LogType.NORMAL, "RealmProxy successfully started!");

                WorldManager.WorldSession.StartConnectionThread();
                Log.Message(LogType.NORMAL, "WorldServer listening on {0} port {1}.", Globals.SERVER_IP, Globals.WORLD_PORT);
                Log.Message(LogType.NORMAL, "WorldServer successfully started!");
                Log.Message();

                HandlerDefinitions.InitializePacketHandler();
            }
            else
            {
                Log.Message(LogType.ERROR, "WorldServer couldn't be started: ");
                Log.Message();
            }

            // Free memory...
            GC.Collect();
            Log.Message(LogType.NORMAL, "Total Memory: {0}MB", Convert.ToSingle(GC.GetTotalMemory(false) / 1024 / 1024));

            // Init Command handlers...
            ConsoleManager.InitCommands();
        }
    }
}
