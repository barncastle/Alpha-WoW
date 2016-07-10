using Common.Constants;
using Common.Database;
using Common.Helpers;
using Common.Helpers.Extensions;
using Common.Network.Packets;
using System;
using System.Linq;
using WorldServer.Storage;

namespace WorldServer.Game.Objects
{
    public abstract class WorldObject
    {
        [Key]
        public ulong Guid { get; set; }
        public ObjectTypes ObjectType = ObjectTypes.TYPE_OBJECT;

        public float WalkSpeed = 2.5f;
        public float RunningSpeed = 7.0f;
        public float SwimSpeed = 4.7222223f;
        public float TurnRate = 3.141593f;
        public uint MovementFlags = 0;
        public uint UnitFlags = 0;
        public uint DynamicFlags = 0;
        public byte ShapeShiftForm = 0;
        public uint BaseAttackTime = 1000;
        public uint DisplayID;
        public float Scale = 1f; // non-Tauren player/item scale as default, npc/go from database, pets from dbc
        public float BoundingRadius = 0f;

        public Vector Location = new Vector();
        public ulong TransportID = 0;
        public Vector Transport = new Vector();
        public Single TransportOrientation = 0;
        public Single Orientation = 0;
        public Single Pitch = 0;
        public uint Zone = 0;
        public uint Map = 0;
        public TReadOnly<string> Grid = new TReadOnly<string>("");

        public WorldObject() { }

        public bool IsTypeOf(ObjectTypes type)
        {
            return this.ObjectType.HasFlag(type);
        }

        public bool HasQuest(uint id)
        {
            uint entry = 0;

            //TODO other types of object
            if (IsTypeOf(ObjectTypes.TYPE_UNIT) && !IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                entry = ((Creature)this).Entry;
                return Database.CreatureQuests.ContainsKey(entry);
            }

            return false;
        }

        #region Packet Functions
        private ObjectTypeIds GetTypeId()
        {
            ObjectTypes[] types = (ObjectTypes[])Enum.GetValues(typeof(ObjectTypes));
            ObjectTypeIds[] ids = (ObjectTypeIds[])Enum.GetValues(typeof(ObjectTypeIds));

            for (int i = types.Length - 1; i > 0; i--)
                if (IsTypeOf(types[i]))
                    return ids[i];

            return ObjectTypeIds.TYPEID_OBJECT;
        }

        private byte GetUpdateMask()
        {
            uint mask = 0;
            if (IsTypeOf(ObjectTypes.TYPE_CONTAINER))
                mask += ((uint)ContainerFields.CONTAINER_END);
            if (IsTypeOf(ObjectTypes.TYPE_ITEM))
                mask += ((uint)ItemFields.ITEM_END);
            if (IsTypeOf(ObjectTypes.TYPE_PLAYER))
                mask += ((uint)PlayerFields.PLAYER_END);
            if (IsTypeOf(ObjectTypes.TYPE_UNIT))
                mask += ((uint)UnitFields.UNIT_END);
            if (IsTypeOf(ObjectTypes.TYPE_OBJECT))
                mask += ((uint)ObjectFields.OBJECT_END);
            if (IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
                mask += ((uint)GameObjectFields.GAMEOBJECT_END);
            return (byte)((mask + 31) / 32);
        }

        public PacketWriter CreateObject(bool isClient)
        {
            PacketWriter packet = new PacketWriter(Opcodes.SMSG_UPDATE_OBJECT);
            packet.WriteUInt32(1); //Number of transactions
            packet.WriteUInt8(2);
            packet.WriteUInt64(Guid);

            packet.WriteUInt8((byte)GetTypeId());

            packet.WriteUInt64(this.TransportID);              // TransportGuid
            packet.WriteFloat(this.Transport.X);               // TransportX
            packet.WriteFloat(this.Transport.Y);               // TransportY
            packet.WriteFloat(this.Transport.Z);               // TransportZ
            packet.WriteFloat(this.TransportOrientation);      // TransportW (TransportO)

            packet.WriteFloat(this.Location.X);
            packet.WriteFloat(this.Location.Y);
            packet.WriteFloat(this.Location.Z);
            packet.WriteFloat(this.Orientation);

            packet.WriteFloat(this.Pitch);
            packet.WriteUInt32(this.MovementFlags);
            packet.WriteUInt32(0); // FallTime?

            packet.WriteFloat(this.WalkSpeed);
            packet.WriteFloat(this.RunningSpeed);
            packet.WriteFloat(this.SwimSpeed);
            packet.WriteFloat(this.TurnRate);

            packet.WriteUInt32((uint)(isClient ? 1 : 0)); // Flags, 1 - Player, 0 - Bot
            packet.WriteUInt32((uint)(IsTypeOf(ObjectTypes.TYPE_PLAYER) ? 1 : 0)); // AttackCycle
            packet.WriteUInt32(0); // TimerId

            if (IsTypeOf(ObjectTypes.TYPE_UNIT)) // VictimGuid
                packet.WriteUInt64(((Unit)this).CombatTarget);
            else
                packet.WriteUInt64(0);

            packet.WriteUInt8(GetUpdateMask());
            for (int i = 0; i < GetUpdateMask(); i++)
                packet.WriteUInt32(uint.MaxValue);

            return packet;
        }

        public PacketWriter UpdateObject(ref UpdateClass uc)
        {
            PacketWriter packet = new PacketWriter(Opcodes.SMSG_UPDATE_OBJECT);
            packet.WriteUInt32(1); //Number of transactions
            packet.WriteUInt8(0);
            packet.WriteUInt64(Guid);
            packet.WriteUInt8(GetUpdateMask());
            packet.WriteBytes(uc.GetMask()); //Mask of updated fields
            uc.BuildPacket(ref packet, false); //Only appends updated values
            return packet;
        }

        public PacketWriter BuildDestroy()
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_DESTROY_OBJECT);
            pw.WriteUInt64(this.Guid);
            return pw;
        }

        public virtual PacketWriter BuildUpdate(UpdateTypes type = UpdateTypes.UPDATE_PARTIAL, bool self = false)
        {
            return null;
        }

        public virtual PacketWriter QueryDetails()
        {
            return null;
        }
        #endregion

        #region Update Functions
        public virtual void Update(long time)
        {
            if (IsTypeOf(ObjectTypes.TYPE_UNIT))
            {
                ((Unit)this).UpdateAttackTimer(AttackTypes.BASE_ATTACK);
                ((Unit)this).UpdateSpell();

                if (IsTypeOf(ObjectTypes.TYPE_PLAYER) && ((Player)this).Inventory.HasOffhandWeapon())
                    ((Player)this).UpdateAttackTimer(AttackTypes.OFFHAND_ATTACK);
            }
        }
        #endregion
    }
}
