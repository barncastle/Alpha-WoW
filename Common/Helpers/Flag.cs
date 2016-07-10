namespace Common.Helpers
{
    public static class Flag
    {
        //Byte
        public static bool HasFlag(this byte value, byte flag)
        {
            return (value & flag) == flag;
        }

        //UInt
        public static bool HasFlag(this uint value, uint flag)
        {
            return (value & flag) == flag;
        }

        //Int32
        public static bool HasFlag(this int value, int flag)
        {
            return (value & flag) == flag;
        }

        //Byte
        public static void SetFlag(ref byte value, byte flag)
        {
            value |= flag;
        }

        public static void RemoveFlag(ref byte value, byte flag)
        {
            value &= (byte)(flag ^ 0xff);
        }

        //UInt
        public static void SetFlag(ref uint value, uint flag)
        {
            value |= flag;
        }

        public static void RemoveFlag(ref uint value, uint flag)
        {
            value &= ~flag;
        }

        //Int32
        public static void SetFlag(ref int value, int flag)
        {
            value |= flag;
        }

        public static void RemoveFlag(ref int value, int flag)
        {
            value &= ~flag;
        }
    }
}
