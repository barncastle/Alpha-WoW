using Common.Constants;
using Common.Helpers;
using Common.Network.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Game.Objects
{
    public class Group
    {
        public const uint MAX_GROUP_SIZE = 5;
        public Player Leader { get; set; }
        public ConcurrentDictionary<ulong, Player> Members = new ConcurrentDictionary<ulong, Player>();
        public ConcurrentDictionary<ulong, Player> Invites = new ConcurrentDictionary<ulong, Player>();
        public bool IsFull { get { return Members.Count >= MAX_GROUP_SIZE; } }
        public LootMethods LootMethod = LootMethods.LOOT_METHOD_FREEFORALL;
        public ulong LootMaster = 0;

        public Group(Player leader)
        {
            this.Leader = leader;
            Flag.SetFlag(ref leader.PlayerFlags,(byte)PlayerFlags.PLAYER_FLAGS_GROUP_LEADER);

            this.LootMaster = leader.Guid;
            TryAddMember(leader, false);
        }

        public void SetLeader(Player p)
        {
            this.Leader = p;
            Flag.SetFlag(ref p.PlayerFlags, (byte)PlayerFlags.PLAYER_FLAGS_GROUP_LEADER);

            foreach (Player mem in this.Members.Values)
                if (this.Leader != mem)
                    Flag.RemoveFlag(ref mem.PlayerFlags, (byte)PlayerFlags.PLAYER_FLAGS_GROUP_LEADER);

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_GROUP_SET_LEADER);
            pkt.WriteString(p.Name);
            SendPacket(pkt);
            this.SendUpdate();
        }

        public void SendUpdate()
        {
            foreach (Player p in Members.Values)
            {
                PacketWriter pkt = new PacketWriter(Opcodes.SMSG_GROUP_LIST);
                pkt.WriteUInt32((byte)(Members.Count));

                pkt.WriteString(Leader.Name);
                pkt.WriteUInt64(Leader.Guid);
                pkt.WriteUInt8(1); //??

                foreach (Player mem in Members.Values)
                {
                    if (mem == Leader) continue;

                    pkt.WriteString(mem.Name);
                    pkt.WriteUInt64(mem.Guid);
                    pkt.WriteUInt8(0); //??
                }

                pkt.WriteUInt8((byte)LootMethod);
                pkt.WriteUInt64(LootMaster);

                p.Client.Send(pkt);
            }
        }

        public void Kick(Player p)
        {
            if (TryRemoveMember(p))
            {
                p.Group = null;
                p.GroupInvited = false;
                p.Client.Send(new PacketWriter(Opcodes.SMSG_GROUP_UNINVITE));
                if (Members.Count == 1)
                    Disband(true);
            }
        }

        public void Disband(bool hideDestroy)
        {
            foreach (Player p in Members.Values)
            {
                if (!hideDestroy)
                    p.Client.Send(new PacketWriter(Opcodes.CMSG_GROUP_DISBAND));

                p.Client.Send(new PacketWriter(Opcodes.SMSG_GROUP_DESTROYED));

                p.GroupInvited = false;
                p.Group = null;
            }

            foreach (Player p in Invites.Values)
            {
                p.GroupInvited = false;
                p.Group = null;
            }

            Members.Clear();
            Invites.Clear();
            Leader = null;
        }

        public bool TryAddMember(Player p, bool invite)
        {
            if (p.Group != null || IsFull)
                return false;

            if (invite)
            {
                if (Invites.TryAdd(p.Guid, p))
                {
                    p.GroupInvited = true;
                    p.Group = this;
                    return true;
                }
            }
            else
            {
                if (Members.TryAdd(p.Guid, p))
                {
                    p.GroupInvited = true;
                    p.Group = this;
                    p.GroupStatus = WhoPartyStatuses.WHO_PARTY_STATUS_IN_PARTY;
                    SendPacket(p.QueryDetails(), p);

                    if (p != Leader)
                        Flag.RemoveFlag(ref p.PlayerFlags,(byte)PlayerFlags.PLAYER_FLAGS_GROUP_LEADER);

                    if (Members.Count > 1)
                    {
                        this.SendUpdate();
                        this.SendPartyMemberStats(p);
                    }

                    return true;
                }
            }

            return false;
        }

        public bool TryRemoveMember(Player p)
        {
            bool isleader = (p == Leader);
            Player dump;

            if (Members.TryRemove(p.Guid, out dump))
            {
                p.Group = null;
                p.GroupInvited = false;
                p.GroupStatus = WhoPartyStatuses.WHO_PARTY_STATUS_NOT_IN_PARTY;

                if (Members.Count == 1)
                {
                    this.Disband(false);
                    return true;
                }

                if (isleader)
                    SetLeader(Members.Values.First());

                SendUpdate();
                return true;
            }

            return false;
        }

        public bool TryUninvite(Player p)
        {
            Player dump;
            if (Invites.ContainsKey(p.Guid))
            {
                if (Invites.TryRemove(p.Guid, out dump))
                {
                    p.Group = null;
                    p.GroupInvited = false;
                    p.GroupStatus = WhoPartyStatuses.WHO_PARTY_STATUS_NOT_IN_PARTY;

                    if (Members.Count <= 1)
                        this.Disband(true);
                }
            }


            return false;
        }

        public void SendPacket(PacketWriter packet, Player ignore = null)
        {
            foreach (Player p in Members.Values)
            {
                if (p == ignore)
                    continue;

                p.Client.Send(packet);
            }
        }

        public void SendPartyMemberStats(Player p)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_PARTY_MEMBER_STATS);
            pkt.WriteUInt64(p.Guid);
            pkt.WriteUInt32(p.Health.Current);
            pkt.WriteUInt32(p.Health.Maximum);
            pkt.WriteUInt8(p.PowerType);
            pkt.WriteUInt32(p.GetPowerValue(false));
            pkt.WriteUInt32(p.GetPowerValue(true));
            pkt.WriteUInt32(p.Level);

            pkt.WriteUInt32(p.Zone); //?
            pkt.WriteUInt32(p.Map); //?
            pkt.WriteUInt32(p.Class);
            //pkt.WriteVector(p.Location); //?
            //pkt.WriteUInt32((uint)(p.LoggedIn && p.IsOnline ? 1 : 0));

            SendPacket(pkt, p);
        }

        public void SendAllPartyStatus()
        {
            foreach (Player mem in this.Members.Values)
                SendPartyMemberStats(mem);
        }

        public HashSet<Player> GetGroupInRange(WorldObject source, float range)
        {
            HashSet<Player> players = new HashSet<Player>();
            foreach (Player p in Members.Values)
                if (source.Location.Distance(p.Location) <= range && !p.IsDead)
                    players.Add(p);

            return players;
        }
    }
}
