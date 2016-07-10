using Common.Database;
using Common.Helpers.Extensions;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldServer.Storage;
using static WorldServer.Game.Objects.UnitExtensions.TalentExtension;

namespace WorldServer.Game.Structs
{
    [Table("creatures")]
    public class CreatureTemplate
    {
        [Key]
        [Column("entry")]
        public uint Entry { get; set; }
        [Column("modelid")]
        public uint ModelID { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("subname")]
        public string SubName { get; set; }
        [Column("npcflag")]
        public byte NPCFlags { get; set; }
        [Column("speed")]
        public float Speed { get; set; }
        [Column("scale")]
        public float Scale { get; set; }
        [Column("baseattacktime")]
        public int AttackTime { get; set; }
        [Column("unit_flags")]
        public uint UnitFlags { get; set; }
        [Column("dynamicflags")]
        public uint DynamicFlags { get; set; }
        [Column("type")]
        public uint CreatureType { get; set; }
        [Column("type_flags")]
        public uint CreatureTypeFlags { get; set; }
        [ColumnList("spell", 4)]
        public ulong[] Spells = new ulong[4];
        [Column("movementtype")]
        public bool MovementType { get; set; }
        [Column("equipment_id")]
        public int EquipmentID { get; set; }
        [Column("PetSpellDataId")]
        public uint PetSpellDataID { get; set; }
        [Column("rank")]
        public uint Rank { get; set; }
        [Column("family")]
        public uint Family { get; set; }
        [Column("faction_A")]
        public uint FactionA { get; set; }
        [Column("faction_H")]
        public uint FactionH { get; set; }
        [ColumnList(new[] { "minhealth", "maxhealth" })]
        public TStat Health { get; set; }
        [ColumnList(new[] { "minlevel", "maxlevel" })]
        public TRandom Level { get; set; }
        [ColumnList(new[] { "mingold", "maxgold" })]
        public TRandom Gold { get; set; }
        [ColumnList(new[] { "minmana", "maxmana" })]
        public TRandom Mana { get; set; }
        [ColumnList(new[] { "mindmg", "maxdmg" })]
        public TStat Damage { get; set; }

        [ColumnList(new[] { "armor", "resistance1" })]
        public TResistance Armor { get; set; }
        [Column("resistance2")]
        public TResistance Holy { get; set; }
        [Column("resistance3")]
        public TResistance Fire { get; set; }
        [Column("resistance4")]
        public TResistance Nature { get; set; }
        [Column("resistance5")]
        public TResistance Frost { get; set; }
        [Column("resistance6")]
        public TResistance Shadow { get; set; }        

        //Internal use only
        [Column("trainer_type")]
        public uint TrainerType { get; set; }
        [Column("trainer_spell")]
        public uint TrainerSpell { get; set; }
        [Column("class")]
        public uint TrainerClass { get; set; }
        [Column("race")]
        public uint TrainerRace { get; set; }

        public float BoundingRadius;
        public float CombatReach;
        public List<VendorItem> VendorItems;
        public Dictionary<uint, VendorSpell> VendorSpells;

        public PacketWriter QueryDetails()
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_CREATURE_QUERY_RESPONSE);
            pw.WriteUInt32(this.Entry);
            pw.WriteString(this.Name);

            for (int i = 0; i < 3; i++)
                pw.WriteString(this.Name); //Other names - never implemented

            pw.WriteString(this.SubName);
            pw.WriteUInt32(this.CreatureTypeFlags); //Creature Type i.e tameable
            pw.WriteUInt32(this.CreatureType);
            pw.WriteUInt32(this.Family);
            pw.WriteUInt32(this.Rank);
            pw.WriteUInt32(0);
            pw.WriteUInt32(this.PetSpellDataID);
            pw.WriteUInt32(this.ModelID);
            pw.WriteUInt16(0); //??
            return pw;
        }

        public void OnDbLoad()
        {
            CreatureModelInfo cmi = Database.CreatureModelInfo.TryGet(this.ModelID);
            if (cmi != null)
            {
                this.BoundingRadius = cmi.BoundingRadius;
                this.CombatReach = cmi.CombatReach;
            }
            
            this.VendorItems = Database.VendorItems.TryGet(this.Entry)?.ToList(); //Shallow copy
            this.VendorSpells = Database.VendorSpells.TryGet(this.Entry)?.ToDictionary(x => x.SpellId, y => y);
        }
    }
}
