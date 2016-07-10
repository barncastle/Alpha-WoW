using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Reader
{
    public static class BinaryReaderExtensions
    {
        public static Dictionary<Type, Func<BinaryReader, object>> ReadValue = new Dictionary<Type, Func<BinaryReader, object>>()
        {
            {typeof(bool),   br => br.ReadBoolean()},
            {typeof(sbyte),  br => br.ReadSByte()},
            {typeof(byte),   br => br.ReadByte()},
            {typeof(short),  br => br.ReadInt16()},
            {typeof(ushort), br => br.ReadUInt16()},
            {typeof(int),    br => br.ReadInt32()},
            {typeof(uint),   br => br.ReadUInt32()},
            {typeof(float),  br => br.ReadSingle()},
            {typeof(long),   br => br.ReadInt64()},
            {typeof(ulong),  br => br.ReadUInt64()},
            {typeof(double), br => br.ReadDouble()},
        };

        public static T Read<T>(this BinaryReader br)
        {
            return (T)ReadValue[typeof(T)](br);
        }

        public static T Read<T>(PacketReader br)
        {
            return (T)ReadValue[typeof(T)](br);
        }
    }
}
