using Common.Constants;
using Common.Network.Packets;
using WorldServer.Game.Objects;
using WorldServer.Network;
using WorldServer.Storage;
using WorldServer.Game.Objects.PlayerExtensions.FriendExtension;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Game;

namespace WorldServer.Packets.Handlers
{
    public class FriendHandler
    {
        public static void HandleFriendListOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            manager.Character.SendFriendList();
            manager.Character.SendIgnoreList();
        }

        public static void HandleAddFriendOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            string name = packet.ReadString();
            Player target = Database.Players.TryGetName(name);
            FriendResults result = FriendResults.FRIEND_OFFLINE;
            if (target != null)
            {
                if (target == manager.Character)
                    result = FriendResults.FRIEND_SELF;
                else if (manager.Character.IsEnemyTo(target))
                    result = FriendResults.FRIEND_ENEMY;
                else if (manager.Character.FriendList.Contains(target.Guid))
                    result = FriendResults.FRIEND_ALREADY;
                else
                {
                    if (target.IsOnline && target.LoggedIn)
                        result = FriendResults.FRIEND_ADDED_ONLINE;
                    else
                        result = FriendResults.FRIEND_ADDED_OFFLINE;

                    if (manager.Character.FriendList.Count >= Globals.MAX_FRIEND_LIST)
                        result = FriendResults.FRIEND_LIST_FULL;
                    else
                        manager.Character.FriendList.Add(target.Guid);
                }

                manager.Character.SendFriendStatus(target, result, false);
            }

            manager.Character.SendFriendList();
        }

        public static void HandleDelFriendOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            if (manager.Character.FriendList.Contains(guid))
            {
                Player friend = Database.Players.TryGet(guid);
                manager.Character.SendFriendStatus(friend, FriendResults.FRIEND_REMOVED, false);
                manager.Character.FriendList.Remove(guid);
            }
               
            manager.Character.SendFriendList();
        }

        public static void HandleAddIgnoreOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            string name = packet.ReadString();
            Player target = Database.Players.TryGetName(name);
            FriendResults result = FriendResults.FRIEND_IGNORE_NOT_FOUND;

            if (target != null)
            {
                if (target == manager.Character)
                    result = FriendResults.FRIEND_IGNORE_SELF;
                else if (manager.Character.IgnoreList.Contains(target.Guid))
                    result = FriendResults.FRIEND_IGNORE_ALREADY;
                else
                {
                    result = FriendResults.FRIEND_IGNORE_ADDED;

                    if (manager.Character.IgnoreList.Count >= Globals.MAX_IGNORE_LIST)
                        result = FriendResults.FRIEND_IGNORE_FULL;
                    else
                        manager.Character.IgnoreList.Add(target.Guid);
                }

                manager.Character.SendFriendStatus(target, result, false);
            }

            manager.Character.SendIgnoreList();
        }

        public static void HandleDelIgnoreOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            if (manager.Character.IgnoreList.Contains(guid))
            {
                Player ignore = Database.Players.TryGet(guid);
                manager.Character.SendFriendStatus(ignore, FriendResults.FRIEND_IGNORE_REMOVED, false);
                manager.Character.IgnoreList.Remove(guid);
            }

            manager.Character.SendIgnoreList();
        }
    }
}
