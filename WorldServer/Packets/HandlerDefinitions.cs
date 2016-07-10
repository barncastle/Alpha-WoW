using WorldServer.Packets.Handlers;

namespace WorldServer.Packets
{
    public class HandlerDefinitions
    {
        public static void InitializePacketHandler()
        {
            //Login related opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AUTH_SESSION, AuthHandler.HandleAuthSession);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_ENUM, CharHandler.HandleCharEnum);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_CREATE, CharHandler.HandleCharCreate);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_DELETE, CharHandler.HandleCharDelete);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_PING, NetHandler.HandlePing);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_PLAYER_LOGIN, WorldHandler.HandlePlayerLogin);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_LOGOUT_REQUEST, WorldHandler.HandleLogoutRequest);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_ZONEUPDATE, MovementHandler.HandleZoneUpdate);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_NAME_QUERY, CharHandler.HandleNameCache);

            //Item opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_ITEM_QUERY_SINGLE, ItemHandler.HandleItemQuerySingle);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_DESTROYITEM, ItemHandler.HandleDestroyItem);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SWAP_ITEM, ItemHandler.HandleSwapItem);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SWAP_INV_ITEM, ItemHandler.HandleSwapInventoryItem);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AUTOEQUIP_ITEM, ItemHandler.HandleAutoEquipItem);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AUTOSTORE_BAG_ITEM, ItemHandler.HandleAutostoreBagItem);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SPLIT_ITEM, ItemHandler.HandleSplitItemOpcode);

            //NPC opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SELL_ITEM, NPCHandler.HandleSellItemOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_BUY_ITEM, NPCHandler.HandleBuyItemOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_BUY_ITEM_IN_SLOT, NPCHandler.HandleBuyItemInSlotOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_LIST_INVENTORY, NPCHandler.HandleListInventoryOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_TRAINER_BUY_SPELL, NPCHandler.HandleTrainerBuySpellOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_TRAINER_LIST, NPCHandler.HandleTrainerList);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_BANKER_ACTIVATE, NPCHandler.HandleBankerActivateOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_BUY_BANK_SLOT, NPCHandler.HandleBuyBankSlotOpcode);

            //GameObject opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GAMEOBJECT_QUERY, GameObjectHandler.HandleGameObjectQueryOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GAMEOBJ_USE, GameObjectHandler.HandleGameObjectUseOpcode);

            //Chat opcodes            
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_MESSAGECHAT, ChatHandler.HandleMessageChat);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_TEXT_EMOTE, ChatHandler.HandleTextEmoteOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_STANDSTATECHANGE, ChatHandler.HandleStandStateChangeOpcode);

            //Movement opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_WORLD_TELEPORT, WorldHandler.HandleWorldTeleport);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_WORLDPORT_ACK, WorldHandler.HandleWorldPortAck);
            PacketManager.DefineOpcodeHandler(Opcodes.SMSG_MOVE_WORLDPORT_ACK, WorldHandler.HandleWorldPortAck);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_FORWARD, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_BACKWARD, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_STRAFE_LEFT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_STRAFE_RIGHT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_STRAFE, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_JUMP, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_TURN_LEFT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_TURN_RIGHT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_TURN, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_PITCH_UP, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_PITCH_DOWN, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_PITCH, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_RUN_MODE, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_WALK_MODE, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_SWIM, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_SWIM, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_FACING, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_PITCH, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_ROOT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_UNROOT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_HEARTBEAT, MovementHandler.HandleMovementStatus);

            //Trade Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_INITIATE_TRADE, TradeHandler.HandleInitiateTradeOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_BEGIN_TRADE, TradeHandler.HandleBeginTradeOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CANCEL_TRADE, TradeHandler.HandleCancelTradeOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SET_TRADE_ITEM, TradeHandler.HandleSetTradeItemOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SET_TRADE_GOLD, TradeHandler.HandleSetTradeGoldOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CLEAR_TRADE_ITEM, TradeHandler.HandleClearTradeItemOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_UNACCEPT_TRADE, TradeHandler.HandleUnacceptTradeOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_ACCEPT_TRADE, TradeHandler.HandleAcceptTradeOpcode);

            //Group Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GROUP_INVITE, GroupHandler.HandleGroupInviteOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GROUP_DECLINE, GroupHandler.HandleGroupDeclineOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GROUP_ACCEPT, GroupHandler.HandleGroupAcceptOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GROUP_UNINVITE_GUID, GroupHandler.HandleGroupUninviteGuidOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GROUP_UNINVITE, GroupHandler.HandleGroupUninviteOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GROUP_DISBAND, GroupHandler.HandleGroupDisbandOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MINIMAP_PING, GroupHandler.HandleMinimapPingOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_GROUP_SET_LEADER, GroupHandler.HandleGroupSetLeaderOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_LOOT_METHOD, GroupHandler.HandleLootMethodOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SET_LOOKING_FOR_GROUP, GroupHandler.HandleLookingForGroupOpcode);

            //Character Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SETWEAPONMODE, CharHandler.HandleWeaponSheathe);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SET_TARGET, CharHandler.HandleSetTarget);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SET_SELECTION, CharHandler.HandleSetSelection);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_ATTACKSWING, CharHandler.HandleAttackSwing);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_ATTACKSTOP, CharHandler.HandleAttackStop);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_REPOP_REQUEST, CharHandler.HandleRepopRequest);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SET_ACTION_BUTTON, CharHandler.HandleSetActionButtonOpcode);

            //Friend Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_ADD_FRIEND, FriendHandler.HandleAddFriendOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_FRIEND_LIST, FriendHandler.HandleFriendListOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_DEL_FRIEND, FriendHandler.HandleDelFriendOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_ADD_IGNORE, FriendHandler.HandleAddIgnoreOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_DEL_IGNORE, FriendHandler.HandleDelIgnoreOpcode);

            //Creature Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CREATURE_QUERY, CreatureHandler.HandleCreatureQuery);

            //Loot Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_LOOT, LootHandler.HandleLoot);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_LOOT_RELEASE, LootHandler.HandleLootRelease);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_LOOT_MONEY, LootHandler.HandleLootMoney);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AUTOSTORE_LOOT_ITEM, LootHandler.HandleAutoStoreLootItem);

            //Misc Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUERY_TIME, MiscHandler.HandleQueryTime);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AREATRIGGER, MiscHandler.HandleAreaTriggerOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_PLAYED_TIME, MiscHandler.HandlePlayedTime);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_INSPECT, MiscHandler.HandleInspectOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_WHO, MiscHandler.HandleWhoOpcode);

            //Quest Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUESTGIVER_STATUS_QUERY, QuestHandler.HandleQuestGiverStatusQuery);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUESTGIVER_HELLO, QuestHandler.HandleQuestgiverHelloOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUESTGIVER_ACCEPT_QUEST, QuestHandler.HandleQuestgiverAcceptQuestOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUEST_QUERY, QuestHandler.HandleQuestQueryOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUESTGIVER_COMPLETE_QUEST, QuestHandler.HandleQuestgiverCompleteQuest);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUESTGIVER_CHOOSE_REWARD, QuestHandler.HandleQuestgiverChooseRewardOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUESTGIVER_REQUEST_REWARD, QuestHandler.HandleQuestgiverRequestRewardOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_QUESTLOG_REMOVE_QUEST, QuestHandler.HandleQuestLogRemoveQuest);

            //Spell Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CAST_SPELL, SpellHandler.HandleCastSpellOpcode);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CANCEL_CAST, SpellHandler.HandleCancelCastOpcode);

            //Ignored Opcodes
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_LOOKING_FOR_GROUP, WorldHandler.HandleNULL);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_COLLIDE_REDIRECT, WorldHandler.HandleNULL); //Fall damage wasn't introduced yet
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_COLLIDE_STUCK, WorldHandler.HandleNULL); //Think this was a dev debug opcode for tracking getting stuck
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_SCREENSHOT, WorldHandler.HandleNULL);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_DEBUG_AISTATE, WorldHandler.HandleNULL);
        }
    }
}
