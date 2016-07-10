using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class SpellRange
    {
        public int m_ID;
        public float m_rangeMin;
        public float m_rangeMax;
        public int m_flags;
        public string m_displayName_lang;
        public int[] NONE = new int[7];
        public int m_displayName_flag;
        public string m_displayNameShort_lang;
        public int[] NONE2 = new int[7];
        public int m_displayNameShort_flag;
    }
}
