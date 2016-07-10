using Common.Constants;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Storage;

namespace WorldServer.Game.Objects.PlayerExtensions.FriendExtension
{
    public static class FriendExtension
    {
        public static void SendFriendStatus(this Player p, Player friend, FriendResults result, bool broadcast)
        {
            FriendInfo fi = BuildFriendInfo(friend);

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_FRIEND_STATUS);
            pkt.WriteUInt8((byte)result);
            pkt.WriteUInt64(friend.Guid);
            pkt.WriteUInt8((byte)fi.Status);
            if (fi.Status > 0)
            {
                pkt.WriteUInt32(fi.Area);
                pkt.WriteUInt32(fi.Level);
                pkt.WriteUInt32(fi.Class);
            }

            if (broadcast)
            {
                foreach (Player player in Database.Players.Values)
                    if (player.FriendList.Contains(friend.Guid))
                        player.Client.Send(pkt);
            }
            else
                p.Client.Send(pkt);
        }

        public static void SendFriendList(this Player p)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_FRIEND_LIST);
            pkt.WriteUInt8((byte)p.FriendList.Count);
            foreach (ulong guid in p.FriendList)
            {
                Player friend = Database.Players.TryGet(guid);

                if (!p.QueriedObjects.Contains(guid))
                    p.Client.Send(friend.QueryDetails());

                FriendInfo fi = BuildFriendInfo(friend);
                pkt.WriteUInt64(friend.Guid);
                pkt.WriteUInt8((byte)fi.Status);
                pkt.WriteUInt32(fi.Area);
                pkt.WriteUInt32(fi.Level);
                pkt.WriteUInt32(fi.Class);

            }

            p.Client.Send(pkt);
        }

        public static void SendIgnoreList(this Player p)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_IGNORE_LIST);
            pkt.WriteUInt8((byte)p.IgnoreList.Count);
            foreach (ulong guid in p.IgnoreList)
                pkt.WriteUInt64(guid);

            p.Client.Send(pkt);
        }

        private static FriendInfo BuildFriendInfo(Player friend)
        {
            FriendInfo fi = new FriendInfo();

            fi.Status = FriendStatuses.FRIEND_STATUS_ONLINE;
            if (friend.ChatFlag == ChatFlags.CHAT_TAG_AFK)
                fi.Status = FriendStatuses.FRIEND_STATUS_AFK;
            if (friend.ChatFlag == ChatFlags.CHAT_TAG_DND)
                fi.Status = FriendStatuses.FRIEND_STATUS_AFK;
            if (!(friend.LoggedIn && friend.IsOnline))
                fi.Status = FriendStatuses.FRIEND_STATUS_OFFLINE;

            fi.Class = friend.Class;
            fi.Area = friend.Zone;
            fi.Level = friend.Level;

            return fi;
        }

        public struct FriendInfo
        {
            public FriendStatuses Status;
            public uint Flags;
            public uint Area;
            public uint Level;
            public uint Class;
        }
    }
}
