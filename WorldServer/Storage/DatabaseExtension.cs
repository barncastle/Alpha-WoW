using Common.Constants;
using WorldServer.Game.Objects;
using WorldServer.Game.Structs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldServer.Storage
{
    public static class DatabaseExtension
    {
        public static Item CreateItemOrContainer(this DbSet<uint, ItemTemplate> ItemTemplates, uint Key)
        {
            ItemTemplate template = ItemTemplates.TryGet(Key);
            return (template?.InvType == (uint)InventoryTypes.BAG ? new Container(Key) : new Item(Key));
        }

        public static Account GetByName(this DbSet<uint, Account> Accounts, string name)
        {
            return Accounts.Values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static List<Player> GetAllByAccount(this DbSet<ulong, Player> Players, uint AccountId)
        {
            return Players.Values.Where(x => x.AccountId == AccountId).ToList();
        }

        public static Player TryGetName(this DbSet<ulong, Player> Players, string name)
        {
            return Players.Values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
