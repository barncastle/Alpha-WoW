using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Structures
{
    public class CharStartOutfit
    {
        public uint ID;
        public byte Race;
        public byte Class;
        public byte Sex;
        public byte OutfitID;
        public int[] m_ItemID = new int[12];
        public int[] m_DisplayItemID = new int[12];
        public int[] m_InventoryType = new int[12];

        public bool Match(byte _race, byte _class, byte _gender)
        {
            return this.Race == _race && this.Class == _class && this.Sex == _gender;
        }
    }
}
