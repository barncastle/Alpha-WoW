using Common.Constants;
using Common.Database.DBC;
using Common.Database.DBC.Structures;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Game.Objects.PlayerExtensions
{
    public static class TalentExtension
    {
        public static readonly int[] TALENT_LINES = new[] { 222, 230, 231, 233, 234 };

        public static void SendTalentList(this Player p)
        {
            HashSet<int> nextTalent = new HashSet<int>();
            HashSet<TalentItem> talents = new HashSet<TalentItem>();

            foreach (SkillLineAbility ability in DBC.SkillLineAbility.Values)
            {
                TRAINER_SERVICE status = TRAINER_SERVICE.TRAINER_SERVICE_AVAILABLE;

                if (!TalentExtension.TALENT_LINES.Contains(ability.m_skillLine)) //Remove incorrect skill lines
                    continue;

                Spell spell = null;
                if (!DBC.Spell.TryGetValue((uint)ability.m_spell, out spell)) //Only use those with spells
                    continue;

                if (p.Talents.Contains(ability.m_ID)) //Already have ability
                {
                    if (ability.m_supercededBySpell > 0)
                        nextTalent.Add(ability.m_supercededBySpell); //Store next as possibly available

                    status = TRAINER_SERVICE.TRAINER_SERVICE_USED;
                }
                else
                {
                    if (nextTalent.Contains(ability.m_spell))
                        status = TRAINER_SERVICE.TRAINER_SERVICE_AVAILABLE; //Definitely available as in superceed list
                    else if (spell.iRank == 1)
                        status = TRAINER_SERVICE.TRAINER_SERVICE_AVAILABLE; //Definitely available as Rank 1
                    else
                        status = TRAINER_SERVICE.TRAINER_SERVICE_UNAVAILABLE; //Probably not available as fallen through
                }

                TalentItem ti = new TalentItem();
                ti.SpellId = (uint)ability.m_spell;
                ti.Status = (byte)status;
                ti.Cost = 0;
                ti.TalentPoints = 10;
                ti.RequiredLevel = (ushort)spell.baseLevel;
                ti.RequiredSkillLine = 0;
                ti.RequiredSkillRank = 0;
                ti.RequiredSkillStep = 0;
                ti.RequiredAbility = new ushort[] { 0, 0, 0 };
                ti.Usable = 0;
                ti.Enabled = 0;
                talents.Add(ti);
            }

            PacketWriter pk = new PacketWriter(Opcodes.SMSG_TRAINER_LIST);
            pk.WriteUInt64(p.Guid);
            pk.WriteUInt32((uint)TRAINER_TYPE.TRAINER_TYPE_TALENTS); //Type
            pk.WriteUInt32((uint)talents.Count); //Spell count
            foreach(TalentItem ti in talents)
            {
                pk.WriteUInt32(ti.SpellId);
                pk.WriteUInt8(ti.Status);
                pk.WriteUInt32(ti.Cost);
                pk.WriteUInt16(ti.TalentPoints);
                pk.WriteUInt16(ti.RequiredLevel);
                pk.WriteUInt32(ti.RequiredSkillLine);
                pk.WriteUInt32(ti.RequiredSkillRank);
                pk.WriteUInt32(ti.RequiredSkillStep);

                for (int i = 0; i < 3; i++)
                    pk.WriteUInt16(ti.RequiredAbility[i]);

                pk.WriteUInt32(ti.Usable);
                pk.WriteUInt8(ti.Enabled);
            }

            p.Client.Send(pk);
        }

        internal class TalentItem
        {
            public uint SpellId { get; set; }
            public byte Status { get; set; }
            public uint Cost { get; set; }
            public ushort TalentPoints { get; set; }
            public ushort RequiredLevel { get; set; }
            public uint RequiredSkillLine { get; set; }
            public uint RequiredSkillRank { get; set; }
            public uint RequiredSkillStep { get; set; }
            public ushort[] RequiredAbility = new ushort[] { 0, 0, 0 };
            public uint Usable { get; set; }
            public byte Enabled { get; set; }
        }
    }
}
