using System;
using System.Security.Cryptography;
using System.Text;
using Common.Database;

namespace WorldServer.Game.Objects
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("ip")]
        public string IP { get; set; }
        [Column("gmlevel")]
        public byte GMLevel { get; set; }

        public void SetPassword(string plaintext)
        {
            this.Password = HashPassword(plaintext);
        }

        public bool ComparePassword(string plaintext)
        {
            return this.Password == HashPassword(plaintext);
        }

        private string HashPassword(string plaintext)
        {
            using (SHA512 sha = new SHA512Managed())
            {
                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(plaintext));
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}
