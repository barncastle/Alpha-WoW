using Common.Network.Packets;
using WorldServer.Network;
using WorldServer.Game.Commands;
using Common.Database.DBC;
using WorldServer.Game.Managers;
using WorldServer.Storage;
using WorldServer.Game;
using Common.Constants;
using WorldServer.Game.Objects;

namespace WorldServer.Packets.Handlers
{
    public class ChatHandler
    {
        public static void HandleMessageChat(ref PacketReader packet, ref WorldManager manager)
        {
            uint type = packet.ReadUInt32();
            uint lang = packet.ReadUInt32();
            Player target = null;
            string msg = string.Empty;

            switch ((ChatMsgs)type)
            {
                case ChatMsgs.CHAT_MSG_SAY:
                case ChatMsgs.CHAT_MSG_EMOTE:
                case ChatMsgs.CHAT_MSG_YELL:
                    msg = packet.ReadString();
                    if (ConsoleManager.InvokeHandler(msg, manager.Character) || string.IsNullOrEmpty(msg))
                        return;

                    ChatManager.Instance.SendChatMessage(manager.Character, (ChatMsgs)type, msg, lang, 0);
                    return;
                case ChatMsgs.CHAT_MSG_WHISPER:
                    target = Database.Players.TryGetName(packet.ReadString());
                    msg = packet.ReadString();

                    if (target != null && !string.IsNullOrEmpty(msg))
                        ChatManager.Instance.SendWhisper(manager.Character, target, msg);

                    return;
                case ChatMsgs.CHAT_MSG_PARTY:
                    msg = packet.ReadString();
                    if (ConsoleManager.InvokeHandler(msg, manager.Character) || string.IsNullOrEmpty(msg))
                        return;

                    ChatManager.Instance.SendPartyMessage(manager.Character, msg);
                    break;
                case ChatMsgs.CHAT_MSG_OFFICER:
                case ChatMsgs.CHAT_MSG_CHANNEL:
                    msg = packet.ReadString();
                    if (ConsoleManager.InvokeHandler(msg, manager.Character) || string.IsNullOrEmpty(msg))
                        return;

                    ChatManager.Instance.SendGuildMessage(manager.Character, msg, (ChatMsgs)type == ChatMsgs.CHAT_MSG_OFFICER);
                    break;
                case ChatMsgs.CHAT_MSG_DND:
                    msg = packet.ReadString();

                    if (!manager.Character.InCombat)
                    {
                        if (manager.Character.ChatFlag == ChatFlags.CHAT_TAG_DND)
                            manager.Character.ToggleChatFlag(ChatFlags.CHAT_TAG_DND);
                        else
                        {
                            manager.Character.ChatAutoResponse = string.IsNullOrEmpty(msg) ? "Do not Disturb" : msg;
                            manager.Character.ToggleChatFlag(ChatFlags.CHAT_TAG_DND);
                        }

                        GridManager.Instance.SendSurrounding(manager.Character.BuildUpdate(), manager.Character);
                    }

                    break;
                case ChatMsgs.CHAT_MSG_AFK:
                    msg = packet.ReadString();

                    if (!manager.Character.InCombat)
                    {
                        if (manager.Character.ChatFlag == ChatFlags.CHAT_TAG_AFK)
                            manager.Character.ToggleChatFlag(ChatFlags.CHAT_TAG_AFK);
                        else
                        {
                            manager.Character.ChatAutoResponse = string.IsNullOrEmpty(msg) ? "Away from keyboard" : msg;
                            manager.Character.ToggleChatFlag(ChatFlags.CHAT_TAG_AFK);
                        }

                        GridManager.Instance.SendSurrounding(manager.Character.BuildUpdate(UpdateTypes.UPDATE_FULL), manager.Character);
                    }
                    break;
            }
        }

        public static void HandleTextEmoteOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            if (manager.Character.IsDead)
                return;

            uint textemote = packet.ReadUInt32();
            ulong guid = packet.ReadUInt64();
            Unit target = Database.Creatures.TryGet<Unit>(guid) ?? Database.Players.TryGet<Unit>(guid);

            if (!DBC.EmotesText.ContainsKey((int)textemote))
                return;

            uint emote_id = (uint)DBC.EmotesText[(int)textemote].m_emoteID;

            //Send text
            PacketWriter response = new PacketWriter(Opcodes.SMSG_TEXT_EMOTE);
            response.WriteUInt64(manager.Character.Guid);
            response.WriteUInt32(textemote);

            if (target == null)
                response.WriteUInt8(0);
            else if (target.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                response.WriteString(((Player)target).Name);
            else if (target.IsTypeOf(ObjectTypes.TYPE_UNIT))
                response.WriteString(((Creature)target).Template.Name);
            else
                response.WriteUInt8(0);

            GridManager.Instance.SendSurroundingInRange(response, manager.Character, Globals.EMOTE_RANGE);

            //Do emote
            StandState state = StandState.UNIT_STANDING;
            switch ((Emotes)textemote)
            {
                case Emotes.SIT:
                    if (!manager.Character.IsSitting) state = StandState.UNIT_SITTING;
                    manager.Character.SetStandState((byte)state);
                    return;
                case Emotes.STAND:
                    manager.Character.SetStandState((byte)state);
                    return;
                case Emotes.SLEEP:
                    if ((StandState)manager.Character.StandState != StandState.UNIT_SLEEPING)
                        state = StandState.UNIT_SLEEPING;
                    manager.Character.SetStandState((byte)state);
                    return;
                case Emotes.KNEEL:
                    if ((StandState)manager.Character.StandState != StandState.UNIT_KNEEL)
                        state = StandState.UNIT_KNEEL;
                    manager.Character.SetStandState((byte)state);
                    return;
                default:
                    manager.Character.PlayEmote(emote_id);
                    return;
            }
        }

        public static void HandleStandStateChangeOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            manager.Character.SetStandState((byte)packet.ReadUInt32());
        }
    }
}
