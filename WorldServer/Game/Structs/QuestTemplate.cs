using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game.Objects;

namespace WorldServer.Game.Structs
{
    [Table("quests")]
    public class QuestTemplate
    {
        [Key]
        [Column("entry")]
        public uint QuestId { get; set; }
        [Column("method")]
        public uint QuestMethod { get; set; }
        [Column("zoneorsort")]
        public int ZoneOrSort { get; set; }
        [Column("minlevel")]
        public uint MinLevel { get; set; }
        [Column("questlevel")]
        public uint QuestLevel { get; set; }
        [Column("type")]
        public uint Type { get; set; }
        [Column("requiredclass")]
        public uint RequiredClasses { get; set; }
        [Column("requiredraces")]
        public uint RequiredRaces { get; set; }
        [Column("requiredskill")]
        public uint RequiredSkill { get; set; }
        [Column("requiredskillvalue")]
        public uint RequiredSkillValue { get; set; }
        [Column("RequiredMinRepFaction")]
        public uint RequiredMinRepFaction { get; set; }
        [Column("RequiredMinRepValue")]
        public int RequiredMinRepValue { get; set; }
        [Column("RequiredMaxRepFaction")]
        public uint RequiredMaxRepFaction { get; set; }
        [Column("RequiredMaxRepValue")]
        public int RequiredMaxRepValue { get; set; }
        [Column("SuggestedPlayers")]
        public uint SuggestedPlayers { get; set; }
        [Column("LimitTime")]
        public uint LimitTime { get; set; }
        [Column("QuestFlags")]
        public uint m_QuestFlags { get; set; }
        [Column("SpecialFlags")]
        public uint m_SpecialFlags { get; set; }
        [Column("PrevQuestId")]
        public int PrevQuestId { get; set; }
        [Column("NextQuestId")]
        public int NextQuestId { get; set; }
        [Column("ExclusiveGroup")]
        public int ExclusiveGroup { get; set; }
        [Column("NextQuestInChain")]
        public uint NextQuestInChain { get; set; }
        [Column("SrcItemId")]
        public uint SrcItemId { get; set; }
        [Column("SrcItemCount")]
        public uint SrcItemCount { get; set; }
        [Column("SrcSpell")]
        public uint SrcSpell { get; set; }
        [Column("Title")]
        public string Title { get; set; }
        [Column("Details")]
        public string Details { get; set; }
        [Column("Objectives")]
        public string Objectives { get; set; }
        [Column("OfferRewardText")]
        public string OfferRewardText { get; set; }
        [Column("RequestItemsText")]
        public string RequestItemsText { get; set; }
        [Column("EndText")]
        public string EndText { get; set; }
        [Column("RewOrReqMoney")]
        public int RewOrReqMoney { get; set; }
        [Column("RewMoneyMaxLevel")]
        public uint RewMoneyMaxLevel { get; set; }
        [Column("RewSpell")]
        public uint RewSpell { get; set; }
        [Column("RewSpellCast")]
        public uint RewSpellCast { get; set; }
        [Column("PointMapId")]
        public uint PointMapId { get; set; }
        [Column("PointX")]
        public float PointX { get; set; }
        [Column("PointY")]
        public float PointY { get; set; }
        [Column("PointOpt")]
        public uint PointOpt { get; set; }
        [Column("IncompleteEmote")]
        public uint IncompleteEmote { get; set; }
        [Column("CompleteEmote")]
        public uint CompleteEmote { get; set; }

        [ColumnList("ObjectiveText", 4)]
        public string[] ObjectiveText { get; set; }
        [ColumnList("ReqItemId", 4)]
        public uint[] ReqItemId { get; set; }
        [ColumnList("ReqItemCount", 4)]
        public uint[] ReqItemCount { get; set; }
        [ColumnList("ReqCreatureOrGOId", 4)]
        public int[] ReqCreatureOrGOId { get; set; }
        [ColumnList("ReqCreatureOrGOCount", 4)]
        public uint[] ReqCreatureOrGOCount { get; set; }
        [ColumnList("ReqSpellCast", 4)]
        public uint[] ReqSpell { get; set; }
        [ColumnList("RewChoiceItemId", 6)]
        public uint[] RewChoiceItemId { get; set; }
        [ColumnList("RewChoiceItemCount", 6)]
        public uint[] RewChoiceItemCount { get; set; }
        [ColumnList("RewItemId", 4)]
        public uint[] RewItemId { get; set; }
        [ColumnList("RewItemCount", 4)]
        public uint[] RewItemCount { get; set; }
        [ColumnList("DetailsEmote", 4)]
        public uint[] DetailsEmote { get; set; }
        [ColumnList("OfferRewardEmote", 4)]
        public uint[] OfferRewardEmote { get; set; }

        private uint _RewChoiceItemCount = 0;
        private uint _RewItemCount = 0;
        private uint _ReqItemCount = 0;
        private uint _ReqCreatureOrGOCount = 0;

        public uint GetRewChoiceItemCount { get { return _RewChoiceItemCount; } }
        public uint GetRewItemCount { get { return _RewItemCount; } }
        public uint GetReqItemCount { get { return _ReqItemCount; } }
        public uint GetReqCreatureOrGOCount { get { return _ReqCreatureOrGOCount; } }

        public bool IsAutoComplete { get { return QuestMethod == 0; } }
        public bool HasReward { get { return _RewChoiceItemCount + _RewItemCount + RewOrReqMoney == 0; } }
        public bool HasItemReward { get { return _RewChoiceItemCount > 0; } }

        public uint XPValue(Player player)
        {
            if (player != null)
            {
                if (RewMoneyMaxLevel > 0)
                {
                    uint pLevel = player.Level;
                    uint qLevel = QuestLevel;
                    float fullxp = 0;
                    if (qLevel >= 65)
                        fullxp = RewMoneyMaxLevel / 6.0f;
                    else if (qLevel <= 60)
                        fullxp = RewMoneyMaxLevel / 0.6f;
                    else
                        fullxp = RewMoneyMaxLevel / (1.2f * (qLevel - 60));

                    if (pLevel <= qLevel + 5)
                        return (uint)(Math.Ceiling(fullxp));
                    else if (pLevel >= qLevel + 9)
                        return (uint)(Math.Ceiling(fullxp * 0.1f));
                    else
                    {
                        uint levelDiff = (pLevel - (qLevel + 6));
                        float modifier = 0.8f - (0.2f * levelDiff);
                        return (uint)(Math.Ceiling(fullxp * modifier));
                    }
                }
            }

            return 0;
        }

        public void Load()
        {
            for (int i = 0; i < RewChoiceItemId.Length; i++)
                if (RewChoiceItemId[i] > 0)
                    _RewChoiceItemCount++;

            for (int i = 0; i < RewItemId.Length; i++)
                if (RewItemId[i] > 0)
                    _RewItemCount++;

            for (int i = 0; i < ReqItemId.Length; i++)
                if (ReqItemId[i] > 0)
                    _ReqItemCount++;

            for (int i = 0; i < ReqCreatureOrGOId.Length; i++)
                if (ReqCreatureOrGOId[i] > 0)
                    _ReqCreatureOrGOCount++;
        }
    }
}
