using Common.Database.DBC;
using Common.Database.DBC.Structures;
using Common.Helpers;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldServer.Game;
using WorldServer.Game.Managers;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Game.Structs;
using WorldServer.Network;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class MiscHandler
    {
        public static void HandleQueryTime(ref PacketReader packet, ref WorldManager manager)
        {
            DateTime baseDate = new DateTime(1970, 1, 1);
            TimeSpan ts = DateTime.Now - baseDate;

            PacketWriter queryTime = new PacketWriter(Opcodes.SMSG_QUERY_TIME_RESPONSE);
            queryTime.WriteUInt32(Convert.ToUInt32(ts.TotalSeconds));
            manager.Send(queryTime);
        }

        public static void HandleLoginSetTimespeed(ref WorldManager mananger)
        {
            DateTime time = DateTime.Now;
            int Year = time.Year - 2000;
            int Month = time.Month - 1;
            int Day = time.Day - 1;
            int DayOfWeek = Convert.ToInt32(time.DayOfWeek);
            int Hour = time.Hour;
            int Minute = time.Minute;

            int curTime = Convert.ToInt32(((((Minute + (Hour << 6)) + (DayOfWeek << 11)) + (Day << 14)) + (Month << 20)) + (Year << 24));

            PacketWriter packet = new PacketWriter(Opcodes.SMSG_LOGIN_SETTIMESPEED);
            packet.WriteInt32(curTime);
            packet.WriteFloat(0.01666667f);
            mananger.Send(packet);
        }

        public static void HandleForceSpeedChange(ref WorldManager manager)
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_FORCE_SPEED_CHANGE);
            pw.WriteUInt64(manager.Character.Guid);
            pw.WriteFloat(manager.Character.RunningSpeed);
            manager.Send(pw);
        }

        public static void HandleAreaTriggerOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            uint triggerID = packet.ReadUInt32();

            if(Database.AreaTriggers.ContainsKey(triggerID))
            {
                AreaTrigger a = Database.AreaTriggers.TryGet(triggerID);
                manager.Character.Teleport(a.Map, a.GetQuaternion());
            }
        }

        public static void HandleInspectOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();

            Player target = Database.Players.TryGet(guid);
            if (target == null)
                return;

            target.Inventory.SendInventoryUpdate();
            manager.Character.CurrentSelection = guid;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_INSPECT);
            pkt.WriteUInt64(guid);
            target.Client.Send(pkt);
        }

        public static void HandlePlayedTime(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_PLAYED_TIME);
            pw.WriteUInt32(0); //TotalTime
            pw.WriteUInt32(0); //LevelTime
            manager.Send(pw);
        }

        public static void HandleWhoOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            uint[] zones = new uint[10];
            string[] ustr = new string[4];
            uint level_min = packet.ReadUInt32();
            uint level_max = packet.ReadUInt32();
            string player_name = packet.ReadString().ToLower();
            string guild_name = packet.ReadString().ToLower();
            uint racemask = packet.ReadUInt32();
            uint classmask = packet.ReadUInt32();

            uint zone_count = packet.ReadUInt32();
            if (zone_count > 10) return; //hard limit
            for (int i = 0; i < zone_count; i++)
                zones[i] = packet.ReadUInt32();

            uint ustr_count = packet.ReadUInt32();
            if (ustr_count > 4) return; //hard limit
            for (int i = 0; i < ustr_count; i++)
                ustr[i] = packet.ReadString().ToLower();
            
            uint displaycount = 0;
            HashSet<Player> players = new HashSet<Player>();

            foreach(Player p in Database.Players.Values)
            {
                if (p == manager.Character) //Hide self
                    continue;
                if (!(p.LoggedIn && p.IsOnline)) //Hide offline
                    continue;
                if (p.IsEnemyTo(manager.Character)) //Hide enemies
                    continue;
                if (p.Level < level_min || p.Level > level_max) //Level check
                    continue;
                if (!string.IsNullOrWhiteSpace(player_name) && !p.Name.ToLower().Contains(player_name)) //Name check
                    continue;
                if (!Flag.HasFlag(racemask, p.RaceMask)) //Race check
                    continue;
                if (!Flag.HasFlag(classmask, p.ClassMask)) //Class check
                    continue;
                if (zone_count > 0 && !zones.Contains(p.Zone)) //Zone check
                    continue;

                if(ustr_count > 0) //User defined filters
                {
                    bool skip = true;
                    foreach (string str in ustr)
                    {
                        if (p.Name.ToLower().Contains(str))
                        {
                            skip = false;
                            break;
                        }
                    }

                    if (skip) continue;
                }
                
                displaycount++;
                players.Add(p);

                //TODO GUILD STUFF
            }

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_WHO);
            pkt.WriteUInt32((displaycount > 49 ? 49 : displaycount));
            pkt.WriteUInt32(displaycount);
            foreach(Player p in players)
            {
                pkt.WriteString(p.Name);
                pkt.WriteString(string.Empty);
                pkt.WriteUInt32(p.Level);
                pkt.WriteUInt32(p.Class);
                pkt.WriteUInt32(p.Race);
                pkt.WriteUInt32(p.Zone);                
                pkt.WriteUInt32((uint)p.GroupStatus);
            }

            manager.Send(pkt);
        }
    }
}
