using Common.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Network.Packets
{
    public class UpdateClass
    {
        private BitArray m_objectfields = new BitArray((int)ObjectFields.OBJECT_END, false);
        private Hashtable m_objectvalues = new Hashtable();
        private BitArray m_itemfields = new BitArray((int)ItemFields.ITEM_END, false);
        private Hashtable m_itemvalues = new Hashtable();
        private BitArray m_containerfields = new BitArray((int)ContainerFields.CONTAINER_END, false);
        private Hashtable m_containervalues = new Hashtable();
        private BitArray m_unitfields = new BitArray((int)UnitFields.UNIT_END, false);
        private Hashtable m_unitvalues = new Hashtable();
        private BitArray m_playerfields = new BitArray((int)PlayerFields.PLAYER_END, false);
        private Hashtable m_playervalues = new Hashtable();
        private BitArray m_gameobjectfields = new BitArray((int)GameObjectFields.GAMEOBJECT_END, false);
        private Hashtable m_gameobjectvalues = new Hashtable();
        private BitArray m_dynamicobjectfields = new BitArray((int)DynamicObjectFields.DYNAMICOBJECT_END, false);
        private Hashtable m_dynamicobjectvalues = new Hashtable();
        private Dictionary<OBJECT_TYPES, bool> m_touched = new Dictionary<OBJECT_TYPES, bool>();

        public UpdateClass()
        {
            Clear(); //Clear all fields just to be sure
        }

        /*
         * Value Writers
         */
        public void UpdateValue<T>(ObjectFields pos, T value, int increment = 0)
        {
            m_touched[OBJECT_TYPES.TYPE_OBJECT] = true;

            m_objectfields.Set((int)pos + increment, true);
            if (value is ulong)
            {
                m_objectfields.Set((int)pos + 1 + increment, true);
                m_objectvalues[(int)pos + increment] = (uint)(Convert.ToUInt64(value) & uint.MaxValue);
                m_objectvalues[(int)pos + 1 + increment] = (uint)(Convert.ToUInt64(value) >> 32);
            }
            else if (value is long)
            {
                m_objectfields.Set((int)pos + 1 + increment, true);
                m_objectvalues[(int)pos + increment] = (int)(Convert.ToInt64(value) & int.MaxValue);
                m_objectvalues[(int)pos + 1 + increment] = (int)(Convert.ToInt64(value) >> 32);
            }
            else
                m_objectvalues[(int)pos + increment] = value;
        }
        public void UpdateValue<T>(ItemFields pos, T value, int increment = 0)
        {
            m_touched[OBJECT_TYPES.TYPE_ITEM] = true;

            m_itemfields.Set((int)pos + increment, true);
            if (value is long || value is ulong) //64bit needs to store + append two 32 bits - obviously
            {
                m_itemfields.Set((int)pos + 1 + increment, true);
                m_itemvalues[(int)pos + increment] = Convert.ToInt32(Convert.ToInt64(value) & uint.MaxValue);
                m_itemvalues[(int)pos + 1 + increment] = Convert.ToInt32((Convert.ToInt64(value) >> 32) & uint.MaxValue);
            }
            else
                m_itemvalues[(int)pos + increment] = value;
        }
        public void UpdateValue<T>(ContainerFields pos, T value, int increment = 0)
        {
            m_touched[OBJECT_TYPES.TYPE_CONTAINER] = true;

            m_containerfields.Set((int)pos + increment, true);
            if (value is long || value is ulong) //64bit needs to store + append two 32 bits - obviously
            {
                m_containerfields.Set((int)pos + 1 + increment, true);
                m_containervalues[(int)pos + increment] = Convert.ToInt32(Convert.ToInt64(value) & uint.MaxValue);
                m_containervalues[(int)pos + 1 + increment] = Convert.ToInt32((Convert.ToInt64(value) >> 32) & uint.MaxValue);
            }
            else
                m_containervalues[(int)pos + increment] = value;
        }
        public void UpdateValue<T>(UnitFields pos, T value, int increment = 0)
        {
            m_touched[OBJECT_TYPES.TYPE_UNIT] = true;

            m_unitfields.Set((int)pos + increment, true);
            if (value is long || value is ulong)
            {
                m_unitfields.Set((int)pos + 1 + increment, true);
                m_unitvalues[(int)pos + increment] = Convert.ToInt32(Convert.ToInt64(value) & uint.MaxValue);
                m_unitvalues[(int)pos + 1 + increment] = Convert.ToInt32((Convert.ToInt64(value) >> 32) & uint.MaxValue);
            }
            else
                m_unitvalues[(int)pos + increment] = value;
        }
        public void UpdateValue<T>(PlayerFields pos, T value, int increment = 0)
        {
            m_touched[OBJECT_TYPES.TYPE_PLAYER] = true;

            m_playerfields.Set((int)pos + increment, true);
            if (value is long || value is ulong) //64bit needs to store + append two 32 bits - obviously
            {
                m_playerfields.Set((int)pos + 1 + increment, true);
                m_playervalues[(int)pos + increment] = Convert.ToInt32(Convert.ToInt64(value) & uint.MaxValue);
                m_playervalues[(int)pos + 1 + increment] = Convert.ToInt32((Convert.ToInt64(value) >> 32) & uint.MaxValue);
            }
            else
                m_playervalues[(int)pos + increment] = value;
        }
        public void UpdateValue<T>(GameObjectFields pos, T value, int increment = 0)
        {
            m_touched[OBJECT_TYPES.TYPE_GAMEOBJECT] = true;

            m_gameobjectfields.Set((int)pos + increment, true);
            if (value is long || value is ulong) //64bit needs to store + append two 32 bits - obviously
            {
                m_gameobjectfields.Set((int)pos + 1 + increment, true);
                m_gameobjectvalues[(int)pos + increment] = Convert.ToInt32(Convert.ToInt64(value) & uint.MaxValue);
                m_gameobjectvalues[(int)pos + 1 + increment] = Convert.ToInt32((Convert.ToInt64(value) >> 32) & uint.MaxValue);
            }
            else
                m_gameobjectvalues[(int)pos + increment] = value;
        }
        public void UpdateValue<T>(DynamicObjectFields pos, T value, int increment = 0)
        {
            m_touched[OBJECT_TYPES.TYPE_DYNAMICOBJECT] = true;

            m_dynamicobjectfields.Set((int)pos + increment, true);
            if (value is long || value is ulong) //64bit needs to store + append two 32 bits - obviously
            {
                m_dynamicobjectfields.Set((int)pos + 1 + increment, true);
                m_dynamicobjectvalues[(int)pos + increment] = Convert.ToInt32(Convert.ToInt64(value) & uint.MaxValue);
                m_dynamicobjectvalues[(int)pos + 1 + increment] = Convert.ToInt32((Convert.ToInt64(value) >> 32) & uint.MaxValue);
            }
            else
                m_dynamicobjectvalues[(int)pos + increment] = value;
        }

        public void BuildPacket(ref PacketWriter packet, bool create)
        {
            //Append the packets for each touched field type
            // - Order matters here so might need to reorder down the line
            if (isTouched(OBJECT_TYPES.TYPE_OBJECT))
                AppendPacket(ref m_objectfields, ref m_objectvalues, ref packet, create);
            if (isTouched(OBJECT_TYPES.TYPE_UNIT))
                AppendPacket(ref m_unitfields, ref m_unitvalues, ref packet, create);
            if (isTouched(OBJECT_TYPES.TYPE_PLAYER))
                AppendPacket(ref m_playerfields, ref m_playervalues, ref packet, create);
            if (isTouched(OBJECT_TYPES.TYPE_ITEM))
                AppendPacket(ref m_itemfields, ref m_itemvalues, ref packet, create);
            if (isTouched(OBJECT_TYPES.TYPE_CONTAINER))
                AppendPacket(ref m_containerfields, ref m_containervalues, ref packet, create);
            if (isTouched(OBJECT_TYPES.TYPE_GAMEOBJECT))
                AppendPacket(ref m_gameobjectfields, ref m_gameobjectvalues, ref packet, create);
            if (isTouched(OBJECT_TYPES.TYPE_DYNAMICOBJECT))
                AppendPacket(ref m_dynamicobjectfields, ref m_dynamicobjectvalues, ref packet, create);

            if (Opcodes.SMSG_UPDATE_OBJECT == packet.Opcode)
                packet.Compress();
        }

        public void Clear()
        {
            m_objectfields.SetAll(false);
            m_objectvalues.Clear();
            m_itemfields.SetAll(false);
            m_itemvalues.Clear();
            m_containerfields.SetAll(false);
            m_containervalues.Clear();
            m_unitfields.SetAll(false);
            m_unitvalues.Clear();
            m_playerfields.SetAll(false);
            m_playervalues.Clear();
            m_gameobjectfields.SetAll(false);
            m_gameobjectvalues.Clear();
            m_dynamicobjectfields.SetAll(false);
            m_dynamicobjectvalues.Clear();

            m_touched.Clear();
            foreach (OBJECT_TYPES type in Enum.GetValues(typeof(OBJECT_TYPES)))
                m_touched.Add(type, false);
        }

        public void Touch(OBJECT_TYPES type)
        {
            m_touched[type] = true;
        }

        public byte[] GetMask()
        {
            BitArray arr = new BitArray(0);
            int length = 0;

            if (isTouched(OBJECT_TYPES.TYPE_OBJECT))
            {
                arr = Append(arr, m_objectfields);
                length += (int)ObjectFields.OBJECT_END;
            }
            if (isTouched(OBJECT_TYPES.TYPE_UNIT))
            {
                arr = Append(arr, m_unitfields);
                length += (int)UnitFields.UNIT_END;
            }
            if (isTouched(OBJECT_TYPES.TYPE_PLAYER))
            {
                arr = Append(arr, m_playerfields);
                length += (int)PlayerFields.PLAYER_END;
            }
            if (isTouched(OBJECT_TYPES.TYPE_ITEM))
            {
                arr = Append(arr, m_itemfields);
                length += (int)ItemFields.ITEM_END;
            }
            if (isTouched(OBJECT_TYPES.TYPE_CONTAINER))
            {
                arr = Append(arr, m_containerfields);
                length += (int)ContainerFields.CONTAINER_END;
            }
            if (isTouched(OBJECT_TYPES.TYPE_GAMEOBJECT))
            {
                arr = Append(arr, m_gameobjectfields);
                length += (int)GameObjectFields.GAMEOBJECT_END;
            }
            if (isTouched(OBJECT_TYPES.TYPE_DYNAMICOBJECT))
            {
                arr = Append(arr, m_dynamicobjectfields);
                length += (int)DynamicObjectFields.DYNAMICOBJECT_END;
            }

            return BitArrayToByteArray(arr);
        }

        /*
         * Private Functions
         */
        private void AppendPacket(ref BitArray ba, ref Hashtable ht, ref PacketWriter pw, bool create = true)
        {
            for (int i = 0; i < ba.Length; i++) //Iterate each bit array, get hashtable values and append to the packet correctly
            {
                if (ba.Get(i) && ht[i] != null)
                {
                    var data = ht[i];
                    if (data is byte)
                        pw.WriteUInt8((byte)ht[i]);
                    else if (data is sbyte)
                        pw.WriteInt8((sbyte)ht[i]);
                    else if (data is UInt16)
                        pw.WriteUInt16((UInt16)ht[i]);
                    else if (data is short)
                        pw.WriteInt16((short)ht[i]);
                    else if (data is UInt32)
                        pw.WriteUInt32((UInt32)ht[i]);
                    else if (data is int)
                        pw.WriteInt32((int)ht[i]);
                    else if (data is UInt64)
                        pw.WriteUInt64((UInt64)ht[i]);
                    else if (data is Int64)
                        pw.WriteInt64((Int64)ht[i]);
                    else if (data is float)
                        pw.WriteFloat((float)ht[i]);
                    else if (data is string)
                        pw.WriteString((string)ht[i]);
                    else if (data is double)
                        pw.WriteDouble((double)ht[i]);
                    else if (data is byte[])
                        foreach (byte b in ((byte[])data)) //Adds a byte array as a list of uint8s
                            pw.WriteUInt8(b);
                }
                else if(create)
                    pw.WriteUInt32(0);
            }
        }

        private bool isTouched(OBJECT_TYPES type)
        {
            return m_touched[type];
        }

        private BitArray Append(BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        private byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
    }
}
