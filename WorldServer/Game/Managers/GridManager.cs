using Common.Constants;
using Common.Helpers;
using Common.Network.Packets;
using Common.Singleton;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldServer.Game.Objects;
using System.Linq;
using Common.Helpers.Extensions;
using Common.Logging;

namespace WorldServer.Game.Managers
{
    public class GridManager : SingletonBase<GridManager>
    {
        private const float TOLERANCE = 0.00001f;
        private static Opcodes[] UPDATE_OPCODES = new[] { Opcodes.SMSG_COMPRESSED_UPDATE_OBJECT, Opcodes.SMSG_UPDATE_OBJECT };

        public const float GRID_SIZE = 300f;
        public ConcurrentDictionary<string, Grid> Grids = new ConcurrentDictionary<string, Grid>();

        public Grid AddOrGet(WorldObject obj, bool store = false)
        {
            Grid grid = null;
            GridCoords coords = new GridCoords(obj.Location, obj.Map);

            if (this.Grids.ContainsKey(coords.Key))
                grid = this.Grids[coords.Key];
            else
            {
                grid = new Grid(coords.MinX, coords.MaxX, coords.MinY, coords.MaxY, coords.Map);
                this.Grids.TryAdd(grid.Key, grid);
            }

            if (store)
                grid.TryAdd(obj);

            return grid;
        }

        public void UpdateObject(WorldObject obj)
        {
            if (this.Grids.ContainsKey(obj.Grid.Value)) //Already associated
            {
                Grid current = this.Grids[obj.Grid.Value];
                if (current.Contains(obj)) //Same grid
                    return;

                if (current.TryRemove(obj))
                    current.SendAll(obj.BuildDestroy());
            }

            GridCoords coords = new GridCoords(obj.Location, obj.Map);

            if (this.Grids.ContainsKey(coords.Key))
                this.Grids[coords.Key].TryAdd(obj);
            else
                AddOrGet(obj, true);
                
        }

        public HashSet<Grid> GetSurrounding(WorldObject obj)
        {
            Vector vector = obj.Location;
            uint map = obj.Map;
            HashSet<Grid> grids = new HashSet<Grid>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Grid found = null;
                    Vector vec = new Vector(vector.X + (x * GRID_SIZE), vector.Y + (y * GRID_SIZE), 0);
                    GridCoords coords = new GridCoords(vec, map);

                    if (this.Grids.ContainsKey(coords.Key) && this.Grids.TryGetValue(coords.Key, out found))
                        grids.Add(found);
                }
            }

            return grids;
        }

        public void SendSurrounding(PacketWriter packet, WorldObject obj)
        {
            bool force = !(UPDATE_OPCODES.Contains(packet.Opcode) && obj.IsTypeOf(ObjectTypes.TYPE_PLAYER));

            foreach (Grid grid in GetSurrounding(obj))
            {
                if (obj.IsTypeOf(ObjectTypes.TYPE_PLAYER) && !force)
                    grid.SendAll(packet, (Player)obj);
                else
                    grid.SendAll(packet);
            }
        }

        public void SendSurroundingNotMe(PacketWriter packet, Player obj)
        {
            foreach (Grid grid in GetSurrounding(obj))
                grid.SendAllExcept(packet, obj);
        }

        public void SendSurroundingInRange(PacketWriter packet, WorldObject obj, float range)
        {
            bool force = !(UPDATE_OPCODES.Contains(packet.Opcode) && obj.IsTypeOf(ObjectTypes.TYPE_PLAYER));

            foreach (Grid grid in GetSurrounding(obj))
            {
                if (obj.IsTypeOf(ObjectTypes.TYPE_PLAYER) && !force)
                    grid.SendAllInRange(packet, obj, range, (Player)obj);
                else
                    grid.SendAllInRange(packet, obj, range);
            }
        }

        public void SendSurroundingInRangeNotMe(PacketWriter packet, Player obj, float range)
        {
            foreach (Grid grid in GetSurrounding(obj))
                grid.SendAllInRangeExcept(packet, obj, range, obj);
        }

        public HashSet<WorldObject> GetSurroundingObjects(WorldObject obj, bool playersonly = false)
        {
            List<WorldObject> allobjects = new List<WorldObject>();

            foreach (Grid grid in GetSurrounding(obj))
                if (playersonly)
                    allobjects.AddRange(grid.Players.Values);
                else
                    allobjects.AddRange(grid.AllObjects.Values);

            return new HashSet<WorldObject>(allobjects);
        }

        public HashSet<Player> GetOtherPlayersInZone(Player p, bool self = true)
        {
            List<Player> players = new List<Player>();
            uint zone = p.Zone;

            foreach (Grid grid in this.Grids.Values)
                if (grid.Zones.Contains(zone))
                    players.AddRange(grid.Players.Values.Where(x => x.Zone == zone));

            if (!self && players.Contains(p))
                players.Remove(p);

            return new HashSet<Player>(players);
        }

        internal struct GridCoords
        {
            public float MinX;
            public float MinY;
            public float MaxX;
            public float MaxY;
            public uint Map;
            public string Key { get { return Math.Round(MinX, 5) + ":" + Math.Round(MinY, 5) + ":" + Math.Round(MaxX, 5) + ":" + Math.Round(MaxY, 5) + ":" + Map; } }

            public GridCoords(Vector vector, uint map)
            {
                float modx = vector.X / GRID_SIZE;
                float mody = vector.Y / GRID_SIZE;

                this.MaxX = (float)(Math.Ceiling(modx) * GRID_SIZE) - TOLERANCE;
                this.MaxY = (float)(Math.Ceiling(mody) * GRID_SIZE) - TOLERANCE;
                this.MinX = this.MaxX - GRID_SIZE + TOLERANCE;
                this.MinY = this.MaxY - GRID_SIZE + TOLERANCE;
                this.Map = map;
            }
        }
    }

    public class Grid
    {
        public float MinX { get; private set; }
        public float MinY { get; private set; }
        public float MaxX { get; private set; }
        public float MaxY { get; private set; }
        public uint Map { get; private set; }
        public HashSet<uint> Zones { get; private set; }

        public string Key { get; private set; }

        public bool HasPlayers { get { return this.Players.Count > 0; } }
        public bool HasObjects { get { return this.AllObjects.Count > 0; } }

        public ConcurrentDictionary<ulong, GameObject> GameObjects = new ConcurrentDictionary<ulong, GameObject>();
        public ConcurrentDictionary<ulong, Creature> Creatures = new ConcurrentDictionary<ulong, Creature>();
        public ConcurrentDictionary<ulong, Player> Players = new ConcurrentDictionary<ulong, Player>();
        public ConcurrentDictionary<ulong, WorldObject> AllObjects = new ConcurrentDictionary<ulong, WorldObject>();

        public Grid(float xmin, float xmax, float ymin, float ymax, uint map)
        {
            this.Zones = new HashSet<uint>();
            this.MinX = xmin;
            this.MaxX = xmax;
            this.MinY = ymin;
            this.MaxY = ymax;
            this.Map = map;
            this.Key = Math.Round(MinX, 5) + ":" + Math.Round(MinY, 5) + ":" + Math.Round(MaxX, 5) + ":" + Math.Round(MaxY, 5) + ":" + Map;
        }

        public bool Contains(WorldObject obj)
        {
            return Contains(obj.Location, obj.Map);
        }

        public bool Contains(Vector vector, uint map)
        {
            return (Math.Round(vector.X, 5) >= MinX &&
                    Math.Round(vector.X, 5) <= MaxX &&
                    Math.Round(vector.Y, 5) >= MinY &&
                    Math.Round(vector.Y, 5) <= MaxY &&
                    map == Map);
        }

        public bool TryAdd(WorldObject obj)
        {
            bool success = false;

            if (this.AllObjects.ContainsKey(obj.Guid)) //Already contained
                return true;

            if (obj.IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                Player p = (Player)obj;
                if(p.IsOnline && this.Players.TryAdd(obj.Guid, (Player)obj))
                {
                    success = true;
                    this.Zones.Add(((Player)obj).Zone); //Store player zone
                }
            }
            else if (obj.IsTypeOf(ObjectTypes.TYPE_UNIT))
                success = this.Creatures.TryAdd(obj.Guid, (Creature)obj);
            else if (obj.IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
                success = this.GameObjects.TryAdd(obj.Guid, (GameObject)obj);

            if (success && this.AllObjects.TryAdd(obj.Guid, obj))
            {
                obj.Grid = new TReadOnly<string>(this.Key); //Object moved update grid reference
                return true;
            }

            return false;
        }

        public bool TryRemove(WorldObject obj)
        {
            bool success = false;
            WorldObject dump = null;
            Player pdump = null;
            Creature cdump = null;
            GameObject gdump = null;

            if (!this.AllObjects.ContainsKey(obj.Guid)) //Already removed
                return true;

            if (obj.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                success = this.Players.TryRemove(obj.Guid, out pdump);
            else if (obj.IsTypeOf(ObjectTypes.TYPE_UNIT))
                success = this.Creatures.TryRemove(obj.Guid, out cdump);
            else if (obj.IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
                success = this.GameObjects.TryRemove(obj.Guid, out gdump);

            if (success)
                return this.AllObjects.TryRemove(obj.Guid, out dump);

            return false;
        }

        public void SendAll(PacketWriter packet, Player creator = null)
        {
            Parallel.ForEach(this.Players.Values, p =>
            {
                if (creator != null && p == creator)
                    creator.Dirty = true;
                else if (p.LoggedIn)
                    p.Client.Send(packet, true);
            });
        }

        public void SendAllExcept(PacketWriter packet, Player exception)
        {
            Parallel.ForEach(this.Players.Values, p =>
            {
                if (p.LoggedIn && p != exception)
                    p.Client.Send(packet, true);
            });
        }

        public void SendAllInRange(PacketWriter packet, WorldObject source, float range, Player creator = null)
        {
            if (range <= 0)
            {
                SendAll(packet, creator);
                return;
            }

            Parallel.ForEach(this.Players.Values, p =>
            {
                if (creator != null && p == creator)
                    creator.Dirty = true;
                else if (p.LoggedIn && p.Location.Distance(source.Location) <= range)
                    p.Client.Send(packet, true);
            });
        }

        public void SendAllInRangeExcept(PacketWriter packet, WorldObject source, float range, Player exception)
        {
            if (range <= 0)
            {
                SendAllExcept(packet, exception);
                return;
            }

            Parallel.ForEach(this.Players.Values, p =>
            {
                if (p.LoggedIn && p.Location.Distance(source.Location) <= range && p != exception)
                    p.Client.Send(packet);
            });
        }
    }
}
