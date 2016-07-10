using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Network
{
    public class ByteConverter
    {
        public static uint ConvertToUInt32<T>(T[] items)
        {
            byte[] result = new byte[4];
            int pos = 0;
            foreach (T i in items)
            {
                if (i is byte)
                {
                    result[pos] = Convert.ToByte(i);
                    pos++;
                }
                else if (i is sbyte)
                {
                    byte[] tmp = BitConverter.GetBytes(Convert.ToSByte(i));
                    Buffer.BlockCopy(tmp, 0, result, pos, tmp.Length);
                    pos++;
                }
                else if (i is ushort)
                {
                    byte[] tmp = BitConverter.GetBytes(Convert.ToUInt16(i));
                    Buffer.BlockCopy(tmp, 0, result, pos, tmp.Length);
                    pos += 2;
                }
                else if (i is short)
                {
                    byte[] tmp = BitConverter.GetBytes(Convert.ToInt16(i));
                    Buffer.BlockCopy(tmp, 0, result, pos, tmp.Length);
                    pos += 2;
                }
                else if (i is int)
                {
                    byte[] tmp = BitConverter.GetBytes(Convert.ToInt32(i));
                    Buffer.BlockCopy(tmp, 0, result, pos, tmp.Length);
                    break; //Max length
                }
                else if (i is uint)
                    return Convert.ToUInt32(i); //Is the right type already
            }

            if (result.Length > 4)
                throw new Exception("ConvertToUInt32 : Too many bytes passed in");

            return BitConverter.ToUInt32(result, 0);
        }

        public static uint ConvertToUInt32(ushort val1, ushort val2)
        {
            byte[] result = new byte[4];

            byte[] tmp = BitConverter.GetBytes(val1);
            Buffer.BlockCopy(tmp, 0, result, 0, tmp.Length);

            tmp = BitConverter.GetBytes(val2);
            Buffer.BlockCopy(tmp, 0, result, 2, tmp.Length);

            return BitConverter.ToUInt32(result, 0);
        }

        public static uint ConvertToUInt32(byte val1, byte val2, byte val3, byte val4)
        {
            byte[] result = new[] { val1, val2, val3, val4 };
            return BitConverter.ToUInt32(result, 0);
        }
    }
}
