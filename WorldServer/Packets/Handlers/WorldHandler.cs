using Common.Network.Packets;
using WorldServer.Network;
using WorldServer.Game.Objects;
using WorldServer.Storage;
using WorldServer.Game.Managers;
using Common.Constants;
using Common.Helpers;

namespace WorldServer.Packets.Handlers
{
    public class WorldHandler
    {

        public static void HandleNULL(ref PacketReader packet, ref WorldManager manager) { } //Prevents showing unhandled packets

        public static void HandleWorldTeleport(ref PacketReader packet, ref WorldManager manager)
        {
            packet.ReadUInt32();
            manager.Character.Teleport(packet.ReadUInt8(), packet.ReadQuaternion());
        }

        public static void HandleWorldTeleportAck(ref PacketReader packet, ref WorldManager manager)
        {
            HandleWorldPortAck(ref packet, ref manager);
        }

        public static void HandleWorldPortAck(ref PacketReader packet, ref WorldManager manager)
        {
            GridManager.Instance.SendSurrounding(manager.Character.BuildUpdate(), manager.Character);
            manager.Character.FindObjectsInRange(true);
        }

        public static void HandlePlayerLogin(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            manager.Character = Database.Players.TryGet(guid);
            manager.Character.Client = manager;
            manager.Character.IsOnline = true;

            MiscHandler.HandleLoginSetTimespeed(ref manager);
            /*
             * SMSG_ACTION_BUTTONS
             */
            manager.Character.SendInitialSpells();
            manager.Character.SendMOTD();
            manager.Character.PreLoad();

            manager.Send(manager.Character.QueryDetails(), true);
            manager.Send(manager.Character.BuildUpdate(UpdateTypes.UPDATE_FULL, true));

            manager.Character.Login();
            manager.Character.UpdateSurroundingPlayers();
        }

        public static void HandleLogoutRequest(ref PacketReader packet, ref WorldManager manager)
        {
            manager.Send(new PacketWriter(Opcodes.SMSG_LOGOUT_COMPLETE));
            manager.Character.Logout();
        }

    }
}
