using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Database.DBC.Structures
{
    public class Lock
    {
        public uint m_ID;
        public int[] m_Type = new int[4];
        public int[] m_Index = new int[4];
        public int[] m_Skill = new int[4];
        public int[] m_Action = new int[4];
    }
}
