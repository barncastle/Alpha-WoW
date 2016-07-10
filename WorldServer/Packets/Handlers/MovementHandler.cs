using System;
using Common.Network.Packets;
using WorldServer.Network;
using WorldServer.Game.Objects;
using WorldServer.Game.Managers;
using Common.Helpers;
using WorldServer.Game.Objects.PlayerExtensions;
using System.Linq;
using WorldServer.Game.Objects.PlayerExtensions.Quests;

namespace WorldServer.Packets.Handlers
{
    public class MovementHandler
    {
        public static void HandleMovementStatus(ref PacketReader packet, ref WorldManager manager)
        {
            Player c = manager.Character;
            if (c == null)
                return;

            Vector prevLoc = manager.Character.Location;
            long pos = packet.BaseStream.Position; //Store position after header

            manager.Character.TransportID = packet.ReadUInt64(); ;
            manager.Character.Transport = packet.ReadVector();
            manager.Character.TransportOrientation = packet.ReadFloat();
            manager.Character.Location = packet.ReadVector();
            manager.Character.Orientation = packet.ReadFloat();
            manager.Character.Pitch = packet.ReadFloat();
            manager.Character.MovementFlags = packet.ReadUInt32();

            packet.BaseStream.Position = pos;
            PacketWriter movementStatus = new PacketWriter(packet.Opcode);
            movementStatus.WriteUInt64(manager.Character.Guid);
            movementStatus.WriteBytes(packet.ReadBytes((int)(packet.BaseStream.Length - pos)));
            GridManager.Instance.SendSurroundingNotMe(movementStatus, manager.Character);

            if (prevLoc != c.Location)
            {
                GridManager.Instance.UpdateObject(c);
                c.FindObjectsInRange();
            }
        }

        public static void HandleZoneUpdate(ref PacketReader packet, ref WorldManager manager)
        {
            uint zone = packet.ReadUInt32();
            Player c = manager.Character;
            if (c == null)
                return;

            c.Zone = zone;
            c.UpdateSurroundingQuestStatus();

            if (c.Group != null)
                c.Group.SendAllPartyStatus();
        }
    }
}
