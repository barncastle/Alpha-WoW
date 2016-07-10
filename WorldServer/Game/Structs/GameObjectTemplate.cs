using Common.Database;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Game.Structs
{
    [Table("gameobjects")]
    public class GameObjectTemplate
    {
        [Key]
        [Column("entry")]
        public uint Entry { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("type")]
        public uint Type { get; set; }
        [Column("displayid")]
        public uint DisplayId { get; set; }
        [Column("flags")]
        public uint Flags { get; set; }
        [Column("faction")]
        public uint Faction { get; set; }
        [Column("size")]
        public float Size { get; set; }
        [ColumnList("data", 10)]
        public uint[] RawData { get; set; }
    }
}
