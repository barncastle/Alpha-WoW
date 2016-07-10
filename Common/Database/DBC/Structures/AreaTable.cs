using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class AreaTable
    {
        public uint m_ID;
        public int m_AreaNumber;
        public int m_ContinentID;
        public int m_ParentAreaNum;
        public int m_AreaBit;
        public int m_flags;
        public int m_SoundProviderPref;
        public int m_SoundProviderPrefUnderwater;
        public int m_MIDIAmbience;
        public int m_MIDIAmbienceUnderwater;
        public int m_ZoneMusic;
        public int m_IntroSound;
        public int m_IntroPriority;
        public string[] m_AreaName_lang = new string[8];
        public int m_AreaName_flag;
    }
}
