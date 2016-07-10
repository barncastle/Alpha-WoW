using Common.Database;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.PlayerExtensions;
using WorldServer.Game.Objects.PlayerExtensions.Quests;

namespace WorldServer.Game.Structs
{
    public class LootItem
    {
        [Key]
        [Column("entry")]
        public uint Entry { get; set; }

        [Column("item")]
        public uint Item { get; set; }

        [Column("chance")]
        public float Chance { get; set; }

        [Column("mincountorref")]
        public int MinCount { get; set; }

        [Column("maxcount")]
        public int MaxCount { get; set; }

        [Column("questitem")]
        public bool QuestItem { get; set; }

        [Column("groupid")]
        public int GroupId { get; set; }
    }

    public class LootObject
    {
        public bool IsQuestItem { get; set; }
        public Item Item { get; set; }
        public uint Count { get; set; }

        public bool Lootable(Player p)
        {
            return !this.IsQuestItem || p.CheckIsRequiredQuestItem(this.Item.Entry);
        }
    }

    public class VendorItem
    {
        [Key]
        [Column("entry")]
        public uint Entry { get; set; }
        [Column("item")]
        public uint Item { get; set; }
        [Column("maxcount")]
        public int MaxCount { get; set; }
        [Column("maxcount")]
        public int CurCount { get; set; }
        [Column("incrtime")]
        public float RespawnSeconds
        {
            get { return respawn; }
            set { respawn = value / 60f; }
        }

        public long UpdateTime { get; set; }

        private float respawn;        
    }

    public class VendorSpell
    {
        [Key]
        [Column("entry")]
        public uint Entry { get; set; }
        [Column("spell")]
        public uint SpellId { get; set; }
        [Column("spellcost")]
        public uint Cost { get; set; }
        [Column("spellpointcost")]
        public uint SpellPointCost { get; set; }
        [Column("reqskill")]
        public uint RequiredSkill { get; set; }
        [Column("reqskillvalue")]
        public uint RequiredSkillValue { get; set; }
        [Column("reqlevel")]
        public uint RequiredLevel { get; set; }
    }

    
}
