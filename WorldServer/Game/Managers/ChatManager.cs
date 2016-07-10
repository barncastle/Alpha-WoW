using Common.Constants;
using Common.Network.Packets;
using Common.Singleton;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldServer.Game.Objects;
using WorldServer.Storage;

namespace WorldServer.Game.Managers
{
    public class ChatManager : SingletonBase<ChatManager>
    {
        public Dictionary<uint, ushort[]> LANG_SKILL_LOOKUP =
        new Dictionary<uint, ushort[]>()
        {
            { 1, new ushort[] { 109, 110 }}, //Orcish
            { 2, new ushort[] { 113, 114 }}, //Darnassian
            { 3, new ushort[] { 115, 116 }}, //Taurahe
            { 6, new ushort[] { 111, 112 }}, //Dwarvish
            { 7, new ushort[] { 98, 99 }}, //Common
            { 13, new ushort[] { 313, 314 }}, //Gnomish
            { 14, new ushort[] { 315, 316 }}, //Troll
        };

        public uint GetLanguage(uint lang, Player player)
        {
            if (!LANG_SKILL_LOOKUP.ContainsKey(lang))
                return lang;

            foreach (ushort skillid in LANG_SKILL_LOOKUP[lang])
                if (player.Skills.ContainsKey(skillid))
                    return 0;

            return lang;
        }

        public void SendSystemMessage(Player target, string message)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_MESSAGECHAT);
            pkt.WriteUInt8((byte)ChatMsgs.CHAT_MSG_SYSTEM);
            pkt.WriteUInt32(0);
            pkt.WriteUInt64(target.Guid);
            pkt.WriteString(message);
            pkt.WriteUInt8((byte)ChatFlags.CHAT_TAG_NONE);
            target.Client.Send(pkt);
        }

        public void SendNotification(Player p, string message)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_NOTIFICATION);
            pkt.WriteString(message);
            p.Client.Send(pkt);
        }

        public void SendWhisper(Unit sender, Player target, string message)
        {
            if(target.IgnoreList.Contains(sender.Guid))
            {
                PacketWriter pkt = new PacketWriter(Opcodes.SMSG_MESSAGECHAT);
                pkt.WriteUInt8((byte)ChatMsgs.CHAT_MSG_WHISPER);
                pkt.WriteUInt32(0);
                pkt.WriteUInt64(sender.Guid);
                pkt.WriteString(message);
                pkt.WriteUInt8((byte)(sender.IsTypeOf(ObjectTypes.TYPE_PLAYER) ? ((Player)sender).ChatFlag : ChatFlags.CHAT_TAG_NONE));
                target.Client.Send(pkt);
            }

            if (sender.IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                Player player = (Player)sender;
                PacketWriter pkt = new PacketWriter(Opcodes.SMSG_MESSAGECHAT);
                pkt.WriteUInt8((byte)ChatMsgs.CHAT_MSG_WHISPER_INFORM);
                pkt.WriteUInt32(0);
                pkt.WriteUInt64(target.Guid);
                pkt.WriteString(message);
                pkt.WriteUInt8((byte)target.ChatFlag);
                player.Client.Send(pkt);

                if (target.ChatFlag == ChatFlags.CHAT_TAG_AFK)
                    SendSystemMessage(player, target.Name + " is Away from Keyboard: " + target.ChatAutoResponse);
                else if (target.ChatFlag == ChatFlags.CHAT_TAG_DND)
                    SendSystemMessage(player, target.Name + " does not wish to be disturbed: " + target.ChatAutoResponse);
            }
        }

        public void SendChatMessage(WorldObject sender, ChatMsgs msgtype, string message, uint lang, ulong target)
        {
            float range = 0;
            switch (msgtype)
            {
                case ChatMsgs.CHAT_MSG_YELL:
                case ChatMsgs.CHAT_MSG_MONSTER_YELL:
                    range = Globals.YELL_RANGE;
                    break;
                case ChatMsgs.CHAT_MSG_MONSTER_SAY:
                case ChatMsgs.CHAT_MSG_SAY:
                    range = Globals.SAY_RANGE;
                    break;
            }

            Parallel.ForEach(GridManager.Instance.GetSurroundingObjects(sender, true).Cast<Player>(), p =>
            {
                if (range > 0 && p.Location.Distance(sender.Location) > range)
                    return;
                if (p.IgnoreList.Contains(sender.Guid))
                    return;

                p.Client.Send(BuildPacket(sender, msgtype, message, lang, target, p));
            });
        }

        public void SendPartyMessage(Player sender, string message)
        {
            if (sender.Group != null)
            {
                foreach (Player p in sender.Group.Members.Values)
                {
                    if (p.IgnoreList.Contains(sender.Guid))
                        continue;

                    p.Client.Send(BuildPacket(sender, ChatMsgs.CHAT_MSG_PARTY, message, 0, 0, p));
                }
                    
            }
            else
                sender.Client.Send(BuildPacket(sender, ChatMsgs.CHAT_MSG_PARTY, message, 0, 0, sender));
        }

        public void SendGuildMessage(Player sender, string message, bool officer = false)
        {
            //TODO Guild message
        }

        public PacketWriter BuildPacket(WorldObject sender, ChatMsgs msgtype, string message, uint lang, ulong target, Player recipient)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_MESSAGECHAT);
            pkt.WriteUInt8((byte)msgtype);
            pkt.WriteUInt32(GetLanguage(lang, recipient));
            pkt.WriteUInt64(sender.Guid);
            pkt.WriteString(message);
            pkt.WriteUInt8((byte)(sender.IsTypeOf(ObjectTypes.TYPE_PLAYER) ? ((Player)sender).ChatFlag : ChatFlags.CHAT_TAG_NONE));
            return pkt;
        }


    }
}
