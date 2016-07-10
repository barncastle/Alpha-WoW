using Common.Constants;
using Common.Database.DBC;
using Common.Database.DBC.Structures;
using Common.Network.Packets;
using System.Collections.Generic;
using System.Linq;
using WorldServer.Game.Structs;
using WorldServer.Storage;

namespace WorldServer.Game.Objects.UnitExtensions
{
    public static class TalentExtension
    {
        public const int TALENT_SKILL_ID = 3;

        public static void SendPlaySpellVisual(this Player p, ulong guid, uint spellvisual)
        {
            PacketWriter pk = new PacketWriter(Opcodes.SMSG_PLAY_SPELL_VISUAL);
            pk.WriteUInt64(guid);
            pk.WriteUInt32(spellvisual);
            p.Client.Send(pk);
        }

        public static bool CanPurchaseSpell(this Player p, VendorSpell vendorspell)
        {
            if (vendorspell.RequiredSkill > 0)
            {
                if (!p.Skills.ContainsKey((ushort)vendorspell.RequiredSkill))
                    return false;
                else if (p.Skills[(ushort)vendorspell.RequiredSkill].m_skillRank < vendorspell.RequiredSkillValue)
                    return false;
            }

            if (vendorspell.RequiredLevel > 0 && p.Level < vendorspell.RequiredLevel)
                return false;
            if (p.Spells.ContainsKey(vendorspell.Entry))
                return false;
            if (p.Money < vendorspell.Cost)
                return false;

            return true;
        }

        public static void SendTalentList(this Player p)
        {
            HashSet<int> nextTalent = new HashSet<int>();
            HashSet<SpellListItem> talents = new HashSet<SpellListItem>();

            foreach (SkillLineAbility ability in DBC.SkillLineAbility.Values)
            {
                TrainerServices status = TrainerServices.TRAINER_SERVICE_AVAILABLE;
                SkillLine skillline = DBC.SkillLine[ability.m_skillLine];
                if (skillline.m_skillType != TALENT_SKILL_ID)
                    continue;

                Spell spell = null;
                if (!DBC.Spell.TryGetValue((uint)ability.m_spell, out spell)) //Only use those with spells
                    continue;

                if (p.Talents.Contains(ability.m_ID)) //Already have ability
                {
                    if (ability.m_supercededBySpell > 0)
                        nextTalent.Add(ability.m_supercededBySpell); //Store next as possibly available

                    status = TrainerServices.TRAINER_SERVICE_USED;
                }
                else
                {
                    if (nextTalent.Contains(ability.m_spell))
                        status = TrainerServices.TRAINER_SERVICE_AVAILABLE; //Definitely available as in superceed list
                    else if (spell.iRank == 1)
                        status = TrainerServices.TRAINER_SERVICE_AVAILABLE; //Definitely available as Rank 1
                    else
                        status = TrainerServices.TRAINER_SERVICE_UNAVAILABLE; //Probably not available as fallen through
                }

                SpellListItem ti = new SpellListItem();
                ti.SpellId = (uint)ability.m_spell;
                ti.Status = (byte)status;
                ti.TalentPoints = 10;
                ti.RequiredLevel = (byte)spell.baseLevel;
                ti.RequiredSkillLine = 0;
                ti.RequiredSkillRank = 0;
                ti.RequiredSkillStep = 0;
                ti.RequiredAbility = new uint[] { 0, 0, 0 };
                talents.Add(ti);
            }

            PacketWriter pk = new PacketWriter(Opcodes.SMSG_TRAINER_LIST);
            pk.WriteUInt64(p.Guid);
            pk.WriteUInt32((uint)TrainerTypes.TRAINER_TYPE_TALENTS); //Type
            pk.WriteUInt32((uint)talents.Count); //Spell count
            foreach (SpellListItem spell in talents)
                spell.BuildPacket(ref pk);

            p.Client.Send(pk);
        }

        public static void SendSpellList(this Creature c, Player p)
        {
            HashSet<SpellListItem> spells = new HashSet<SpellListItem>();
            foreach (var vendorspell in c.Template.VendorSpells.Values)
            {
                TrainerServices status = TrainerServices.TRAINER_SERVICE_AVAILABLE;

                Spell spell = null;
                if (!DBC.Spell.TryGetValue((uint)vendorspell.SpellId, out spell)) //Only use those with spells
                    continue;

                if (p.Spells.ContainsKey(vendorspell.SpellId))
                    status = TrainerServices.TRAINER_SERVICE_USED;
                else if (!p.CanPurchaseSpell(vendorspell))
                    status = TrainerServices.TRAINER_SERVICE_UNAVAILABLE;

                //Check existence
                //Check spell chain

                SpellListItem ti = new SpellListItem();
                ti.SpellId = (uint)vendorspell.SpellId;
                ti.Status = (byte)status;
                ti.Cost = vendorspell.Cost;
                ti.SkillPoints = (byte)vendorspell.SpellPointCost;
                ti.RequiredLevel = (byte)vendorspell.RequiredLevel;
                ti.RequiredSkillLine = vendorspell.RequiredSkill;
                ti.RequiredSkillRank = vendorspell.RequiredSkillValue;
                ti.RequiredSkillStep = 0;
                ti.RequiredAbility = new uint[] { 0, 0, 0 };
                spells.Add(ti);
            }

            PacketWriter pk = new PacketWriter(Opcodes.SMSG_TRAINER_LIST);
            pk.WriteUInt64(c.Guid);
            pk.WriteUInt32(c.Template.TrainerType); //Type
            pk.WriteUInt32((uint)spells.Count); //Spell count
            foreach (SpellListItem spell in spells)
                spell.BuildPacket(ref pk);

            p.Client.Send(pk);
        }

        public static void SendBuySpellSucceed(this Player p, ulong guid, uint spell)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_TRAINER_BUY_SUCCEEDED);
            pkt.WriteUInt64(guid);
            pkt.WriteUInt32(spell);
            p.Client.Send(pkt);
        }

        public class SpellListItem
        {
            public uint SpellId { get; set; }
            public byte Status { get; set; }
            public uint Cost { get; set; }
            public byte TalentPoints { get; set; }
            public byte SkillPoints { get; set; }
            public byte RequiredLevel { get; set; }
            public uint RequiredSkillLine { get; set; }
            public uint RequiredSkillRank { get; set; }
            public uint RequiredSkillStep { get; set; }
            public uint[] RequiredAbility = new uint[] { 0, 0, 0 };

            public void BuildPacket(ref PacketWriter pk)
            {
                pk.WriteUInt32(this.SpellId);
                pk.WriteUInt8(this.Status);
                pk.WriteUInt32(this.Cost);
                pk.WriteUInt8(this.TalentPoints);
                pk.WriteUInt8(this.SkillPoints);
                pk.WriteUInt8(this.RequiredLevel);
                pk.WriteUInt32(this.RequiredSkillLine);
                pk.WriteUInt32(this.RequiredSkillRank);
                pk.WriteUInt32(this.RequiredSkillStep);

                for (int i = 0; i < 3; i++)
                    pk.WriteUInt32(this.RequiredAbility[i]);
            }
        }
    }
}
