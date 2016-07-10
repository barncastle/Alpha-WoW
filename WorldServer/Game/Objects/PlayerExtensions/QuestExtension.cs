using Common.Constants;
using Common.Database.DBC;
using Common.Helpers;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Game.Structs;
using WorldServer.Storage;

namespace WorldServer.Game.Objects.PlayerExtensions.Quests
{
    public static class QuestExtension
    {
        private const int MAX_QUEST_LOG = 20;

        public static void BuildQuestUpdate(this Player p, ref UpdateClass uc)
        {
            int i = 0;
            foreach (Quest quest in p.Quests.Values)
            {
                if (quest.Status == QuestStatuses.QUEST_STATUS_COMPLETE && quest.Rewarded)
                    continue;

                QuestTemplate template = Database.QuestTemplates.TryGet(quest.QuestId);

                uc.UpdateValue<uint>(PlayerFields.PLAYER_QUEST_LOG_1_1, quest.QuestId, i * 6);
                uc.UpdateValue<uint>(PlayerFields.PLAYER_QUEST_LOG_1_1, 0, (i * 6) + 1);
                uc.UpdateValue<uint>(PlayerFields.PLAYER_QUEST_LOG_1_1, 0, (i * 6) + 2);
                uc.UpdateValue<uint>(PlayerFields.PLAYER_QUEST_LOG_1_1, quest.GetProgress(), (i * 6) + 3);
                uc.UpdateValue<uint>(PlayerFields.PLAYER_QUEST_LOG_1_1, 0, (i * 6) + 4); //m_questFailureTime
                uc.UpdateValue<uint>(PlayerFields.PLAYER_QUEST_LOG_1_1, 0, (i * 6) + 5); //m_qtyMonsterToKill
                i++;
            }
        }

        public static bool AddQuest(this Player p, uint questid, ulong guid)
        {
            if (p.Quests.ContainsKey(questid) || !Database.QuestTemplates.ContainsKey(questid)) //Doesnt exist
                return false;

            if (p.Quests.Count >= MAX_QUEST_LOG)
                return false;

            QuestTemplate template = Database.QuestTemplates.TryGet(questid);
            //TODO checks, item removal etc

            p.Quests.Add(questid, new Quest(questid, guid));

            if (p.CanCompleteQuest(questid))
                p.Quests[questid].Status = QuestStatuses.QUEST_STATUS_COMPLETE;

            p.Dirty = true;
            p.SendQuestQueryResponse(template);
            return true;
        }

        public static uint GetDialogStatus(this Player p, WorldObject obj, uint defStatus)
        {
            QuestStatuses status = QuestStatuses.QUEST_STATUS_NONE;
            HashSet<QuestTemplate> rbounds = new HashSet<QuestTemplate>();
            HashSet<QuestTemplate> ibounds = new HashSet<QuestTemplate>();

            if (obj.IsTypeOf(ObjectTypes.TYPE_UNIT) && !obj.IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                uint entry = ((Creature)obj).Entry;
                rbounds = new HashSet<QuestTemplate>(Database.CreatureQuests.TryGet(entry).Select(x => Database.QuestTemplates.TryGet(x.QuestEntry)));
                ibounds = new HashSet<QuestTemplate>(Database.CreatureInvolvedQuests.TryGet(entry).Select(x => Database.QuestTemplates.TryGet(x.QuestEntry)));

                if (((Creature)obj).IsEnemyTo(p))
                    return (uint)QuestGiverStatuses.QUEST_GIVER_NONE;
            }
            else if (obj.IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
                rbounds = new HashSet<QuestTemplate>(); //TODO gameobject quests
            else
                return (uint)QuestGiverStatuses.QUEST_GIVER_NONE;

            //Quest Finish
            foreach (QuestTemplate qt in ibounds)
            {
                QuestGiverStatuses dialogStatusNew = QuestGiverStatuses.QUEST_GIVER_NONE;
                status = p.GetQuestStatus(qt.QuestId);

                Quest quest = null;
                if (p.Quests.ContainsKey(qt.QuestId))
                    quest = p.Quests[qt.QuestId];

                if ((status == QuestStatuses.QUEST_STATUS_COMPLETE && quest?.Rewarded != true) || qt.IsAutoComplete)
                    return (uint)QuestGiverStatuses.QUEST_GIVER_REWARD; //Turn in is priority
                else if (status == QuestStatuses.QUEST_STATUS_INCOMPLETE)
                    dialogStatusNew = QuestGiverStatuses.QUEST_GIVER_NONE;

                if ((uint)dialogStatusNew > defStatus)
                    defStatus = (uint)dialogStatusNew;
            }

            //Quest start
            foreach (QuestTemplate qt in rbounds)
            {
                if (qt == null)
                    continue;

                QuestGiverStatuses dialogStatusNew = QuestGiverStatuses.QUEST_GIVER_NONE;
                status = p.GetQuestStatus(qt.QuestId);

                if (status == QuestStatuses.QUEST_STATUS_NONE) //QUEST_STATUS_NONE aka New Quest
                {
                    if (!p.CheckQuestRequirements(qt, false) || !p.CheckQuestLevel(qt, false))
                        continue;

                    if (qt.IsAutoComplete)
                        dialogStatusNew = QuestGiverStatuses.QUEST_GIVER_REWARD; //Autocomplete just show icon
                    else if (p.Level < qt.MinLevel && p.Level >= qt.MinLevel - 4)
                        dialogStatusNew = QuestGiverStatuses.QUEST_GIVER_FUTURE; //Player is in level range of accepting soon (silver icon)
                    else if (p.Level >= qt.MinLevel && p.Level < qt.QuestLevel + 7)
                        dialogStatusNew = QuestGiverStatuses.QUEST_GIVER_QUEST; //Player in level range (yellow icon)
                    else if (p.Level > qt.QuestLevel + 7)
                        dialogStatusNew = QuestGiverStatuses.QUEST_GIVER_TRIVIAL; //Trivial for player (grey icon hovered)
                }

                if ((uint)dialogStatusNew > defStatus)
                    defStatus = (uint)dialogStatusNew;
            }

            return defStatus;
        }

        public static QuestStatuses GetQuestStatus(this Player p, uint questid)
        {
            if (p.Quests.ContainsKey(questid))
                return p.Quests[questid]?.Status ?? QuestStatuses.QUEST_STATUS_NONE;

            return QuestStatuses.QUEST_STATUS_NONE;
        }

        public static void SendPreparedQuest(this Player p, WorldObject obj)
        {
            QuestStatuses status = QuestStatuses.QUEST_STATUS_NONE;
            QuestMenu menu = new QuestMenu();
            HashSet<QuestTemplate> rbounds = new HashSet<QuestTemplate>();
            HashSet<QuestTemplate> ibounds = new HashSet<QuestTemplate>();

            if (obj.IsTypeOf(ObjectTypes.TYPE_UNIT) && !obj.IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                uint entry = ((Creature)obj).Entry;
                rbounds = new HashSet<QuestTemplate>(Database.CreatureQuests.TryGet(entry).Select(x => Database.QuestTemplates.TryGet(x.QuestEntry)));
                ibounds = new HashSet<QuestTemplate>(Database.CreatureInvolvedQuests.TryGet(entry).Select(x => Database.QuestTemplates.TryGet(x.QuestEntry)));
            }
            else if (obj.IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
                rbounds = new HashSet<QuestTemplate>(); //TODO gameobject quests

            //Finish quest
            foreach (QuestTemplate qt in ibounds)
            {
                if (qt == null) continue;

                Quest quest = null;
                if (p.Quests.ContainsKey(qt.QuestId))
                    quest = p.Quests[qt.QuestId];

                status = p.GetQuestStatus(qt.QuestId);

                if (status == QuestStatuses.QUEST_STATUS_COMPLETE && quest?.Rewarded == false)
                    menu.AddItem(qt, QuestStatuses.QUEST_STATUS_COMPLETE);
                else if (status == QuestStatuses.QUEST_STATUS_INCOMPLETE) //Incomplete
                    menu.AddItem(qt, QuestStatuses.QUEST_STATUS_INCOMPLETE);
                else if (status == QuestStatuses.QUEST_STATUS_AVAILABLE)
                    menu.AddItem(qt, QuestStatuses.QUEST_STATUS_AVAILABLE);
            }

            //Check all object's quests
            foreach (QuestTemplate qt in rbounds)
            {
                if (qt == null) continue;

                if (p.GetQuestStatus(qt.QuestId) == QuestStatuses.QUEST_STATUS_NONE) //QUEST_STATUS_NONE aka New Quest
                {
                    if (!p.CheckQuestRequirements(qt, false) || !p.CheckQuestLevel(qt, false))
                        continue;

                    if (qt.IsAutoComplete)
                        menu.AddItem(qt, QuestStatuses.QUEST_STATUS_COMPLETE); //Autocomplete just show icon
                    else
                        menu.AddItem(qt, QuestStatuses.QUEST_STATUS_AVAILABLE);
                }
            }

            if (menu.Count == 1)
            {
                var quest = menu.MenuItems.First();
                if (quest.Value == QuestStatuses.QUEST_STATUS_COMPLETE)
                    if (quest.Key.GetRewItemCount > 0)
                        p.SendQuestGiverRequestItems(quest.Key, quest.Value, obj.Guid, true); //Send complete
                    else
                        p.SendQuestGiverOfferReward(quest.Key, obj.Guid, true); //Send complete
                else if (quest.Value == QuestStatuses.QUEST_STATUS_INCOMPLETE)
                    p.SendQuestGiverRequestItems(quest.Key, quest.Value, obj.Guid, true);
                else
                    p.SendQuestGiverQuestDetails(quest.Key, obj.Guid, true); //Send details
            }
            else
            {
                //Todo gossip text & emotes
                p.SendQuestGiverQuestList("Greetings $N", obj.Guid, menu.MenuItems);
            }

            p.UpdateSurroundingQuestStatus();
        }

        public static PacketWriter SendQuestGiverStatusQuery(this Player p, ulong guid)
        {
            uint questStatus = (uint)QuestGiverStatuses.QUEST_GIVER_NONE;
            if (Database.Creatures.ContainsKey(guid))
                questStatus = p.GetDialogStatus(Database.Creatures.TryGet(guid), questStatus);

            //TODO Gameobject

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_QUESTGIVER_STATUS);
            pkt.WriteUInt64(guid);
            pkt.WriteUInt32(questStatus);
            return pkt;
        }

        public static void RewardQuest(this Player p, QuestTemplate quest, uint reward, bool announce)
        {
            //Remove required items
            for (int i = 0; i < 4; i++)
            {
                if (quest.ReqItemId[i] > 0 && quest.ReqItemCount[i] > 0)
                    p.RemoveItem(quest.ReqItemId[i], quest.ReqItemCount[i]);
            }

            if (quest.GetRewChoiceItemCount > 0)
            {
                for (int i = 0; i < quest.GetRewChoiceItemCount; i++)
                {
                    if (quest.RewItemId[i] == reward && Database.Items.ContainsKey(reward))
                        p.AddItem(reward, quest.ReqItemCount[i]);
                }
            }

            //TODO reputation

            if (p.Level < Globals.MAX_LEVEL)
                p.GiveXp(quest.XPValue(p));
            else
                p.Money += quest.RewMoneyMaxLevel;

            //Take/Give money
            p.Money += (uint)quest.RewOrReqMoney;

            p.Quests[quest.QuestId].Status = QuestStatuses.QUEST_STATUS_COMPLETE;
            p.Quests[quest.QuestId].Rewarded = true;

            uint rewmoney = (quest.RewOrReqMoney < 0 ? 0 : (uint)quest.RewOrReqMoney);


            //Quest complete packet
            PacketWriter response = new PacketWriter(Opcodes.SMSG_QUESTGIVER_QUEST_COMPLETE);
            response.WriteUInt32(quest.QuestId);
            response.WriteUInt32(0x3);
            if (p.Level < Globals.MAX_LEVEL)
            {
                response.WriteUInt32(quest.XPValue(p));
                response.WriteUInt32(rewmoney);
            }
            else
            {
                response.WriteUInt32(0);
                response.WriteUInt32(rewmoney + quest.RewMoneyMaxLevel);
            }

            response.WriteUInt32(quest.GetRewItemCount);
            for (int i = 0; i < quest.GetRewItemCount; i++)
            {
                if (quest.RewItemId[i] > 0)
                {
                    response.WriteUInt32(quest.RewItemId[i]);
                    response.WriteUInt32(quest.RewItemCount[i]);
                }
                else
                {
                    response.WriteUInt32(0);
                    response.WriteUInt32(0);
                }
            }

            p.Client.Send(response);
            p.Dirty = true;

            if (quest.RewSpell > 0)
                p.ForceCastSpell(quest.RewSpell);
            else if (quest.RewSpellCast > 0)
                p.ForceCastSpell(quest.RewSpellCast);
        }


        public static void SendCantTakeQuestResponse(this Player p, uint msg)
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_QUESTGIVER_QUEST_INVALID);
            pw.WriteUInt32(msg);
            p.Client.Send(pw);
        }

        public static void SendQuestGiverOfferReward(this Player p, QuestTemplate quest, ulong guid, bool enablenext)
        {
            PacketWriter packet = new PacketWriter(Opcodes.SMSG_QUESTGIVER_OFFER_REWARD);
            packet.WriteUInt64(guid);
            packet.WriteUInt32(quest.QuestId);
            packet.WriteString(quest.Title);
            packet.WriteString(quest.OfferRewardText);
            packet.WriteUInt32(enablenext ? (uint)0x1 : 0x0);

            packet.WriteUInt32(0); //Emote count
            for (int i = 0; i < 4; i++)
            {
                packet.WriteUInt32(0);
                packet.WriteUInt32(0);
            }

            packet.WriteUInt32(quest.GetRewChoiceItemCount);
            for (int i = 0; i < quest.GetRewChoiceItemCount; i++)
            {
                packet.WriteUInt32(quest.RewChoiceItemId[i]);
                packet.WriteUInt32(quest.RewChoiceItemCount[i]);
                packet.WriteUInt32(Database.ItemTemplates.TryGet(quest.RewChoiceItemId[i])?.DisplayID ?? 0);
            }

            packet.WriteUInt32((uint)quest.GetReqItemCount);
            for (int i = 0; i < 4; i++)
            {
                packet.WriteUInt32(quest.ReqItemId[i]);
                packet.WriteUInt32(quest.ReqItemCount[i]);
                packet.WriteUInt32(Database.ItemTemplates.TryGet(quest.ReqItemId[i])?.DisplayID ?? 0);
            }

            packet.WriteUInt32(quest.RewOrReqMoney < 0 ? (uint)-quest.RewOrReqMoney : 0);
            packet.WriteUInt32(quest.RewSpell);
            packet.WriteUInt32(quest.RewSpellCast);
            p.Client.Send(packet);
        }

        public static void SendQuestGiverRequestItems(this Player p, QuestTemplate quest, QuestStatuses status, ulong guid, bool CloseOnCancel)
        {
            bool completeable = p.CanCompleteQuest(quest.QuestId);
            if (!quest.IsAutoComplete && status != QuestStatuses.QUEST_STATUS_COMPLETE) //Check if it is complete
                completeable = false;

            if (p.Quests.ContainsKey(quest.QuestId))
                if (p.Quests[quest.QuestId]?.Rewarded == true)
                    completeable = false;

            PacketWriter packet = new PacketWriter(Opcodes.SMSG_QUESTGIVER_REQUEST_ITEMS);
            packet.WriteUInt64(guid);
            packet.WriteUInt32(quest.QuestId);
            packet.WriteString(quest.Title);
            packet.WriteString(quest.RequestItemsText);
            packet.WriteUInt32(0);
            packet.WriteUInt32(0);
            packet.WriteUInt32(CloseOnCancel ? (uint)1 : 0);
            packet.WriteUInt32(quest.RewOrReqMoney < 0 ? (uint)-quest.RewOrReqMoney : 0);

            packet.WriteUInt32((uint)quest.GetReqItemCount);
            for (int i = 0; i < 4; i++)
            {
                packet.WriteUInt32(quest.ReqItemId[i]);
                packet.WriteUInt32(quest.ReqItemCount[i]);
                packet.WriteUInt32(Database.ItemTemplates.TryGet(quest.ReqItemId[i])?.DisplayID ?? 0);
            }

            packet.WriteUInt32(2);
            packet.WriteUInt32(completeable ? (uint)3 : 0);
            packet.WriteUInt32(4);
            packet.WriteUInt32(8);
            packet.WriteUInt32(0x10);
            p.Client.Send(packet);
        }

        public static void SendQuestGiverQuestDetails(this Player p, QuestTemplate quest, ulong guid, bool ActivateClose)
        {
            PacketWriter packet = new PacketWriter(Opcodes.SMSG_QUESTGIVER_QUEST_DETAILS);
            packet.WriteUInt64(guid);
            packet.WriteUInt32(quest.QuestId);
            packet.WriteString(quest.Title);
            packet.WriteString(quest.Details);
            packet.WriteString(quest.Objectives);
            packet.WriteUInt32(ActivateClose ? (uint)1 : 0);

            //Reward choices
            packet.WriteUInt32((uint)quest.GetRewChoiceItemCount);
            for (int i = 0; i < 4; i++)
            {
                packet.WriteUInt32(quest.RewChoiceItemId[i]);
                packet.WriteUInt32(quest.RewChoiceItemCount[i]);
                packet.WriteUInt32(Database.ItemTemplates.TryGet(quest.RewChoiceItemId[i])?.DisplayID ?? 0);
            }

            //Reward items
            packet.WriteUInt32((uint)quest.GetRewItemCount);
            for (int i = 0; i < 4; i++)
            {

                packet.WriteUInt32(quest.RewItemId[i]);
                packet.WriteUInt32(quest.RewItemCount[i]);

                if (Database.ItemTemplates.ContainsKey(quest.RewItemId[i]))
                {
                    ItemTemplate itemp = Database.ItemTemplates.TryGet(quest.RewItemId[i]);
                    packet.WriteUInt32(itemp.DisplayID);
                    p.QueryItemCheck(quest.RewItemId[i]);
                }
                else
                    packet.WriteUInt32(0);
            }

            //Money
            packet.WriteUInt32((uint)quest.RewOrReqMoney);

            //Required items
            packet.WriteUInt32((uint)quest.GetReqItemCount);
            for (int i = 0; i < 4; i++)
            {
                packet.WriteUInt32(quest.ReqItemId[i]);
                packet.WriteUInt32(quest.ReqItemCount[i]);
                p.QueryItemCheck(quest.ReqItemId[i]);
            }

            //Required creature/gameobject kills
            packet.WriteUInt32((uint)quest.GetReqCreatureOrGOCount);
            for (int i = 0; i < 4; i++)
            {
                if (quest.ReqCreatureOrGOId[i] < 0)
                    packet.WriteUInt32((uint)(quest.ReqCreatureOrGOId[i] * -1) | 0x80000000);
                else
                    packet.WriteUInt32((uint)quest.ReqCreatureOrGOId[i]);

                packet.WriteUInt32(quest.ReqCreatureOrGOCount[i]);
            }

            p.Client.Send(packet);
        }

        public static void SendQuestGiverQuestList(this Player p, string message, ulong guid, Dictionary<QuestTemplate, QuestStatuses> quests)
        {
            WorldObject questGiver = Database.Creatures.TryGet(guid);
            if (questGiver == null)
                return;

            HashSet<QuestQuery> questlist = new HashSet<QuestQuery>();
            foreach (var quest in quests)
            {
                uint status = (uint)(quest.Value == QuestStatuses.QUEST_STATUS_COMPLETE ? 0x4 : 0x0);
                questlist.Add(new QuestQuery(quest.Key.QuestId, status, quest.Key.QuestLevel, quest.Key.Title));
            }

            PacketWriter packet = new PacketWriter(Opcodes.SMSG_QUESTGIVER_QUEST_LIST);
            packet.WriteUInt64(guid);
            packet.WriteString(message);
            packet.WriteInt32(0);
            packet.WriteInt32(0);
            packet.WriteUInt8((byte)questlist.Count);

            foreach (QuestQuery qq in questlist)
            {
                packet.WriteUInt32(qq.Id);
                packet.WriteUInt32(qq.Status);
                packet.WriteUInt32(qq.Level);
                packet.WriteString(qq.Title);
            }

            p.Client.Send(packet);

        }

        public static void SendQuestQueryResponse(this Player p, QuestTemplate quest)
        {
            PacketWriter response = new PacketWriter(Opcodes.SMSG_QUEST_QUERY_RESPONSE);
            response.WriteUInt32(quest.QuestId);
            response.WriteUInt32(quest.QuestMethod);
            response.WriteUInt32(quest.QuestLevel);
            response.WriteUInt32((uint)quest.ZoneOrSort);
            response.WriteUInt32(quest.Type);
            response.WriteUInt32(quest.NextQuestInChain);
            response.WriteUInt32((uint)(quest.RewOrReqMoney < 0 ? 0 : quest.RewOrReqMoney));
            response.WriteUInt32(quest.SrcItemId);

            for (int i = 0; i < 4; i++)
            {
                response.WriteUInt32(quest.RewItemId[i]);
                response.WriteUInt32(quest.RewItemCount[i]);

                p.QueryItemCheck(quest.RewItemId[i]);
            }

            for (int i = 0; i < 6; i++)
            {
                response.WriteUInt32(quest.RewChoiceItemId[i]);
                response.WriteUInt32(quest.RewChoiceItemCount[i]);

                p.QueryItemCheck(quest.RewChoiceItemId[i]);
            }

            response.WriteUInt32(quest.PointMapId);
            response.WriteFloat(quest.PointX);
            response.WriteFloat(quest.PointY);
            response.WriteUInt32(quest.PointOpt);

            response.WriteString(quest.Title);
            response.WriteString(quest.Details);
            response.WriteString(quest.Objectives);
            response.WriteString(quest.EndText);

            for (int i = 0; i < 4; i++)
            {
                if (quest.ReqCreatureOrGOId[i] < 0)
                    response.WriteUInt32((uint)(quest.ReqCreatureOrGOId[i] * -1) | 0x80000000);
                else
                    response.WriteUInt32((uint)quest.ReqCreatureOrGOId[i]);

                response.WriteUInt32(quest.ReqCreatureOrGOCount[i]);
                response.WriteUInt32(quest.ReqItemId[i]);
                response.WriteUInt32(quest.ReqItemCount[i]);
                p.QueryItemCheck(quest.ReqItemId[i]);
            }

            for (int i = 0; i < 3; i++)
                response.WriteString(quest.ObjectiveText[i]);

            p.Client.Send(response);
        }

        public static bool CheckQuestRequirements(this Player p, QuestTemplate qt, bool msg)
        {
            if (qt.RequiredRaces > 0 && !qt.RequiredRaces.HasFlag(p.RaceMask)) //Not required race
            {
                if (msg)
                    p.SendCantTakeQuestResponse((uint)QuestFailedReasons.INVALIDREASON_QUEST_FAILED_WRONG_RACE);
                return false;
            }

            if (qt.SrcItemId > 0)
            {
                if(p.Inventory.GetEntryCount(qt.SrcItemId) == 0)
                    return false;
            }

            bool invalidReq = false;
            if (qt.RequiredClasses > 0 && !qt.RequiredClasses.HasFlag(p.Class)) //Not required class
                invalidReq = true;

            if (qt.NextQuestInChain > 0 && p.Quests.ContainsKey(qt.NextQuestInChain)) //Already started the next quest in chain
                invalidReq = true;

            if (qt.PrevQuestId > 0 && !p.Quests.ContainsKey((uint)qt.PrevQuestId)) //Missing previous quest
                invalidReq = true;

            if (qt.PrevQuestId > 0 && p.Quests.ContainsKey((uint)qt.PrevQuestId))
            {
                Quest quest = p.Quests[(uint)qt.PrevQuestId];
                if (!quest.Rewarded || quest.Status != QuestStatuses.QUEST_STATUS_COMPLETE)
                    invalidReq = true;
            }

            if (qt.RequiredSkill > 0)
            {
                if (!p.Skills.ContainsKey((ushort)qt.RequiredSkill)) //Missing skill
                    invalidReq = true;
                else if (p.Skills[(ushort)qt.RequiredSkill].m_skillRank < qt.RequiredSkillValue) //Not high enough level
                    invalidReq = true;
            }

            if (invalidReq)
            {
                if (msg)
                    p.SendCantTakeQuestResponse((uint)QuestFailedReasons.INVALIDREASON_QUEST_FAILED_MISSING_ITEMS);
                return false;
            }
            else
                return true;
        }

        public static bool CheckQuestLevel(this Player p, QuestTemplate qt, bool msg)
        {
            if (p.Level < qt.MinLevel)
            {
                if (msg)
                    p.SendCantTakeQuestResponse((uint)QuestFailedReasons.INVALIDREASON_QUEST_FAILED_LOW_LEVEL);
                return false;
            }

            return true;
        }

        public static bool CanCompleteQuest(this Player p, uint questid)
        {
            QuestTemplate template = Database.QuestTemplates.TryGet(questid);
            if (template == null || !p.Quests.ContainsKey(questid))
                return false;

            Quest quest = p.Quests[questid];
            if (template.IsAutoComplete)
                return true;

            //Item check
            for (int i = 0; i < 4; i++)
            {
                uint reqcount = template.ReqItemCount[i];
                uint reqitem = template.ReqItemId[i];

                if (reqitem == 0 || reqcount == 0)
                    continue;

                if (!quest.ReqItems.ContainsKey(reqitem))
                    return false;
                if (quest.ReqItems[reqitem] < reqcount)
                    return false;
            }

            //GO check
            for (int i = 0; i < 4; i++)
            {
                uint reqcount = template.ReqCreatureOrGOCount[i];
                uint reqobj = (uint)template.ReqCreatureOrGOId[i];

                if (reqcount == 0 || reqobj == 0)
                    continue;

                if (!quest.ReqCreatureGo.ContainsKey(reqobj))
                    return false;
                if (quest.ReqCreatureGo[reqobj] < reqcount)
                    return false;
            }

            //Money check
            if (template.RewOrReqMoney < 0 && p.Money < (template.RewOrReqMoney * -1))
                return false;

            return true;
        }

        public static bool CanRewardQuest(this Player p, QuestTemplate quest, uint reward)
        {
            if (!p.Quests.ContainsKey(quest.QuestId))
                return false;

            Quest pQuest = p.Quests[quest.QuestId];

            if (!quest.IsAutoComplete && pQuest.Status != QuestStatuses.QUEST_STATUS_COMPLETE)
                return false;

            if (quest.RewOrReqMoney < 0 && p.Money < -quest.RewOrReqMoney)
                return false;

            if (quest.HasItemReward && !p.Inventory.CanStoreItem(quest.RewItemId[reward], quest.RewItemCount[reward]))
                return false;

            if (pQuest.Rewarded)
                return false;

            return true;
        }

        public static void UpdateSurroundingQuestStatus(this Player p)
        {
            Parallel.ForEach(p.ObjectsInRange.Values, (x) =>
            {
                if(x.IsTypeOf(ObjectTypes.TYPE_UNIT) && !x.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                {
                    uint entry = ((Creature)x).Entry;
                    if (Database.CreatureInvolvedQuests.ContainsKey(entry) || Database.CreatureQuests.ContainsKey(entry))
                        p.Client.Send(p.SendQuestGiverStatusQuery(x.Guid), true);
                }
            });
        }


        public static bool CheckIsRequiredQuestItem(this Player p, uint entry)
        {
            foreach (Quest quest in p.Quests.Values)
            {
                QuestTemplate template = Database.QuestTemplates.TryGet(quest.QuestId);
                if (template == null || quest.Status == QuestStatuses.QUEST_STATUS_COMPLETE)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    if (template.ReqItemId[i] == entry)
                        return true;
                }
            }

            return false;
        }

        public static void CheckQuestItemAdded(this Player p, uint entry, uint count)
        {
            foreach (Quest quest in p.Quests.Values)
            {
                QuestTemplate template = Database.QuestTemplates.TryGet(quest.QuestId);
                if (template == null || quest.Status == QuestStatuses.QUEST_STATUS_COMPLETE)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    if (template.ReqItemId[i] == 0)
                        continue;

                    uint reqitem = template.ReqItemId[i];

                    if (reqitem == entry)
                    {
                        uint curcount = (uint)(quest.ReqItems.ContainsKey(reqitem) ? quest.ReqItems[reqitem] : 0);
                        uint reqcount = template.ReqItemCount[i];
                        if (curcount < reqcount)
                        {
                            p.Quests[quest.QuestId].AddItem(entry, count);

                            //Quest update
                            PacketWriter response = new PacketWriter(Opcodes.SMSG_QUESTUPDATE_ADD_ITEM);
                            response.WriteUInt32(entry);
                            response.WriteUInt32(count);
                            p.Client.Send(response);
                        }

                        if (p.CanCompleteQuest(quest.QuestId))
                        {
                            p.Quests[quest.QuestId].Status = QuestStatuses.QUEST_STATUS_COMPLETE;
                            p.UpdateSurroundingQuestStatus();
                        }
                    }
                }
            }
        }

        public static void CheckQuestItemRemove(this Player p, uint entry, uint count)
        {
            foreach (Quest quest in p.Quests.Values)
            {
                QuestTemplate template = Database.QuestTemplates.TryGet(quest.QuestId);
                if (template == null || quest.Status == QuestStatuses.QUEST_STATUS_COMPLETE)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    if (template.ReqItemId[i] == 0)
                        continue;

                    uint reqitem = template.ReqItemId[i];

                    if (reqitem == entry)
                    {
                        uint curcount = (uint)(quest.ReqItems.ContainsKey(reqitem) ? quest.ReqItems[reqitem] : 0);
                        uint reqcount = template.ReqItemCount[i];
                        if (curcount < reqcount)
                        {
                            if (p.Quests[quest.QuestId].ReqItems.ContainsKey(entry))
                                p.Quests[quest.QuestId].ReqItems[entry] -= (count > curcount ? 0 : count);

                            if (p.Quests[quest.QuestId].Status == QuestStatuses.QUEST_STATUS_COMPLETE)
                                p.Quests[quest.QuestId].Status = QuestStatuses.QUEST_STATUS_INCOMPLETE;
                        }

                        return;
                    }
                }
            }
        }

        public static void CheckQuestCreatureKill(this Player p, ulong guid)
        {
            foreach (Quest quest in p.Quests.Values)
            {
                QuestTemplate template = Database.QuestTemplates.TryGet(quest.QuestId);
                Creature creature = Database.Creatures.TryGet(guid);

                if (template == null || creature == null || quest.Status == QuestStatuses.QUEST_STATUS_COMPLETE)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    if (template.ReqCreatureOrGOId[i] <= 0 || template.ReqSpell[i] != 0)
                        continue;

                    uint reqkill = (uint)template.ReqCreatureOrGOId[i];
                    if (reqkill == creature.Entry)
                    {
                        uint curcount = quest.ReqCreatureGo.ContainsKey(reqkill) ? quest.ReqCreatureGo[reqkill] : 0;
                        uint reqcount = template.ReqCreatureOrGOCount[i];
                        if (curcount < reqcount)
                        {
                            p.Quests[quest.QuestId].AddCreatureGO(creature.Entry, 1);

                            //Quest update
                            PacketWriter response = new PacketWriter(Opcodes.SMSG_QUESTUPDATE_ADD_KILL);
                            response.WriteUInt32(quest.QuestId);
                            response.WriteUInt32(creature.Entry);
                            response.WriteUInt32(curcount + 1);
                            response.WriteUInt32(reqcount);
                            response.WriteUInt64(guid);
                            p.Client.Send(response);
                        }

                        if (p.CanCompleteQuest(quest.QuestId))
                        {
                            p.Quests[quest.QuestId].Status = QuestStatuses.QUEST_STATUS_COMPLETE;
                            p.UpdateSurroundingQuestStatus();
                        }
                    }
                }
            }
        }

        public static void CheckQuestTalkedTo(this Player p, ulong guid)
        {
            foreach (Quest quest in p.Quests.Values)
            {
                QuestTemplate template = Database.QuestTemplates.TryGet(quest.QuestId);
                Creature creature = Database.Creatures.TryGet(guid);

                if (template == null || creature == null || quest.Status == QuestStatuses.QUEST_STATUS_COMPLETE)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    if (template.ReqCreatureOrGOId[i] <= 0 || template.ReqSpell[i] != 0)
                        continue;

                    uint reqkill = template.ReqItemId[i];
                    if (reqkill == creature.Entry)
                    {
                        uint curcount = (uint)(quest.ReqCreatureGo.ContainsKey(reqkill) ? quest.ReqCreatureGo[reqkill] : 0);
                        uint reqcount = template.ReqCreatureOrGOCount[i];
                        if (curcount < reqcount)
                        {
                            p.Quests[quest.QuestId].AddCreatureGO(creature.Entry, 1);

                            //Quest update
                            PacketWriter response = new PacketWriter(Opcodes.SMSG_QUESTUPDATE_ADD_KILL);
                            response.WriteUInt32(quest.QuestId);
                            response.WriteUInt32(creature.Entry);
                            response.WriteUInt32(curcount + 1);
                            response.WriteUInt32(reqcount);
                            response.WriteUInt64(guid);
                            p.Client.Send(response);
                        }

                        if (p.CanCompleteQuest(quest.QuestId))
                        {
                            p.Quests[quest.QuestId].Status = QuestStatuses.QUEST_STATUS_COMPLETE;
                            p.UpdateSurroundingQuestStatus();
                        }
                    }
                }
            }
        }

        internal class QuestMenu
        {
            public Dictionary<QuestTemplate, QuestStatuses> MenuItems = new Dictionary<QuestTemplate, QuestStatuses>();
            public int Count { get { return this.MenuItems.Count; } }

            public void AddItem(QuestTemplate quest, QuestStatuses status)
            {
                this.MenuItems.Add(quest, status);
            }

            public void ClearMenu()
            {
                this.MenuItems = new Dictionary<QuestTemplate, QuestStatuses>();
            }
        }

        internal struct QuestQuery
        {
            public uint Id;
            public uint Status;
            public uint Level;
            public string Title;

            public QuestQuery(uint id, uint status, uint level, string title)
            {
                this.Id = id;
                this.Status = status;
                this.Level = level;
                this.Title = title;
            }
        }

    }
}
