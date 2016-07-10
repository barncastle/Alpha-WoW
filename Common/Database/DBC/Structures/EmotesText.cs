using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class EmotesText
    {
        public int m_ID;
        public string m_name;
        public int m_emoteID;
        public int[] m_emoteText = new int[16];
    }
}
