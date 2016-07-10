using Common.Constants;
using Common.Network.Packets;
using System.Collections.Generic;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.PlayerExtensions;
using WorldServer.Game.Objects.PlayerExtensions.Quests;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Game.Structs;
using WorldServer.Network;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class QuestHandler
    {
        public static void HandleQuestGiverStatusQuery(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            //TODO Gameobject
            manager.Send(manager.Character.SendQuestGiverStatusQuery(guid));
        }

        public static void HandleQuestgiverHelloOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();

            if (Database.Creatures.ContainsKey(guid))
            {
                if (Database.Creatures.TryGet(guid).IsEnemyTo(manager.Character))
                    return;

                manager.Character.SendPreparedQuest(Database.Creatures.TryGet(guid));
            }
                
        }

        public static void HandleQuestgiverAcceptQuestOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            uint questid = packet.ReadUInt32();

            QuestTemplate quest = Database.QuestTemplates.TryGet(questid);

            WorldObject obj = null; //TODO add gameobject check
            obj = Database.Creatures.TryGet<WorldObject>(guid) ??
                  Database.Items.TryGet<WorldObject>(guid); //Try to find the object

            if (obj == null || quest == null || obj?.HasQuest(questid) == false) //Check the quest/giver exists
                return;

            if (!manager.Character.CheckQuestRequirements(quest, false) || !manager.Character.CheckQuestLevel(quest, false))
                return; //Double check we can take this quest

            if (obj.IsTypeOf(ObjectTypes.TYPE_UNIT) && (((Creature)obj).IsEnemyTo(manager.Character)))
                return;

            manager.Character.AddQuest(quest.QuestId, guid); //Add quest
            manager.Character.Dirty = true;

            manager.Character.UpdateSurroundingQuestStatus(); //Update surrounding
        }

        public static void HandleQuestgiverQueryQuestOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            uint questid = packet.ReadUInt32();

            QuestTemplate quest = Database.QuestTemplates.TryGet(questid);

            WorldObject obj = null; //TODO add gameobject check
            obj = Database.Creatures.TryGet<WorldObject>(guid) ??
                  Database.Items.TryGet<WorldObject>(guid); //Try to find the object

            if (obj == null || quest == null || obj?.HasQuest(questid) == false) //Check the quest/giver exists
                return;
            if (obj.IsTypeOf(ObjectTypes.TYPE_UNIT) && (((Creature)obj).IsEnemyTo(manager.Character)))
                return;

            manager.Character.SendQuestGiverQuestDetails(quest, guid, true);
        }

        public static void HandleQuestQueryOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            uint questid = packet.ReadUInt32();
            QuestTemplate quest = Database.QuestTemplates.TryGet(questid);
            if (quest != null)
                manager.Character.SendQuestQueryResponse(quest);
        }

        public static void HandleQuestgiverCompleteQuest(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            uint questid = packet.ReadUInt32();

            if (Database.QuestTemplates.ContainsKey(questid))
            {
                QuestTemplate quest = Database.QuestTemplates.TryGet(questid);
                if(manager.Character.Quests.ContainsKey(questid))
                {
                    Quest pQuest = manager.Character.Quests[questid];
                    manager.Character.SendQuestGiverRequestItems(quest, pQuest.Status, guid, false);
                }
                else
                    manager.Character.SendQuestGiverQuestDetails(quest, guid, true);
            }
        }

        public static void HandleQuestgiverChooseRewardOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            uint quest = packet.ReadUInt32();
            uint reward = packet.ReadUInt32();

            if (reward > 6) //Only 6 choices of reward
                return;

            QuestTemplate template = Database.QuestTemplates.TryGet(quest);
            if (template == null)
                return;

            if(manager.Character.CanRewardQuest(template, reward))
            {
                manager.Character.RewardQuest(template, reward, true);

                if(template.NextQuestInChain > 0 && Database.QuestTemplates.ContainsKey(template.NextQuestInChain))
                    manager.Character.SendQuestGiverQuestDetails(Database.QuestTemplates.TryGet(template.NextQuestInChain), guid, true);                   
            }
            else
                manager.Character.SendQuestGiverOfferReward(template, guid, true);
        }

        public static void HandleQuestgiverRequestRewardOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            uint quest = packet.ReadUInt32();

            if(manager.Character.CanCompleteQuest(quest))
                manager.Character.Quests[quest].Status = QuestStatuses.QUEST_STATUS_COMPLETE;

            if (manager.Character.GetQuestStatus(quest) != QuestStatuses.QUEST_STATUS_COMPLETE)
                return;

            if (Database.QuestTemplates.ContainsKey(quest))
                manager.Character.SendQuestGiverOfferReward(Database.QuestTemplates.TryGet(quest), guid, true);
        }

        public static void HandleQuestLogRemoveQuest(ref PacketReader packet, ref WorldManager manager)
        {
            byte slot = packet.ReadUInt8();
            uint questid = 0;
            int i = 0;
            foreach(var q in manager.Character.Quests.Values)
            {
                if (q.Status == QuestStatuses.QUEST_STATUS_COMPLETE && q.Rewarded)
                    continue;

                if(i == slot)
                {
                    questid = q.QuestId;
                    break;
                }
                i++;
            }

            if(manager.Character.Quests.ContainsKey(questid))
            {
                manager.Character.Quests.Remove(questid);
                manager.Character.UpdateSurroundingQuestStatus();
                manager.Character.Dirty = true;
            }
        }
    }
}
