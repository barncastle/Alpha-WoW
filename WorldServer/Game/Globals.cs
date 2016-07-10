using Common.Constants;
using Common.Database.DBC;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Timers;
using WorldServer.Game.Managers;
using WorldServer.Game.Objects;
using WorldServer.Storage;

namespace WorldServer.Game
{
    public class Globals
    {
        //System Timer
        private static Timer UpdateTimer;
        private static Timer SaveTimer;

        //Constants
        public const string SERVER_IP = "127.0.0.1";
        public const int REALM_PORT = 9100;
        public const int PROXY_PORT = 9090;
        public const int WORLD_PORT = 8100;
        public const int CLIENT_VERSION = 3368;
        public const float UPDATE_DISTANCE = 155f;
        public const float INTERACT_DISTANCE = 5.0f;
        public const byte MAX_LEVEL = 25; //Alpha level cap
        public const string WELCOME_MESSAGE = "Welcome to Alpha WoW";
        public const float EMOTE_RANGE = 50f;
        public const float SAY_RANGE = 50f;
        public const float YELL_RANGE = 300f;
        public const float MAX_GROUP_XP_DISTANCE = 75f;
        public const int MAX_FRIEND_LIST = 25;
        public const int MAX_IGNORE_LIST = 25;
        public const bool AUTO_CREATE_ACCOUNT = true;
        public static readonly string CONNECTION_STRING = ConfigurationManager.AppSettings["ConnectionString"];

        //Update Variables
        public static long Time { get { return DateTime.Now.Ticks; } }

        //Basic GUID system
        public static ulong ITEM_GUID = (ulong)HIGH_GUID.HIGHGUID_ITEM;
        public static ulong UNIT_GUID = (ulong)HIGH_GUID.HIGHGUID_UNIT;
        public static ulong PLAYER_GUID = (ulong)HIGH_GUID.HIGHGUID_PLAYER;
        public static ulong CLIENT_ID = (ulong)HIGH_GUID.HIGHGUID_PLAYER;
        public static ulong GO_GUID = (ulong)HIGH_GUID.HIGHGUID_GAMEOBJECT;
        
        public static void Initialize()
        {   
            DBC.Initialize();
            Database.Initialize();
            SpellEffect.InitSpellEffects();
                      
            UpdateTimer = new Timer() { Enabled = true, Interval = 50 };
            UpdateTimer.Elapsed += new ElapsedEventHandler(Update);

            SaveTimer = new Timer() { Enabled = true, Interval = 60000 };
            SaveTimer.Elapsed += new ElapsedEventHandler(Save);
        }

        #region Update Functions
        public static void Update(object sender, ElapsedEventArgs e)
        {
            Parallel.ForEach(GridManager.Instance.Grids.Values, x =>
            {
                if (x.HasPlayers)
                    Parallel.ForEach(x.AllObjects.Values, p => p.Update(Time));
            });
        }

        private static void Save(object sender, ElapsedEventArgs e)
        {
            Database.SaveChanges();
        }

        public static long GetFutureTime(float sec)
        {
            return Time + (long)(sec * TimeSpan.TicksPerSecond);
        }
        #endregion
    }
}
