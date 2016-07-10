using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class SkillLine
    {
        public int m_ID;
        public int m_raceMask;
        public int m_classMask;
        public int m_excludeRace;
        public int m_excludeClass;
        public int m_categoryID;
        public int m_skillType;
        public int m_minstringLevel;
        public int m_maxRank;
        public int m_abandonable;
        public string m_displayName_lang;
        public int[] NONE = new int[7];
        public int m_displayName_flag;
    }
}
