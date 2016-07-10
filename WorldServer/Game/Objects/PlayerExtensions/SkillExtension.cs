using Common.Constants;
using Common.Database.DBC;
using Common.Database.DBC.Structures;
using Common.Network;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using WorldServer.Game.Structs;

namespace WorldServer.Game.Objects.PlayerExtensions.Skill
{
    public static class SkillExtension
    {
        public static bool AddSkill(this Player p, ushort skillId)
        {
            if (!p.Skills.ContainsKey(skillId))
            {
                SkillLine cbi = DBC.SkillLine[(int)skillId];
                ushort startValue = 1;

                if (cbi.m_categoryID == (int)SkillTypes.MAX_SKILL)
                    startValue = (ushort)cbi.m_maxRank;

                p.Skills.Add((ushort)cbi.m_ID, new MirrorSkillInfo((ushort)cbi.m_ID, startValue, (ushort)cbi.m_maxRank, cbi.m_displayName_lang));
                return true;
            }

            return false;
        }

        public static bool SetSkill(this Player p, ushort skillId, ushort curValue)
        {
            if (p.Skills.ContainsKey(skillId))
                return p.SetSkill(skillId, curValue, p.Skills[skillId].m_skillMaxRank);
            return false;
        }

        public static bool SetSkill(this Player p, ushort skillId, ushort curValue, ushort maxValue)
        {
            if (p.Skills.ContainsKey(skillId))
            {
                MirrorSkillInfo skill = p.Skills[skillId];
                skill.m_skillRank = curValue;
                skill.m_skillMaxRank = maxValue;
                p.Skills[skillId] = skill;
                return true;
            }
            else
                return p.AddSkill(skillId);
        }

        private static ushort GetMaxRank(Player p, MirrorSkillInfo skill)
        {
            //TODO secondary prof spell check

            SkillLine cbi = DBC.SkillLine[(int)skill.m_skillLineID];
            switch(cbi.m_skillType)
            {
                case 0: //Weapon, Defense, Spell
                    return (ushort)(p.Level * 5);
                case 4: //Language, Riding, Secondary Profs
                    if (cbi.m_categoryID == 1)
                        return (ushort)cbi.m_maxRank; //Language, Riding
                    else
                        return (ushort)((p.Level * 5) + 25); //Secondary Profs
            }

            return 0;
        }

        public static void BuildSkillUpdate(this Player p, ref UpdateClass uc)
        {
            int inc = 0;
            foreach (MirrorSkillInfo skill in p.Skills.Values)
            {
                uc.UpdateValue<uint>(PlayerFields.PLAYER_SKILL_INFO_1_1, ByteConverter.ConvertToUInt32(skill.m_skillLineID, skill.m_skillRank), inc * 3);
                uc.UpdateValue<uint>(PlayerFields.PLAYER_SKILL_INFO_1_1, ByteConverter.ConvertToUInt32(new object[] { GetMaxRank(p, skill), skill.m_skillModifier }), (inc * 3) + 1);
                uc.UpdateValue<uint>(PlayerFields.PLAYER_SKILL_INFO_1_1, ByteConverter.ConvertToUInt32(skill.m_skillStep, skill.m_padding), (inc * 3) + 2);
                inc++;
            }
        }

    }
}
