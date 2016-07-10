using Common.Helpers;
using System;
using System.IO;
using System.Text;

namespace Common.Network.Packets
{
    public class PacketReader : BinaryReader
    {
        public Opcodes Opcode { get; set; }
        public ushort Size { get; set; }

        public PacketReader(byte[] data, bool worldPacket = true) : base(new MemoryStream(data))
        {
            // Packet header (0.5.3.3368): Size: 2 bytes + Cmd: 4 bytes
            if (worldPacket)
            {
                Size = (ushort)((this.ReadUInt16() / 0x100) - 4);
                Opcode = (Opcodes)this.ReadUInt32();
            }
        }

        public sbyte ReadInt8()
        {
            return base.ReadSByte();
        }

        public new short ReadInt16()
        {
            return base.ReadInt16();
        }

        public new int ReadInt32()
        {
            return base.ReadInt32();
        }

        public new long ReadInt64()
        {
            return base.ReadInt64();
        }

        public byte ReadUInt8()
        {
            return base.ReadByte();
        }

        public new ushort ReadUInt16()
        {
            return base.ReadUInt16();
        }

        public new uint ReadUInt32()
        {
            return base.ReadUInt32();
        }

        public new ulong ReadUInt64()
        {
            return base.ReadUInt64();
        }

        public float ReadFloat()
        {
            return base.ReadSingle();
        }

        public new double ReadDouble()
        {
            return base.ReadDouble();
        }

        public Vector ReadVector()
        {
            return new Vector(base.ReadSingle(), base.ReadSingle(), base.ReadSingle());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(base.ReadSingle(), base.ReadSingle(), base.ReadSingle(), base.ReadSingle());
        }

        public string ReadString(byte terminator = 0)
        {
            StringBuilder tmpString = new StringBuilder();
            char tmpChar = base.ReadChar();
            char tmpEndChar = Convert.ToChar(Encoding.UTF8.GetString(new byte[] { terminator }));

            while (tmpChar != tmpEndChar)
            {
                tmpString.Append(tmpChar);
                tmpChar = base.ReadChar();
            }

            return tmpString.ToString();
        }

        public new string ReadString()
        {
            return ReadString(0);
        }

        public new byte[] ReadBytes(int count)
        {
            return base.ReadBytes(count);
        }

        public string ReadStringFromBytes(int count)
        {
            byte[] stringArray = base.ReadBytes(count);
            Array.Reverse(stringArray);

            return Encoding.ASCII.GetString(stringArray);
        }

        public string ReadIPAddress()
        {
            byte[] ip = new byte[4];

            for (int i = 0; i < 4; ++i)
            {
                ip[i] = ReadUInt8();
            }

            return ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3];
        }

        public void SkipBytes(int count)
        {
            base.BaseStream.Position += count;
        }
    }
}
