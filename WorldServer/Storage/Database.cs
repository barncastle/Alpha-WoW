using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game;
using WorldServer.Game.Objects;
using WorldServer.Game.Structs;

namespace WorldServer.Storage
{
    public static class Database
    {
        public static DbSet<uint, AreaTrigger> AreaTriggers;
        public static DbSet<uint, ClassLevelStat> ClassLevelStats;
        public static DbSet<uint, CreateActionButton> CreateActionButtons;
        public static DbSet<uint, CreatePlayerInfo> CreatePlayerInfo;
        public static DbSet<uint, CreateSkillInfo> CreateSkillInfo;
        public static DbSet<uint, LevelStatsInfo> LevelStatsInfo;
        public static GroupedDbSet<uint, List<LootItem>> PickPocketLoot;
        public static GroupedDbSet<uint, List<LootItem>> SkinningLoot;
        public static GroupedDbSet<uint, List<LootItem>> CreatureLoot;
        public static GroupedDbSet<uint, List<LootItem>> GameObjectLoot;
        public static DbSet<uint, QuestTemplate> QuestTemplates;
        public static DbSet<uint, CreatureModelInfo> CreatureModelInfo;
        public static GroupedDbSet<uint, List<CreatureQuest>> CreatureQuests;
        public static GroupedDbSet<uint, List<CreatureQuest>> CreatureInvolvedQuests;
        public static DbSet<uint, GameObjectTemplate> GameObjectTemplates;
        public static GroupedDbSet<uint, List<VendorItem>> VendorItems;
        public static GroupedDbSet<uint, List<VendorSpell>> VendorSpells;
        public static DbSet<uint, CreatureTemplate> CreatureTemplates;
        public static DbSet<uint, ItemTemplate> ItemTemplates;
        public static GroupedDbSet<ulong, List<SocialList>> SocialList;

        public static DbSet<uint, Account> Accounts;
        public static DbSet<ulong, Creature> Creatures;
        public static DbSet<ulong, GameObject> GameObjects;
        public static DbSet<ulong, Item> Items;
        public static DbSet<ulong, Player> Players;
        
        private const string QueryDefault = "SELECT * FROM ";
        private const string QueryLootTemplate = "SELECT entry, item, CASE WHEN chanceorquestchance < 0 then 1 else 0 end as questitem," +
                                                 "ABS(chanceorquestchance) AS chance, mincountorref, maxcount, groupid FROM ";

        public static void Initialize()
        {
            AreaTriggers = new DbSet<uint, AreaTrigger>();
            ClassLevelStats = new DbSet<uint, ClassLevelStat>();
            CreateActionButtons = new DbSet<uint, CreateActionButton>();
            CreatePlayerInfo = new DbSet<uint, CreatePlayerInfo>();
            CreateSkillInfo = new DbSet<uint, CreateSkillInfo>();
            LevelStatsInfo = new DbSet<uint, LevelStatsInfo>();
            PickPocketLoot = new GroupedDbSet<uint, List<LootItem>>(QueryLootTemplate + "pickpocketing_loot_template", "PickPocket Loot");
            SkinningLoot = new GroupedDbSet<uint, List<LootItem>>(QueryLootTemplate + "skinning_loot_template", "Skinning Loot");
            CreatureLoot = new GroupedDbSet<uint, List<LootItem>>(QueryLootTemplate + "creature_loot_template", "Creature Loot");
            GameObjectLoot = new GroupedDbSet<uint, List<LootItem>>(QueryLootTemplate + "gameobject_loot_template", "GameObject Loot");
            QuestTemplates = new DbSet<uint, QuestTemplate>();
            CreatureModelInfo = new DbSet<uint, CreatureModelInfo>();
            CreatureQuests = new GroupedDbSet<uint, List<CreatureQuest>>(QueryDefault + "creature_questrelation", "Creature Quest Relations");
            CreatureInvolvedQuests = new GroupedDbSet<uint, List<CreatureQuest>>(QueryDefault + "creature_involvedrelation", "Creature Involved Relations");
            GameObjectTemplates = new DbSet<uint, GameObjectTemplate>();
            VendorItems = new GroupedDbSet<uint, List<VendorItem>>(QueryDefault + "npc_vendor", "Vendor Items");
            VendorSpells = new GroupedDbSet<uint, List<VendorSpell>>(QueryDefault + "npc_trainer", "Vendor Spells");
            CreatureTemplates = new DbSet<uint, CreatureTemplate>();
            ItemTemplates = new DbSet<uint, ItemTemplate>();

            Accounts = new DbSet<uint, Account>(false, true);
            Creatures = new DbSet<ulong, Creature>(true);
            GameObjects = new DbSet<ulong, GameObject>(true);
            Items = new DbSet<ulong, Item>(true, true);
            Players = new DbSet<ulong, Player>(true, true);
            SocialList = new GroupedDbSet<ulong, List<SocialList>>(QueryDefault + "character_social", "Social Lists", true);
        }

        public static void SaveChanges()
        {
            Accounts.UpdateChanges();
            Items.UpdateChanges();
            Players.UpdateChanges();
        }
    }
}
