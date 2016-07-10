using Common.Network.Packets;
using WorldServer.Network;
using System.Linq;
using Common.Constants;
using WorldServer.Game.Objects;
using WorldServer.Storage;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Game.Managers;
using System;
using Common.Helpers;

namespace WorldServer.Packets.Handlers
{
    public class CharHandler
    {
        private static void SetMasks(Player p)
        {
            Races[] races = (Races[])Enum.GetValues(typeof(Races));
            RaceMask[] rmasks = (RaceMask[])Enum.GetValues(typeof(RaceMask));
            Classes[] classes = (Classes[])Enum.GetValues(typeof(Classes));
            ClassMask[] cmasks = (ClassMask[])Enum.GetValues(typeof(ClassMask));

            for (int i = races.Length - 1; i >= 0; i--)
                if ((byte)races[i] == p.Race)
                    p.RaceMask = (byte)rmasks[i];

            for (int i = classes.Length - 1; i >= 0; i--)
                if ((byte)classes[i] == p.Class)
                    p.ClassMask = (byte)cmasks[i];
        }

        public static void HandleCharEnum(ref PacketReader packet, ref WorldManager manager)
        {
            var account = manager.Account;
            var result = Database.Players.GetAllByAccount(account.Id);

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_ENUM);
            writer.WriteUInt8((byte)result.Count);

            foreach (Player c in result)
            {
                c.PreLoad();

                writer.WriteUInt64(c.Guid);
                writer.WriteString(c.Name);

                writer.WriteUInt8(c.Race);
                writer.WriteUInt8(c.Class);
                writer.WriteUInt8(c.Gender);
                writer.WriteUInt8(c.Skin);
                writer.WriteUInt8(c.Face);
                writer.WriteUInt8(c.HairStyle);
                writer.WriteUInt8(c.HairColour);
                writer.WriteUInt8(c.FacialHair);
                writer.WriteUInt8(c.Level);

                writer.WriteUInt32(c.Zone);
                writer.WriteUInt32(c.Map);

                writer.WriteFloat(c.Location.X);
                writer.WriteFloat(c.Location.Y);
                writer.WriteFloat(c.Location.Z);

                writer.WriteUInt32(c.GuildGuid);
                writer.WriteUInt32(c.PetDisplayInfo);
                writer.WriteUInt32(c.PetLevel);
                writer.WriteUInt32(c.PetFamily);

                for (byte i = 0; i < (byte)InventoryTypes.WEAPONOFFHAND; i++) //Loop through inventory types
                {
                    if (i == (byte)InventoryTypes.CLOAK || i == (byte)InventoryTypes.FINGER) //Ignore non visible - Cloaks weren't visible this patch!
                        continue;

                    var item = c.Inventory.Backpack.Items.Values.FirstOrDefault(x => x.Type == (InventoryTypes)i);
                    writer.WriteUInt32(item?.Template?.DisplayID ?? 0); // DisplayId
                    writer.WriteUInt8((byte)(item?.Type ?? 0)); // InventoryType
                }
            }

            manager.Send(writer);
        }

        public static void HandleCharCreate(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = 1;
            if (Database.Players.Count() > 0)
                guid = Database.Players.Keys.Max() + 1;

            Player cha = new Player(guid);
            cha.Name = packet.ReadString();
            cha.Name = char.ToUpper(cha.Name[0]) + cha.Name.Substring(1).ToLower(); //Format to UCFirst

            cha.Race = packet.ReadByte();
            cha.Class = packet.ReadByte();
            cha.Gender = packet.ReadByte();
            cha.Skin = packet.ReadByte();
            cha.Face = packet.ReadByte();
            cha.HairStyle = packet.ReadByte();
            cha.HairColour = packet.ReadByte();
            cha.FacialHair = packet.ReadByte();
            packet.ReadByte();

            SetMasks(cha);

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_CREATE);
            if (Database.Players.TryGetName(cha.Name) != null)
            {
                writer.WriteUInt8((byte)CharCreate.CHAR_CREATE_NAME_IN_USE);
            }
            else
            {
                cha.AccountId = manager.Account.Id;
                cha.PreLoad();
                cha.Create();

                if (Database.Players.TryAdd(cha))
                    writer.WriteUInt8((byte)CharCreate.CHAR_CREATE_SUCCESS);
                else
                    writer.WriteUInt8((byte)CharCreate.CHAR_CREATE_FAILED);
            }

            manager.Send(writer);
        }

        public static void HandleCharDelete(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            Player character = Database.Players.TryGet(guid);
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_DELETE);

            if (Database.Players.TryRemove(guid))
                writer.WriteUInt8((byte)CharDelete.CHAR_DELETE_SUCCESS);
            else
                writer.WriteUInt8((byte)CharDelete.CHAR_DELETE_FAILED);

            foreach (Item item in Database.Items.Values.Where(x => x.Player == character.Guid).ToList())
                Database.Items.TryRemove(item);
            
            manager.Send(writer);
        }

        public static void HandleNameCache(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            Player character = (manager.Character != null ? manager.Character : Database.Players.TryGet(guid));
            if (character == null)
                return;
            
            manager.Send(character.QueryDetails());
        }

        public static void HandleWeaponSheathe(ref PacketReader packet, ref WorldManager manager)
        {
            byte mode = packet.ReadUInt8();
            manager.Character.SetSheath(mode);
            GridManager.Instance.SendSurrounding(manager.Character.BuildUpdate(), manager.Character);
        }

        public static void HandleSetTarget(ref PacketReader packet, ref WorldManager manager)
        {
            ulong Guid = packet.ReadUInt64();
            if (manager != null && manager.Character != null)
                manager.Character.CurrentTarget = Guid;
        }

        public static void HandleSetSelection(ref PacketReader packet, ref WorldManager manager)
        {
            ulong Guid = packet.ReadUInt64();

            if (Database.Creatures.ContainsKey(Guid))
                manager.Character.CurrentSelection = Guid;
        }

        public static void HandleAttackSwing(ref PacketReader packet, ref WorldManager manager)
        {
            ulong guid = packet.ReadUInt64();
            Creature enemy = Database.Creatures.TryGet(guid);
            Player c = manager.Character;

            if (enemy == null)
            {
                HandleAttackStop(ref manager);
                return;
            }

            //Check for friendly
            if (enemy.IsFriendlyTo(manager.Character))
            {
                HandleAttackStop(ref manager);
                return;
            }

            //Check enemy isn't dead
            if (enemy.IsDead)
            {
                HandleAttackStop(ref manager);
                return;
            }

            if (c.Attack(enemy, true))
            {
                //Attack start
                PacketWriter pw = new PacketWriter(Opcodes.SMSG_ATTACKSTART);
                pw.WriteUInt64(c.Guid);
                pw.WriteUInt64(enemy.Guid);
                manager.Send(pw);
            }
        }

        public static void HandleAttackStop(ref WorldManager manager)
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_ATTACKSTOP);
            pw.WriteUInt64(manager.Character.Guid);
            pw.WriteUInt64(manager.Character.CombatTarget);
            pw.WriteUInt32(0);
            manager.Send(pw);

            manager.Character.SwingError = 0;
            manager.Character.CombatTarget = 0;
            manager.Character.IsAttacking = false;
            manager.Character.InCombat = false;

            Flag.RemoveFlag(ref manager.Character.UnitFlags, (uint)UnitFlags.UNIT_FLAG_IN_COMBAT);
            GridManager.Instance.SendSurrounding(manager.Character.BuildUpdate(), manager.Character);
        }

        public static void HandleAttackStop(ref PacketReader packet, ref WorldManager manager)
        {
            HandleAttackStop(ref manager);
        }

        public static void HandleRepopRequest(ref PacketReader packet, ref WorldManager manager)
        {
            manager.Character.Respawn();
            GridManager.Instance.SendSurrounding(manager.Character.BuildUpdate(), manager.Character);
        }

        public static void HandleSetActionButtonOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            byte button = packet.ReadUInt8();
            ushort action = packet.ReadUInt16();
            byte misc = packet.ReadUInt8();
            byte type = packet.ReadUInt8();

            if (action == 0)
                manager.Character.RemoveActionButton(button);
            else
                manager.Character.AddActionButton(button, action, type, misc);
        }
    }
}
