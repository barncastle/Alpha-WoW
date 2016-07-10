using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Game.Structs
{
    [Table("creature_model_info")]
    public class CreatureModelInfo
    {
        [Key]
        [Column("modelid")]
        public uint ModelId { get; set; }
        [Column("bounding_radius")]
        public float BoundingRadius { get; set; }
        [Column("combat_reach")]
        public float CombatReach { get; set; }
        [Column("gender")]
        public byte Gender { get; set; }
    }

    public class CreatureQuest
    {
        [Key]
        [Column("entry")]
        public uint CreatureEntry { get; set; }
        [Column("quest")]
        public uint QuestEntry { get; set; }
    }
}
