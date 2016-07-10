using Common.Database.DBC;
using Common.Network.Packets;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.PlayerExtensions;
using WorldServer.Game.Structs;
using WorldServer.Game.Managers;
using WorldServer.Storage;
using Common.Constants;
using WorldServer.Game.Objects.PlayerExtensions.Skill;
using System;

namespace WorldServer.Game.Commands
{
    public class GameMasterCommands : CommandParser
    {
        public static void AddItem(Player player, string[] args)
        {
            bool success = false;
            uint itemid = Read<uint>(args, 0);
            uint amount = (args.Length > 1 ? Read<uint>(args, 1) : 1); //optional

            if (!Database.ItemTemplates.ContainsKey(itemid) || player == null)
                return;

            ItemTemplate template = Database.ItemTemplates.TryGet(itemid);
            
            if (amount <= template.MaxStackCount)
            {
                Item item = Database.ItemTemplates.CreateItemOrContainer(itemid);
                item.StackCount = amount;
                player.AddItem(item);
                success = true;
            }
            else
            {
                uint stackcount = amount;
                for (int i = 0; i < Math.Ceiling((float)amount / template.MaxStackCount); i++)
                {
                    uint amt = (stackcount >= template.MaxStackCount ? template.MaxStackCount : stackcount);

                    Item item = Database.ItemTemplates.CreateItemOrContainer(itemid);
                    item.StackCount = amt;
                    player.AddItem(item);
                    stackcount -= amt;
                }

                success = true;
            }

            if (success) //Success
                player.Dirty = true;
        }

        public static void AddSkill(Player player, string[] args)
        {
            ushort skillid = Read<ushort>(args, 0);

            if (!DBC.SkillLine.ContainsKey(skillid) ||
                 player == null ||
                 player?.Skills.ContainsKey(skillid) == true)
                return;

            if (player.AddSkill(skillid)) //Success
                player.Dirty = true;
        }

        public static void SetSkill(Player player, string[] args)
        {
            bool success = false;
            ushort skillid = Read<ushort>(args,0);
            ushort curvalue = Read<ushort>(args, 1);
            ushort maxvalue = (args.Length > 1 ? Read<ushort>(args, 2) : (ushort)0); //optional

            if (!DBC.SkillLine.ContainsKey(skillid) || player == null)
                return;

            if (args.Length > 3)
                success = player.SetSkill(skillid, curvalue, maxvalue);
            else
                success = player.SetSkill(skillid, curvalue);

            if (success) //Success
                player.Dirty = true;
        }

        public static void Kill(Player player, string[] args)
        {
            if (player.CurrentSelection == 0 || !Database.Creatures.ContainsKey(player.CurrentSelection))
                return;

            Creature target = Database.Creatures.TryGet(player.CurrentSelection);
            target.Attackers.TryAdd(player.Guid, player);
            target.Die(player);
            player.Dirty = true;
        }

        public static void SetLevel(Player player, string[] args)
        {
            byte level = Read<byte>(args, 0);
            if (level > 0 && level != player.Level)
            {
                if (level > Globals.MAX_LEVEL)
                    level = Globals.MAX_LEVEL;

                player.GiveLevel(level);

                GridManager.Instance.SendSurrounding(player.BuildUpdate(), player);
            }
        }

        public static void Kick(Player player, string[] args)
        {
            string playername = Read<string>(args, 0);
            Player target = Database.Players.TryGetName(playername);
            if (target != null)
                target.Kick();
        }

        public static void Money(Player player, string[] args)
        {
            int money = Read<int>(args, 0);
            uint moneyabs = (uint)Math.Abs(money);

            if (money < 0 && player.Money >= moneyabs)
                player.Money -= moneyabs;
            else if (money < 0 && player.Money < moneyabs)
                player.Money = 0;
            else
                player.Money += (uint)money;

            player.Dirty = true;
        }

        public static void SetPower(Player player, string[] args)
        {
            uint power = Read<uint>(args, 0);
            player.SetPowerValue(power, false);
            player.Dirty = true;
        }
    }
}
