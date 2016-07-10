using Common.Database.DBC.CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Database.DBC.Structures
{
    public class Spell
    {
        public uint Id;
        public uint School;
        public uint Category;
        public uint CastUI;
        public uint Attributes;
        public uint AttributesEx;
        public uint ShapeshiftMask;
        public uint Targets;
        public uint TargetCreatureType;
        public uint RequiresSpellFocus;
        public uint CasterAuraState;
        public uint TargetAuraState;
        public uint CastingTimeIndex;
        public uint RecoveryTime;
        public uint CategoryRecoveryTime;
        public uint InterruptFlags;
        public uint AuraInterruptFlags;
        public uint ChannelInterruptFlags;
        public uint procFlags;
        public uint procChance;
        public uint procCharges;
        public uint maxLevel;
        public uint baseLevel;
        public uint spellLevel;
        public uint DurationIndex;
        public uint powerType;
        public uint manaCost;
        public uint manaCostPerLevel;
        public uint manaPerSecond;
        public uint manaPerSecondPerLevel;
        public uint rangeIndex;
        public float speed;
        public uint modalNextSpell;
        public uint[] Totem = new uint[2];
        public uint[] Reagent = new uint[8];
        public uint[] ReagentCount = new uint[8];
        public uint EquippedItemClass;
        public uint EquippedItemSubClass;
        public uint[] Effect = new uint[3];
        public uint[] EffectDieSides = new uint[3];
        public uint[] EffectBaseDice = new uint[3];
        public uint[] EffectDicePerLevel = new uint[3];
        public float[] EffectRealPointsPerLevel = new float[3];
        public uint[] EffectBasePoints = new uint[3];
        public uint[] EffectImplicitTargetA = new uint[3];
        public uint[] EffectImplicitTargetB = new uint[3];
        public uint[] EffectRadiusIndex = new uint[3];
        public uint[] EffectAura = new uint[3];
        public uint[] EffectAuraPeriod = new uint[3];
        public float[] EffectAmplitude = new float[3];
        public uint[] EffectChainTarget = new uint[3];
        public uint[] EffectItemType = new uint[3];
        public uint[] EffectMiscValue = new uint[3];
        public uint[] EffectTriggerSpell = new uint[3];
        public uint SpellVisual;
        public uint SpellIconID;
        public uint activeIconID;
        public uint spellPriority;
        public string Name;
        public int[] NONE6 = new int[7];
        public uint NameFlags;
        public string Rank;
        public int[] NONE7 = new int[7];
        public uint RankFlags;
        public string Description;
        public int[] NONE8 = new int[7];
        public uint DescriptionFlags;
        public uint manaCostPct;
        public uint startRecoveryCategory;
        public uint startRecoveryTime;

        private int m_rank = 0;
        public int iRank
        {
            get
            {
                if (m_rank == 0)
                    Load();

                return m_rank;
            }
        }

        public Spell()
        {

        }

        private void Load()
        {
            Match match = Regex.Match(this.Rank, @"Rank\s+(\d+)", RegexOptions.Compiled);
            if (match.Success)
                int.TryParse(match.Groups[1].Value, out m_rank);
        }
    }
}
