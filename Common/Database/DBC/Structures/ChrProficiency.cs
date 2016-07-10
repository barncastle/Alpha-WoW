using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class ChrProficiency
    {
        public int m_ID;
        public int[] m_proficiency_minLevel = new int[16];
        public int[] m_proficiency_acquireMethod = new int[16];
        public int[] m_proficiency_itemClass = new int[16];
        public int[] m_proficiency_itemSubClassMask = new int[16];
    }
}
