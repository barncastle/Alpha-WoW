using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class WorldSafeLocs
    {
        public int m_ID;
        public int m_continent;
        public float m_locX;
        public float m_locY;
        public float m_locZ;
        public string m_AreaName_lang;
        public int[] NONE = new int[7];
        public int m_AreaName_flag;
    }
}
