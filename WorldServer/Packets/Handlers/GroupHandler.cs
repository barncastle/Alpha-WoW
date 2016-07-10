using Common.Constants;
using Common.Network.Packets;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Network;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class GroupHandler
    {
        public static void HandleGroupInviteOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            string playername = packet.ReadString();
            Player player = Database.Players.TryGetName(playername);
            Group myGroup = manager.Character.Group;
            Group theirGroup = player?.Group;

            if (player == null)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_INVITE, playername, PartyResults.ERR_BAD_PLAYER_NAME_S);
                return;
            }

            if (player.IsEnemyTo(manager.Character))
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_INVITE, playername, PartyResults.ERR_PLAYER_WRONG_FACTION);
                return;
            }

            if (theirGroup != null)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_INVITE, playername, PartyResults.ERR_ALREADY_IN_GROUP_S);
                return;
            }

            if (myGroup != null)
            {
                if (myGroup.Leader != manager.Character)
                {
                    SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_INVITE, "", PartyResults.ERR_NOT_LEADER);
                    return;
                }

                if (myGroup.IsFull)
                {
                    SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_INVITE, "", PartyResults.ERR_GROUP_FULL);
                    return;
                }

                if (!myGroup.TryAddMember(player, true))
                    return;
            }
            else
            {
                Group newGroup = new Group(manager.Character);
                if (!newGroup.TryAddMember(player, true))
                    return;
            }

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_GROUP_INVITE);
            pkt.WriteString(manager.Character.Name);
            player.Client.Send(pkt);

            SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_INVITE, playername, PartyResults.ERR_PARTY_RESULT_OK);
        }

        public static void HandleGroupDeclineOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            Group group = manager.Character.Group;
            Player leader = group.Leader;
            if (group == null)
                return;

            group.TryUninvite(manager.Character);

            if (group.Leader == manager.Character)
                return;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_GROUP_DECLINE);
            pkt.WriteString(manager.Character.Name);
            leader.Client.Send(pkt);
        }

        public static void HandleGroupAcceptOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            Group group = manager.Character.Group;
            if (group == null)
                return;

            Player dump;
            if (group.Invites.TryRemove(manager.Character.Guid, out dump))
            {
                manager.Character.Group = null;
                group.TryAddMember(manager.Character, false);
            }

        }

        public static void HandleGroupUninviteGuidOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            Player target = Database.Players.TryGet(guid);
            Group group = manager.Character.Group;

            if (target == null)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_TARGET_NOT_IN_GROUP_S);
                return;
            }

            if (group == null)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_NOT_IN_GROUP);
                return;
            }

            if (group.Leader != manager.Character)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_NOT_LEADER);
                return;
            }

            if (!group.Members.ContainsKey(guid))
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_TARGET_NOT_IN_GROUP_S);
                return;
            }

            if (group.Members.ContainsKey(guid))
                group.Kick(target);
            else if (group.Invites.ContainsKey(guid))
                group.TryUninvite(target);
        }

        public static void HandleGroupUninviteOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            string playername = packet.ReadString();
            Player target = Database.Players.TryGetName(playername);
            Group group = manager.Character.Group;

            if (target == null)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_TARGET_NOT_IN_GROUP_S);
                return;
            }

            if (group == null)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_NOT_IN_GROUP);
                return;
            }

            if (group.Leader != manager.Character)
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_NOT_LEADER);
                return;
            }

            if (!group.Members.ContainsKey(target.Guid))
            {
                SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, "", PartyResults.ERR_TARGET_NOT_IN_GROUP_S);
                return;
            }

            if (group.Members.ContainsKey(target.Guid))
                group.Kick(target);
            else if (group.Invites.ContainsKey(target.Guid))
                group.TryUninvite(target);

        }

        public static void HandleGroupDisbandOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            if (manager.Character.Group == null)
                return;

            SendPartyStatus(manager.Character, PartyOperations.PARTY_OP_LEAVE, manager.Character.Name, PartyResults.ERR_PARTY_RESULT_OK);
            manager.Character.Group.TryRemoveMember(manager.Character);
        }

        public static void HandleMinimapPingOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            float x = packet.ReadFloat();
            float y = packet.ReadFloat();

            if (manager.Character.Group == null)
                return;

            PacketWriter pkt = new PacketWriter(Opcodes.MSG_MINIMAP_PING);
            pkt.WriteUInt64(manager.Character.Guid);
            pkt.WriteFloat(x);
            pkt.WriteFloat(y);
            manager.Character.Group.SendPacket(pkt, manager.Character);
        }

        public static void HandleGroupSetLeaderOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            string playername = packet.ReadString();
            Player target = Database.Players.TryGetName(playername);
            Group group = manager.Character.Group;

            if (group == null || target == null)
                return;
            if (group.Leader == target || target.Group != group)
                return;

            group.SetLeader(target);
        }

        public static void HandleLootMethodOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            byte lootmethod = (byte)packet.ReadUInt32();
            ulong guid = packet.ReadUInt64();

            Group group = manager.Character.Group;
            if (group == null)
                return;

            if (group.Leader != manager.Character)
                return;

            if (guid > 0)
                group.LootMaster = guid;

            group.LootMethod = (LootMethods)lootmethod;
            group.SendUpdate();
        }

        private static void SendPartyStatus(Player p, PartyOperations operation, string member, PartyResults res)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_PARTY_COMMAND_RESULT);
            pkt.WriteUInt32((uint)operation);
            pkt.WriteString(member);
            pkt.WriteUInt32((uint)res);
            p.Client.Send(pkt);
        }

        public static void HandleLookingForGroupOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            bool looking = packet.ReadUInt32() == 1;
            //If not in a group
            if (manager.Character.GroupStatus != WhoPartyStatuses.WHO_PARTY_STATUS_IN_PARTY)
                manager.Character.GroupStatus = (looking ? WhoPartyStatuses.WHO_PARTY_STATUS_LFG : WhoPartyStatuses.WHO_PARTY_STATUS_NOT_IN_PARTY);
        }
    }
}
