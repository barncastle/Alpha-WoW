using Common.Database.DBC.Reader;
using Common.Database.DBC.Structures;
using Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC
{
    public class DBC
    {
        public static int Count { get; set; }
        public static ConcurrentDictionary<uint, Spell> Spell;
        public static ConcurrentDictionary<int, SpellRange> SpellRange;
        public static ConcurrentDictionary<int, SpellRadius> SpellRadius;
        public static ConcurrentDictionary<int, SpellDuration> SpellDuration;
        public static ConcurrentDictionary<int, SpellCastTimes> SpellCastTimes;
        public static ConcurrentDictionary<uint, CharStartOutfit> CharStartOutfit;
        public static ConcurrentDictionary<int, SkillLine> SkillLine;
        public static ConcurrentDictionary<int, SkillLineAbility> SkillLineAbility;
        //public static HashSet<AreaTrigger> AreaTrigger;
        public static ConcurrentDictionary<int, ChrRaces> ChrRaces;
        public static HashSet<CharBaseInfo> CharBaseInfo;
        public static ConcurrentDictionary<int, ChrProficiency> ChrProficiency;
        public static ConcurrentDictionary<int, WorldSafeLocs> WorldSafeLocs;
        public static ConcurrentDictionary<uint, FactionTemplate> FactionTemplate;
        public static ConcurrentDictionary<int, EmotesText> EmotesText;
        public static ConcurrentDictionary<uint, BankBagSlotPrices> BankBagSlotPrices;
        public static ConcurrentDictionary<uint, AreaTable> AreaTable;
        public static ConcurrentDictionary<uint, Lock> Lock;
        public static ConcurrentDictionary<uint, LockType> LockType;

        public static void Initialize()
        {
            Log.Message(LogType.NORMAL, "Loading DBCs...");

            Spell = DBReader.Read<uint, Spell>("Spell.dbc", "Id");
            SpellRange = DBReader.Read<int, SpellRange>("SpellRange.dbc", "m_ID");
            SpellRadius = DBReader.Read<int, SpellRadius>("SpellRadius.dbc", "m_ID");
            SpellDuration = DBReader.Read<int, SpellDuration>("SpellDuration.dbc", "m_ID");
            SpellCastTimes = DBReader.Read<int, SpellCastTimes>("SpellCastTimes.dbc", "m_ID");
            CharStartOutfit = DBReader.Read<uint, CharStartOutfit>("CharStartOutfit.dbc", "ID");
            SkillLine = DBReader.Read<int, SkillLine>("SkillLine.dbc", "m_ID");
            SkillLineAbility = DBReader.Read<int, SkillLineAbility>("SkillLineAbility.dbc", "m_ID");
            //AreaTrigger = DBReader.Read<AreaTrigger>("AreaTrigger.dbc");
            ChrRaces = DBReader.Read<int, ChrRaces>("ChrRaces.dbc", "m_ID");
            CharBaseInfo = DBReader.Read<CharBaseInfo>("CharBaseInfo.dbc");
            ChrProficiency = DBReader.Read<int, ChrProficiency>("ChrProficiency.dbc", "m_ID");
            WorldSafeLocs = DBReader.Read<int, WorldSafeLocs>("WorldSafeLocs.dbc", "m_ID");
            FactionTemplate = DBReader.Read<uint, FactionTemplate>("FactionTemplate.dbc", "m_ID");
            EmotesText = DBReader.Read<int, EmotesText>("EmotesText.dbc", "m_ID");
            BankBagSlotPrices = DBReader.Read<uint, BankBagSlotPrices>("BankBagSlotPrices.dbc", "m_ID");
            AreaTable = DBReader.Read<uint, AreaTable>("AreaTable.dbc", "m_ID");
            Lock = DBReader.Read<uint, Lock>("Lock.dbc", "m_ID");
            LockType = DBReader.Read<uint, LockType>("LockType.dbc", "m_ID");

            Log.Message(LogType.NORMAL, "Loaded {0} DBCs.", Count);
            Log.Message();
        }
    }
}
