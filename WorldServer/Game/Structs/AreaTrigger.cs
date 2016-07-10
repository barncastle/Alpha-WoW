using Common.Database;
using Common.Helpers;

namespace WorldServer.Game.Structs
{
    [Table("areatrigger_teleport")]
    public class AreaTrigger
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("target_map")]
        public uint Map { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("required_level")]
        public int RequiredLevel { get; set; }

        [Column("target_position_x")]
        public float X { get; set; }

        [Column("target_position_y")]
        public float Y { get; set; }

        [Column("target_position_z")]
        public float Z { get; set; }

        [Column("target_orientation")]
        public float W { get; set; }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(X, Y, Z, W);
        }
    }
}
