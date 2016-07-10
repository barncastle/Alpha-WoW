using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class FactionTemplate
    {
        public uint m_ID;
        public int m_faction;
        public int m_factionGroup;
        public int m_friendGroup;
        public int m_enemyGroup;
        public int[] m_enemies = new int[4];
        public int[] m_friend = new int[4];

        public bool IsFriendlyTo(FactionTemplate entry)
        {
            if(entry.m_faction > 0)
            {
                for (int i = 0; i < 4; ++i)
                    if (m_enemies[i] == entry.m_faction)
                        return false;
                for (int i = 0; i < 4; ++i)
                    if (m_friend[i] == entry.m_faction)
                        return true;
            }

            return (m_friendGroup & entry.m_factionGroup) == entry.m_factionGroup || 
                   (m_factionGroup & entry.m_friendGroup) == entry.m_friendGroup;
        }

        public bool IsEnemyTo(FactionTemplate entry)
        {
            if (entry.m_faction > 0)
            {
                for (int i = 0; i < 4; ++i)
                    if (m_enemies[i] == entry.m_faction)
                        return true;
                for (int i = 0; i < 4; ++i)
                    if (m_friend[i] == entry.m_faction)
                        return false;
            }

            return (m_enemyGroup & entry.m_factionGroup) == m_enemyGroup;
        }

        public bool NeutralToAll()
        {
            for (int i = 0; i < 4; ++i)
                if (m_enemies[i] != 0)
                    return false;

            return m_enemyGroup == 0 && m_friendGroup == 0;
        }
    }
}
