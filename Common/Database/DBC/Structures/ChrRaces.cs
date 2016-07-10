using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class ChrRaces
    {
        public int m_ID;
        public int m_flags;
        public int m_factionID;
        public int m_MaleDisplayId;
        public int m_FemaleDisplayId;
        public string m_ClientPrefix;
        public float m_MountScale;
        public int m_BaseLanguage;
        public int m_creatureType;
        public int m_LoginEffectSpellID;
        public int m_CombatStunSpellID;
        public int m_ResSicknessSpellID;
        public int m_SplashSoundID;
        public int m_startingTaxiNodes;
        public string m_clientFileString;
        public int m_cinematicSequenceID;
        public string m_name_lang;
        public int[] NONE = new int[7];
        public int m_name_flag;
    }
}
