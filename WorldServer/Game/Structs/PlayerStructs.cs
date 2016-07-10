using Common.Constants;
using Common.Database;
using Common.Database.DBC;
using Common.Database.DBC.Structures;
using Common.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldServer.Game.Structs
{
    [Table("playercreateinfo")]
    public class CreatePlayerInfo
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("race")]
        public byte Race { get; set; }

        [Column("class")]
        public byte Class { get; set; }

        [Column("map")]
        public uint Map { get; set; }

        [Column("zone")]
        public uint Zone { get; set; }

        [Column("position_x")]
        public float X { get; set; }

        [Column("position_y")]
        public float Y { get; set; }

        [Column("position_z")]
        public float Z { get; set; }

        [Column("orientation")]
        public float O { get; set; }
    }

    [Table("playercreateinfo_skill")]
    public class CreateSkillInfo
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("race")]
        public byte Race { get; set; }

        [Column("class")]
        public byte Class { get; set; }

        [Column("skill")]
        public ushort SkillID { get; set; }

        [Column("skillmin")]
        public uint SkillMin { get; set; }

        [Column("skillmax")]
        public uint SkillMax { get; set; }
    }

    [Table("playercreateinfo_action")]
    public class CreateActionButton
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("race")]
        public byte Race { get; set; }

        [Column("class")]
        public byte Class { get; set; }

        [Column("button")]
        public uint Button { get; set; }

        [Column("action")]
        public uint Action { get; set; }

        [Column("type")]
        public uint Type { get; set; }
    }

    [Table("player_levelstats")]
    public class LevelStatsInfo
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("race")]
        public byte Race { get; set; }

        [Column("class")]
        public byte Class { get; set; }

        [Column("level")]
        public uint Level { get; set; }

        [Column("str")]
        public uint Str { get; set; }

        [Column("agi")]
        public uint Agi { get; set; }

        [Column("sta")]
        public uint Stam { get; set; }

        [Column("inte")]
        public uint Inte { get; set; }

        [Column("spi")]
        public uint Spi { get; set; }
    }

    [Table("player_classlevelstats")]
    public class ClassLevelStat
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("class")]
        public byte Class { get; set; }

        [Column("level")]
        public int Level { get; set; }

        [Column("basehp")]
        public int BaseHP { get; set; }

        [Column("basemana")]
        public int BaseMana { get; set; }
    }

    public struct GossipMenuItems
    {
        uint menu_id;
        uint id;
        byte option_icon;
        string option_text;
        uint option_id;
        uint npc_option_npcflag;
        int action_menu_id;
        uint action_poi_id;
        uint action_script_id;
        bool box_coded;
        string box_text;
        ushort conditionId;
    };

    public struct GossipMenus
    {
        uint entry;
        uint text_id;
        uint script_id;
        ushort conditionId;
    }

    [Table("npc_text")]
    public class UnitGossipText
    {
        [Key]
        [Column("id")]
        public ulong Unit_Guid;
        [ColumnList("text0_", 2)]
        public string[] Text_0;
        [ColumnList("text1_", 2)]
        public string[] Text_1; //[7]
        public uint[] Language; //[7]
        public float[] Probability; //[7]
    }

    public struct MirrorSkillInfo
    {
        public ushort m_skillLineID;
        public ushort m_skillRank;
        public ushort m_skillMaxRank;
        public short m_skillModifier;
        public ushort m_skillStep;
        public ushort m_padding;
        public string m_text;

        public MirrorSkillInfo(ushort ID, ushort Rank, ushort MaxRank, string Name)
        {
            m_skillLineID = ID;
            m_skillRank = Rank;
            m_skillMaxRank = MaxRank;
            m_skillModifier = 0;
            m_skillStep = 0;
            m_padding = 0;
            m_text = Name;
        }
    }

    public struct ActionButton
    {
        public ushort Action;
        public byte Misc;
        public ActionButtonTypes Type;

        public ActionButton(ushort action, byte misc, byte type)
        {
            this.Action = action;
            this.Misc = misc;
            this.Type = (ActionButtonTypes)type;
        }
    }

    [Table("character_social")]
    public class SocialList
    {
        [Key]
        [Column("guid")]
        public ulong Player { get; set; }
        [Column("friend")]
        public ulong Friend { get; set; }
        [Column("ignore")]
        public bool Ignore { get; set; }
    }

    public class PlayerSpell
    {
        public Spell Spell { get; set; }
        public long Cooldown { get; set; }

        public PlayerSpell(Spell spell)
        {
            this.Spell = spell;
        }

        public PlayerSpell(uint spellid)
        {
            this.Spell = DBC.Spell[spellid];
        }

    }
}
