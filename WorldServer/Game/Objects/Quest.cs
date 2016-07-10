using Common.Constants;
using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game.Structs;
using WorldServer.Storage;

namespace WorldServer.Game.Objects
{
    [Table("characters_quests")]
    public class Quest
    {
        [Column("quest_id")]
        public uint QuestId = 0;
        [Column("quest_staus")]
        public QuestStatuses Status = QuestStatuses.QUEST_STATUS_NONE;
        [Column("quest_rewarded")]
        public bool Rewarded = false;
        public uint Timer = 0;
        public ulong Giver = 0;

        public Dictionary<uint, uint> ReqItems = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> ReqCreatureGo = new Dictionary<uint, uint>();

        public Quest(uint questid)
        {
            this.QuestId = questid;
            Status = QuestStatuses.QUEST_STATUS_INCOMPLETE;
        }

        public Quest(uint questid, ulong guid)
        {
            this.QuestId = questid;
            this.Giver = guid;
            Status = QuestStatuses.QUEST_STATUS_INCOMPLETE;
        }

        public void AddItem(uint entry, uint count)
        {
            if (ReqItems.ContainsKey(entry))
                ReqItems[entry] += count;
            else
                ReqItems.Add(entry, count);
        }

        public void AddCreatureGO(uint entry, uint count)
        {
            if (ReqCreatureGo.ContainsKey(entry))
                ReqCreatureGo[entry] += count;
            else
                ReqCreatureGo.Add(entry, count);
        }

        public uint GetProgress()
        {
            QuestTemplate template = Database.QuestTemplates.TryGet(QuestId);

            int val = 0;
            for (int i = 0; i < 4; i++)
            {
                if (ReqCreatureGo.ContainsKey((uint)template.ReqCreatureOrGOId[i]) && template.ReqCreatureOrGOId[i] != 0)
                {
                    int count = (int)ReqCreatureGo[(uint)template.ReqCreatureOrGOId[i]];
                    val |= ((i + 1) << count) - 1;
                }
            }
            return (uint)val;
        }
    }
}
