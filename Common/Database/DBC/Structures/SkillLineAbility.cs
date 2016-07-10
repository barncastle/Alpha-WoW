using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class SkillLineAbility
    {
        public int m_ID;
        public int m_skillLine;
        public int m_spell;
        public int m_raceMask;
        public int m_classMask;
        public int m_excludeRace;
        public int m_excludeClass;
        public int m_minSkillLineRank;
        public int m_supercededBySpell;
        public int m_trivialSkillLineRankHigh;
        public int m_trivialSkillLineRankLow;
        public int m_abandonable;
    }
}
