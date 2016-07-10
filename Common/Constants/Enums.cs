using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Constants
{
    public enum CharList
    {
        CHAR_LIST_RETRIEVING = 0x24,
        CHAR_LIST_RETRIEVED = 0x25,
        CHAR_LIST_FAILED = 0x26,
        LAST_CHAR_LIST_RESULT = 0x27,
    }

    public enum CharCreate
    {
        CHAR_CREATE_IN_PROGRESS = 0x27,
        CHAR_CREATE_SUCCESS = 0x28,
        CHAR_CREATE_ERROR = 0x29,
        CHAR_CREATE_FAILED = 0x2A,
        CHAR_CREATE_NAME_IN_USE = 0x2B,
        CHAR_CREATE_DISABLED = 0x2C,
        LAST_CHAR_CREATE_RESULT = 0x2D,
    }

    public enum CharDelete
    {
        CHAR_DELETE_IN_PROGRESS = 0x2D,
        CHAR_DELETE_SUCCESS = 0x2E,
        CHAR_DELETE_FAILED = 0x2F,
        LAST_CHAR_DELETE_RESULT = 0x30,
    }

    public enum CharLogin
    {
        CHAR_LOGIN_IN_PROGRESS = 0x30,
        CHAR_LOGIN_SUCCESS = 0x31,
        CHAR_LOGIN_NO_WORLD = 0x32,
        CHAR_LOGIN_DUPLICATE_CHARACTER = 0x33,
        CHAR_LOGIN_NO_INSTANCES = 0x34,
        CHAR_LOGIN_FAILED = 0x35,
        CHAR_LOGIN_DISABLED = 0x36,
        LAST_CHAR_LOGIN_RESULT = 0x37,
    }


    // PLAYER INFORMATION
    public enum Classes : byte
    {
        CLASS_WARRIOR = 1,
        CLASS_PALADIN = 2,
        CLASS_HUNTER = 3,
        CLASS_ROGUE = 4,
        CLASS_PRIEST = 5,
        CLASS_SHAMAN = 7,
        CLASS_MAGE = 8,
        CLASS_WARLOCK = 9,
        CLASS_DRUID = 11
    }

    public enum ClassMask : uint
    {
        CLASS_WARRIOR = 1 << 0,
        CLASS_PALADIN = 1 << 1,
        CLASS_HUNTER = 1 << 2,
        CLASS_ROGUE = 1 << 3,
        CLASS_PRIEST = 1 << 4,
        CLASS_SHAMAN = 1 << 6,
        CLASS_MAGE = 1 << 7,
        CLASS_WARLOCK = 1 << 8,
        CLASS_DRUID = 1 << 10
    }

    public enum Races : byte
    {
        RACE_HUMAN = 1,
        RACE_ORC = 2,
        RACE_DWARF = 3,
        RACE_NIGHT_ELF = 4,
        RACE_UNDEAD = 5,
        RACE_TAUREN = 6,
        RACE_GNOME = 7,
        RACE_TROLL = 8
    }

    public enum RaceMask : uint
    {
        RACE_HUMAN = 1 << 0,
        RACE_ORC = 1 << 1,
        RACE_DWARF = 1 << 2,
        RACE_NIGHT_ELF = 1 << 3,
        RACE_UNDEAD = 1 << 4,
        RACE_TAUREN = 1 << 5,
        RACE_GNOME = 1 << 6,
        RACE_TROLL = 1 << 7
    }

    public enum Genders : byte
    {
        GENDER_MALE = 0,
        GENDER_FEMALE = 1
    }

    public enum PowerTypes : uint
    {
        TYPE_MANA = 0,
        TYPE_RAGE = 1,
        TYPE_FOCUS = 2,
        TYPE_ENERGY = 3,
        TYPE_HAPPINESS = 4,
        POWER_HEALTH = 0xFFFFFFFE
    }

    public enum UnitFlags : uint
    {
        UNIT_FLAG_SHEATHE = 0x40000000,
        UNIT_FLAG_GHOST = 0x10000,
        UNIT_FLAG_SNEAK = 0x8000,
        UNIT_FLAG_DEAD = 0x4000,
        UNIT_FLAG_MOUNT = 0x2000,
        UNIT_FLAG_MOUNT_ICON = 0x1000,
        UNIT_FLAG_FLYING = 0x8,
        UNIT_FLAG_FROZEN = 0x4,
        UNIT_FLAG_FROZEN2 = 0x800000,
        UNIT_FLAG_FROZEN3 = 0x400000,
        UNIT_FLAG_FROZEN4 = 0x1,
        UNIT_FLAG_STANDARD = 0x0,
        UNIT_FLAG_PASSIVE = 0x200,
        UNIT_FLAG_IN_COMBAT = 0x80000,
        UNIT_FLAG_SKINNABLE = 0x4000000,
        UNIT_FLAG_LOOTING = 0x400,
    }

    public enum StandState
    {
        UNIT_STANDING = 0x0,
        UNIT_SITTING = 0x1,
        UNIT_SITTINGCHAIR = 0x2,
        UNIT_SLEEPING = 0x3,
        UNIT_SITTINGCHAIRLOW = 0x4,
        UNIT_FIRSTCHAIRSIT = 0x4,
        UNIT_SITTINGCHAIRMEDIUM = 0x5,
        UNIT_SITTINGCHAIRHIGH = 0x6,
        UNIT_LASTCHAIRSIT = 0x6,
        UNIT_DEAD = 0x7,
        UNIT_KNEEL = 0x8,
        UNIT_NUMSTANDSTATES = 0x9,
        UNIT_NUMCHAIRSTATES = 0x3,
    }

    public enum SheathState
    {
        SHEATH_STATE_UNARMED = 0,
        SHEATH_STATE_MELEE = 1,
        SHEATH_STATE_RANGED = 2
    }

    // ITEM INFORMATION
    public enum ItemClasses : byte
    {
        ITEM_CLASS_CONSUMABLE = 0,
        ITEM_CLASS_CONTAINER = 1,
        ITEM_CLASS_WEAPON = 2,
        ITEM_CLASS_JEWELRY = 3,
        ITEM_CLASS_ARMOR = 4,
        ITEM_CLASS_REAGENT = 5,
        ITEM_CLASS_PROJECTILE = 6,
        ITEM_CLASS_TRADE_GOODS = 7,
        ITEM_CLASS_GENERIC = 8,
        ITEM_CLASS_BOOK = 9,
        ITEM_CLASS_MONEY = 10,
        ITEM_CLASS_QUIVER = 11,
        ITEM_CLASS_QUEST = 12,
        ITEM_CLASS_KEY = 13,
        ITEM_CLASS_PERMANENT = 14,
        ITEM_CLASS_JUNK = 15
    }

    public enum ItemSubClasses : byte
    {
        // Consumable
        ITEM_SUBCLASS_CONSUMABLE = 0,
        ITEM_SUBCLASS_FOOD = 1,
        ITEM_SUBCLASS_LIQUID = 2,
        ITEM_SUBCLASS_POTION = 3,
        ITEM_SUBCLASS_SCROLL = 4,
        ITEM_SUBCLASS_BANDAGE = 5,
        ITEM_SUBCLASS_HEALTHSTONE = 6,
        ITEM_SUBCLASS_COMBAT_EFFECT = 7,

        // Container
        ITEM_SUBCLASS_BAG = 0,
        ITEM_SUBCLASS_SOUL_BAG = 1,
        ITEM_SUBCLASS_HERB_BAG = 2,
        ITEM_SUBCLASS_ENCHANTING_BAG = 3,

        // Weapon
        ITEM_SUBCLASS_AXE = 0,
        ITEM_SUBCLASS_TWOHAND_AXE = 1,
        ITEM_SUBCLASS_BOW = 2,
        ITEM_SUBCLASS_GUN = 3,
        ITEM_SUBCLASS_MACE = 4,
        ITEM_SUBCLASS_TWOHAND_MACE = 5,
        ITEM_SUBCLASS_POLEARM = 6,
        ITEM_SUBCLASS_SWORD = 7,
        ITEM_SUBCLASS_TWOHAND_SWORD = 8,
        ITEM_SUBCLASS_WEAPON_obsolete = 9,
        ITEM_SUBCLASS_STAFF = 10,
        ITEM_SUBCLASS_WEAPON_EXOTIC = 11,
        ITEM_SUBCLASS_WEAPON_EXOTIC2 = 12,
        ITEM_SUBCLASS_FIST_WEAPON = 13,
        ITEM_SUBCLASS_MISC_WEAPON = 14,
        ITEM_SUBCLASS_DAGGER = 15,
        ITEM_SUBCLASS_THROWN = 16,
        ITEM_SUBCLASS_SPEAR = 17,
        ITEM_SUBCLASS_CROSSBOW = 18,
        ITEM_SUBCLASS_WAND = 19,
        ITEM_SUBCLASS_FISHING_POLE = 20,

        // Armor
        ITEM_SUBCLASS_MISC = 0,
        ITEM_SUBCLASS_CLOTH = 1,
        ITEM_SUBCLASS_LEATHER = 2,
        ITEM_SUBCLASS_MAIL = 3,
        ITEM_SUBCLASS_PLATE = 4,
        ITEM_SUBCLASS_BUCKLER = 5,
        ITEM_SUBCLASS_SHIELD = 6,
        ITEM_SUBCLASS_LIBRAM = 7,
        ITEM_SUBCLASS_IDOL = 8,
        ITEM_SUBCLASS_TOTEM = 9,

        // Projectile
        ITEM_SUBCLASS_WAND_obslete = 0,
        ITEM_SUBCLASS_BOLT_obslete = 1,
        ITEM_SUBCLASS_ARROW = 2,
        ITEM_SUBCLASS_BULLET = 3,
        ITEM_SUBCLASS_THROWN_obslete = 4,

        // Trade goods
        ITEM_SUBCLASS_TRADE_GOODS = 0,
        ITEM_SUBCLASS_PARTS = 1,
        ITEM_SUBCLASS_EXPLOSIVES = 2,
        ITEM_SUBCLASS_DEVICES = 3,
        ITEM_SUBCLASS_GEMS = 4,
        ITEM_SUBCLASS_CLOTHS = 5,
        ITEM_SUBCLASS_LEATHERS = 6,
        ITEM_SUBCLASS_METAL_AND_STONE = 7,
        ITEM_SUBCLASS_MEAT = 8,
        ITEM_SUBCLASS_HERB = 9,
        ITEM_SUBCLASS_ELEMENTAL = 10,
        ITEM_SUBCLASS_OTHERS = 11,
        ITEM_SUBCLASS_ENCHANTANTS = 12,
        ITEM_SUBCLASS_MATERIALS = 13,

        // Recipe
        ITEM_SUBCLASS_BOOK = 0,
        ITEM_SUBCLASS_LEATHERWORKING = 1,
        ITEM_SUBCLASS_TAILORING = 2,
        ITEM_SUBCLASS_ENGINEERING = 3,
        ITEM_SUBCLASS_BLACKSMITHING = 4,
        ITEM_SUBCLASS_COOKING = 5,
        ITEM_SUBCLASS_ALCHEMY = 6,
        ITEM_SUBCLASS_FIRST_AID = 7,
        ITEM_SUBCLASS_ENCHANTING = 8,
        ITEM_SUBCLASS_FISHING = 9,
        ITEM_SUBCLASS_JEWELCRAFTING = 10,

        // Quiver
        ITEM_SUBCLASS_QUIVER0_obslete = 0,
        ITEM_SUBCLASS_QUIVER1_obslete = 1,
        ITEM_SUBCLASS_QUIVER = 2,
        ITEM_SUBCLASS_AMMO_POUCH = 3,

        // Keys
        ITEM_SUBCLASS_KEY = 0,
        ITEM_SUBCLASS_LOCKPICK = 1,

        // Misc
        ITEM_SUBCLASS_JUNK = 0,
        ITEM_SUBCLASS_REAGENT = 1,
        ITEM_SUBCLASS_PET = 2,
        ITEM_SUBCLASS_HOLIDAY = 3,
        ITEM_SUBCLASS_OTHER = 4,
        ITEM_SUBCLASS_MOUNT = 5
    }

    public enum InventoryTypes : byte
    {
        NONE_EQUIP = 0x00,
        HEAD = 0x01,
        NECK = 0x02,
        SHOULDER = 0x03,
        BODY = 0x04,
        CHEST = 0x05,
        WAIST = 0x06,
        LEGS = 0x07,
        FEET = 0x08,
        WRIST = 0x09,
        HAND = 0x0A,
        FINGER = 0x0B,
        TRINKET = 0x0C,
        WEAPON = 0x0D,
        SHIELD = 0x0E,
        RANGED = 0x0F,
        CLOAK = 0x10,
        TWOHANDEDWEAPON = 0x11,
        BAG = 0x12,
        TABARD = 0x13,
        ROBE = 0x14,
        WEAPONMAINHAND = 0x15,
        WEAPONOFFHAND = 0x16,
        HOLDABLE = 0x17,
        AMMO = 0x18,
        THROWN = 0x19,
        RANGEDRIGHT = 0x1A,
        NUM_TYPES = 0x1B
    }

    public enum InventorySlots : int
    {
        SLOT_HEAD = 0,
        SLOT_NECK = 1,
        SLOT_SHOULDERS = 2,
        SLOT_SHIRT = 3,
        SLOT_CHEST = 4,
        SLOT_WAIST = 5,
        SLOT_LEGS = 6,
        SLOT_FEET = 7,
        SLOT_WRISTS = 8,
        SLOT_HANDS = 9,
        SLOT_FINGERL = 10,
        SLOT_FINGERR = 11,
        SLOT_TRINKETL = 12,
        SLOT_TRINKETR = 13,
        SLOT_BACK = 14,
        SLOT_MAINHAND = 15,
        SLOT_OFFHAND = 16,
        SLOT_RANGED = 17,
        SLOT_TABARD = 18,

        //! Misc Types
        SLOT_BAG1 = 19,
        SLOT_BAG2 = 20,
        SLOT_BAG3 = 21,
        SLOT_BAG4 = 22,
        SLOT_INBACKPACK = 23,

        SLOT_ITEM_START = 23,
        SLOT_ITEM_END = 39,

        SLOT_BANK_ITEM_START = 39,
        SLOT_BANK_ITEM_END = 63,
        SLOT_BANK_BAG_1 = 63,
        SLOT_BANK_BAG_2 = 64,
        SLOT_BANK_BAG_3 = 65,
        SLOT_BANK_BAG_4 = 66,
        SLOT_BANK_BAG_5 = 67,
        SLOT_BANK_BAG_6 = 68,
        SLOT_BANK_END = 69
    }

    public enum InventoryStats : int
    {
        HEALTH = 0,
        POWER = 1, //Mana, energy etc
        AGILITY = 3,
        STRENGTH = 4,
        INTELLECT = 5,
        SPIRIT = 6,
        STAMINA = 7
    }

    public enum InventoryError : byte
    {
        EQUIP_ERR_OK = 0,
        EQUIP_ERR_CANT_EQUIP_LEVEL_I = 1,       // ERR_CANT_EQUIP_LEVEL_I
        EQUIP_ERR_CANT_EQUIP_SKILL = 2,       // ERR_CANT_EQUIP_SKILL
        EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT = 3,       // ERR_WRONG_SLOT
        EQUIP_ERR_BAG_FULL = 4,       // ERR_BAG_FULL
        EQUIP_ERR_NONEMPTY_BAG_OVER_OTHER_BAG = 5,       // ERR_BAG_IN_BAG
        EQUIP_ERR_CANT_TRADE_EQUIP_BAGS = 6,       // ERR_TRADE_EQUIPPED_BAG
        EQUIP_ERR_ONLY_AMMO_CAN_GO_HERE = 7,       // ERR_AMMO_ONLY
        EQUIP_ERR_NO_REQUIRED_PROFICIENCY = 8,       // ERR_PROFICIENCY_NEEDED
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE = 9,       // ERR_NO_SLOT_AVAILABLE
        EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM = 10,      // ERR_CANT_EQUIP_EVER
        EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM2 = 11,      // ERR_CANT_EQUIP_EVER
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE2 = 12,      // ERR_NO_SLOT_AVAILABLE
        EQUIP_ERR_CANT_EQUIP_WITH_TWOHANDED = 13,      // ERR_2HANDED_EQUIPPED
        EQUIP_ERR_CANT_DUAL_WIELD = 14,      // ERR_2HSKILLNOTFOUND
        EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG = 15,      // ERR_WRONG_BAG_TYPE
        EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG2 = 16,      // ERR_WRONG_BAG_TYPE
        EQUIP_ERR_CANT_CARRY_MORE_OF_THIS = 17,      // ERR_ITEM_MAX_COUNT
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE3 = 18,      // ERR_NO_SLOT_AVAILABLE
        EQUIP_ERR_ITEM_CANT_STACK = 19,      // ERR_CANT_STACK
        EQUIP_ERR_ITEM_CANT_BE_EQUIPPED = 20,      // ERR_NOT_EQUIPPABLE
        EQUIP_ERR_ITEMS_CANT_BE_SWAPPED = 21,      // ERR_CANT_SWAP
        EQUIP_ERR_SLOT_IS_EMPTY = 22,      // ERR_SLOT_EMPTY
        EQUIP_ERR_ITEM_NOT_FOUND = 23,      // ERR_ITEM_NOT_FOUND
        EQUIP_ERR_CANT_DROP_SOULBOUND = 24,      // ERR_DROP_BOUND_ITEM
        EQUIP_ERR_OUT_OF_RANGE = 25,      // ERR_OUT_OF_RANGE
        EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT = 26,      // ERR_TOO_FEW_TO_SPLIT
        EQUIP_ERR_COULDNT_SPLIT_ITEMS = 27,      // ERR_SPLIT_FAILED
        EQUIP_ERR_MISSING_REAGENT = 28,      // ERR_SPELL_FAILED_REAGENTS_GENERIC
        EQUIP_ERR_NOT_ENOUGH_MONEY = 29,      // ERR_NOT_ENOUGH_MONEY
        EQUIP_ERR_NOT_A_BAG = 30,      // ERR_NOT_A_BAG
        EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS = 31,      // ERR_DESTROY_NONEMPTY_BAG
        EQUIP_ERR_DONT_OWN_THAT_ITEM = 32,      // ERR_NOT_OWNER
        EQUIP_ERR_CAN_EQUIP_ONLY1_QUIVER = 33,      // ERR_ONLY_ONE_QUIVER
        EQUIP_ERR_MUST_PURCHASE_THAT_BAG_SLOT = 34,      // ERR_NO_BANK_SLOT
        EQUIP_ERR_TOO_FAR_AWAY_FROM_BANK = 35,      // ERR_NO_BANK_HERE
        EQUIP_ERR_ITEM_LOCKED = 36,      // ERR_ITEM_LOCKED
        EQUIP_ERR_YOU_ARE_STUNNED = 37,      // ERR_GENERIC_STUNNED
        EQUIP_ERR_YOU_ARE_DEAD = 38,      // ERR_PLAYER_DEAD
        EQUIP_ERR_CANT_DO_RIGHT_NOW = 39,      // ERR_CLIENT_LOCKED_OUT
        EQUIP_ERR_INT_BAG_ERROR = 40,      // ERR_INTERNAL_BAG_ERROR
        EQUIP_ERR_CAN_EQUIP_ONLY1_BOLT = 41,      // ERR_ONLY_ONE_BOLT
        EQUIP_ERR_CAN_EQUIP_ONLY1_AMMOPOUCH = 42,      // ERR_ONLY_ONE_AMMO
        EQUIP_ERR_STACKABLE_CANT_BE_WRAPPED = 43,      // ERR_CANT_WRAP_STACKABLE
        EQUIP_ERR_EQUIPPED_CANT_BE_WRAPPED = 44,      // ERR_CANT_WRAP_EQUIPPED
        EQUIP_ERR_WRAPPED_CANT_BE_WRAPPED = 45,      // ERR_CANT_WRAP_WRAPPED
        EQUIP_ERR_BOUND_CANT_BE_WRAPPED = 46,      // ERR_CANT_WRAP_BOUND
        EQUIP_ERR_UNIQUE_CANT_BE_WRAPPED = 47,      // ERR_CANT_WRAP_UNIQUE
        EQUIP_ERR_BAGS_CANT_BE_WRAPPED = 48,      // ERR_CANT_WRAP_BAGS
        EQUIP_ERR_ALREADY_LOOTED = 49,      // ERR_LOOT_GONE
        EQUIP_ERR_INVENTORY_FULL = 50,      // ERR_INV_FULL
        EQUIP_ERR_BANK_FULL = 51,      // ERR_BAG_FULL
        EQUIP_ERR_ITEM_IS_CURRENTLY_SOLD_OUT = 52,      // ERR_VENDOR_SOLD_OUT
        EQUIP_ERR_BAG_FULL3 = 53,      // ERR_BAG_FULL
        EQUIP_ERR_ITEM_NOT_FOUND2 = 54,      // ERR_ITEM_NOT_FOUND
        EQUIP_ERR_ITEM_CANT_STACK2 = 55,      // ERR_CANT_STACK
        EQUIP_ERR_BAG_FULL4 = 56,      // ERR_BAG_FULL
        EQUIP_ERR_ITEM_SOLD_OUT = 57,      // ERR_VENDOR_SOLD_OUT
        EQUIP_ERR_OBJECT_IS_BUSY = 58,      // ERR_OBJECT_IS_BUSY
        EQUIP_ERR_NONE = 59,      // ERR_CANT_BE_DISENCHANTED
        EQUIP_ERR_NOT_IN_COMBAT = 60,      // ERR_NOT_IN_COMBAT
        EQUIP_ERR_NOT_WHILE_DISARMED = 61,      // ERR_NOT_WHILE_DISARMED
        EQUIP_ERR_BAG_FULL6 = 62,      // ERR_BAG_FULL
        EQUIP_ERR_CANT_EQUIP_RANK = 63,      // ERR_CANT_EQUIP_RANK
        EQUIP_ERR_CANT_EQUIP_REPUTATION = 64,      // ERR_CANT_EQUIP_REPUTATION
        EQUIP_ERR_TOO_MANY_SPECIAL_BAGS = 65,      // ERR_TOO_MANY_SPECIAL_BAGS
        EQUIP_ERR_LOOT_CANT_LOOT_THAT_NOW = 66,      // ERR_LOOT_CANT_LOOT_THAT_NOW
    }

    //OBJECT INFORMATION
    public enum ObjectTypes : ushort
    {
        TYPE_OBJECT = 1,
        TYPE_ITEM = 2,
        TYPE_CONTAINER = 6,
        TYPE_UNIT = 8,
        TYPE_PLAYER = 16,
        TYPE_GAMEOBJECT = 32,
        TYPE_DYNAMICOBJECT = 64,
        TYPE_CORPSE = 128,
        TYPE_AIGROUP = 256,
        TYPE_AREATRIGGER = 512
    };

    public enum ObjectTypeIds : byte
    {
        TYPEID_OBJECT = 0,
        TYPEID_ITEM = 1,
        TYPEID_CONTAINER = 2,
        TYPEID_UNIT = 3,
        TYPEID_PLAYER = 4,
        TYPEID_GAMEOBJECT = 5,
        TYPEID_DYNAMICOBJECT = 6,
        TYPEID_CORPSE = 7
    };

    public enum UpdateTypes : byte
    {
        UPDATE_PARTIAL = 0,
        //  1 byte  - MASK
        //  8 bytes - GUID
        //  Goto Update Block
        UPDATE_MOVEMENT = 1,
        //  1 byte  - MASK
        //  8 bytes - GUID
        //  Goto Position Update
        UPDATE_FULL = 2,
        //  1 byte  - MASK
        //  8 bytes - GUID
        //  1 byte - Object Type (*)
        //  Goto Position Update
        //  Goto Update Block
        UPDATE_OUT_OF_RANGE = 3,
        //  4 bytes - Count
        //  Loop Count Times:
        //  1 byte  - MASK
        //  8 bytes - GUID
        UPDATE_IN_RANGE = 4
    }

    public enum HIGH_GUID : uint
    {
        HIGHGUID_ITEM = 0x40000000,
        HIGHGUID_CONTAINER = 0x40000000,
        HIGHGUID_UNIT = 0xF0001000,
        HIGHGUID_PLAYER = 0x00000000,
        HIGHGUID_GAMEOBJECT = 0xF0007000,
        HIGHGUID_DYNAMICOBJECT = 0xF000A000,
        HIGHGUID_CORPSE = 0xF0007000
    }

    public enum Factions
    {
        ALLIANCE = 4,
        HORDE = 6
    }

    public enum NpcFlags
    {
        NPC_FLAG_NONE = 0x0,
        NPC_FLAG_GOSSIP = 0x1,
        NPC_FLAG_QUESTGIVER = 0x2,
        NPC_FLAG_VENDOR = 0x3,
        NPC_FLAG_FLIGHTMASTER = 0x4,
        NPC_FLAG_TRAINER = 0x8,
        NPC_FLAG_BINDER = 0x10,       //Appears to be similar to inn keeper?
        NPC_FLAG_BANKER = 0x20,
        NPC_FLAG_TABARDDESIGNER = 0x40,
        NPC_FLAG_PETITIONER = 0x80,

        NPC_FLAG_SPIRITHEALER = 0x00000020,
        NPC_FLAG_SPIRITGUIDE = 0x00000040,
        NPC_FLAG_INNKEEPER = 0x00000080,
    }

    //COMBAT INFORMATION
    public enum HitInfo
    {
        NORMALSWING = 0x00000000,
        UNK1 = 0x00000001,               // req correct packet structure
        AFFECTS_VICTIM = 0x00000002,
        OFFHAND = 0x00000004,
        UNK2 = 0x00000008,
        MISS = 0x00000010,
        FULL_ABSORB = 0x00000020,
        PARTIAL_ABSORB = 0x00000040,
        FULL_RESIST = 0x00000080,
        PARTIAL_RESIST = 0x00000100,
        CRITICALHIT = 0x00000200,               // critical hit
        BLOCK = 0x00002000,               // blocked damage
        GLANCING = 0x00010000,
        CRUSHING = 0x00020000,
        NO_ANIMATION = 0x00040000,
        SWINGNOHITSOUND = 0x00200000,               // unused?
        RAGE_GAIN = 0x00800000
    }

    public enum VictimStates
    {
        VS_NONE = 0, // set when attacker misses
        VS_WOUND = 1, // victim got clear/blocked hit
        VS_DODGE = 2,
        VS_PARRY = 3,
        VS_INTERRUPT = 4,
        VS_BLOCK = 5, // unused? not set when blocked, even on full block
        VS_EVADE = 6,
        VS_IMMUNE = 7,
        VS_DEFLECT = 8
    }

    public enum ProcFlags
    {
        NONE = 0x00000000,

        KILLED = 0x00000001,    // 00 Killed by agressor - not sure about this flag
        KILL = 0x00000002,    // 01 Kill target (in most cases need XP/Honor reward)

        DONE_MELEE_AUTO_ATTACK = 0x00000004,    // 02 Done melee auto attack
        TAKEN_MELEE_AUTO_ATTACK = 0x00000008,    // 03 Taken melee auto attack

        DONE_SPELL_MELEE_DMG_CLASS = 0x00000010,    // 04 Done attack by Spell that has dmg class melee
        TAKEN_SPELL_MELEE_DMG_CLASS = 0x00000020,    // 05 Taken attack by Spell that has dmg class melee

        DONE_RANGED_AUTO_ATTACK = 0x00000040,    // 06 Done ranged auto attack
        TAKEN_RANGED_AUTO_ATTACK = 0x00000080,    // 07 Taken ranged auto attack

        DONE_SPELL_RANGED_DMG_CLASS = 0x00000100,    // 08 Done attack by Spell that has dmg class ranged
        TAKEN_SPELL_RANGED_DMG_CLASS = 0x00000200,    // 09 Taken attack by Spell that has dmg class ranged

        DONE_SPELL_NONE_DMG_CLASS_POS = 0x00000400,    // 10 Done positive spell that has dmg class none
        TAKEN_SPELL_NONE_DMG_CLASS_POS = 0x00000800,    // 11 Taken positive spell that has dmg class none

        DONE_SPELL_NONE_DMG_CLASS_NEG = 0x00001000,    // 12 Done negative spell that has dmg class none
        TAKEN_SPELL_NONE_DMG_CLASS_NEG = 0x00002000,    // 13 Taken negative spell that has dmg class none

        DONE_SPELL_MAGIC_DMG_CLASS_POS = 0x00004000,    // 14 Done positive spell that has dmg class magic
        TAKEN_SPELL_MAGIC_DMG_CLASS_POS = 0x00008000,    // 15 Taken positive spell that has dmg class magic

        DONE_SPELL_MAGIC_DMG_CLASS_NEG = 0x00010000,    // 16 Done negative spell that has dmg class magic
        TAKEN_SPELL_MAGIC_DMG_CLASS_NEG = 0x00020000,    // 17 Taken negative spell that has dmg class magic

        DONE_PERIODIC = 0x00040000,    // 18 Successful do periodic (damage / healing)
        TAKEN_PERIODIC = 0x00080000,    // 19 Taken spell periodic (damage / healing)

        TAKEN_DAMAGE = 0x00100000,    // 20 Taken any damage
        DONE_TRAP_ACTIVATION = 0x00200000,    // 21 On trap activation (possibly needs name change to ON_GAMEOBJECT_CAST or USE)

        DONE_MAINHAND_ATTACK = 0x00400000,    // 22 Done main-hand melee attacks (spell and autoattack)
        DONE_OFFHAND_ATTACK = 0x00800000,    // 23 Done off-hand melee attacks (spell and autoattack)

        DEATH = 0x01000000,    // 24 Died in any way

        // flag masks
        AUTO_ATTACK_MASK = DONE_MELEE_AUTO_ATTACK | TAKEN_MELEE_AUTO_ATTACK
                                                    | DONE_RANGED_AUTO_ATTACK | TAKEN_RANGED_AUTO_ATTACK,

        MELEE_MASK = DONE_MELEE_AUTO_ATTACK | TAKEN_MELEE_AUTO_ATTACK
                                                    | DONE_SPELL_MELEE_DMG_CLASS | TAKEN_SPELL_MELEE_DMG_CLASS
                                                    | DONE_MAINHAND_ATTACK | DONE_OFFHAND_ATTACK,

        RANGED_MASK = DONE_RANGED_AUTO_ATTACK | TAKEN_RANGED_AUTO_ATTACK
                                                    | DONE_SPELL_RANGED_DMG_CLASS | TAKEN_SPELL_RANGED_DMG_CLASS,

        SPELL_MASK = DONE_SPELL_MELEE_DMG_CLASS | TAKEN_SPELL_MELEE_DMG_CLASS
                                                    | DONE_SPELL_RANGED_DMG_CLASS | TAKEN_SPELL_RANGED_DMG_CLASS
                                                    | DONE_SPELL_NONE_DMG_CLASS_POS | TAKEN_SPELL_NONE_DMG_CLASS_POS
                                                    | DONE_SPELL_NONE_DMG_CLASS_NEG | TAKEN_SPELL_NONE_DMG_CLASS_NEG
                                                    | DONE_SPELL_MAGIC_DMG_CLASS_POS | TAKEN_SPELL_MAGIC_DMG_CLASS_POS
                                                    | DONE_SPELL_MAGIC_DMG_CLASS_NEG | TAKEN_SPELL_MAGIC_DMG_CLASS_NEG,

        SPELL_CAST_MASK = SPELL_MASK | DONE_TRAP_ACTIVATION | RANGED_MASK,

        PERIODIC_MASK = DONE_PERIODIC | TAKEN_PERIODIC,

        DONE_HIT_MASK = DONE_MELEE_AUTO_ATTACK | DONE_RANGED_AUTO_ATTACK
                                                     | DONE_SPELL_MELEE_DMG_CLASS | DONE_SPELL_RANGED_DMG_CLASS
                                                     | DONE_SPELL_NONE_DMG_CLASS_POS | DONE_SPELL_NONE_DMG_CLASS_NEG
                                                     | DONE_SPELL_MAGIC_DMG_CLASS_POS | DONE_SPELL_MAGIC_DMG_CLASS_NEG
                                                     | DONE_PERIODIC | DONE_MAINHAND_ATTACK | DONE_OFFHAND_ATTACK,

        TAKEN_HIT_MASK = TAKEN_MELEE_AUTO_ATTACK | TAKEN_RANGED_AUTO_ATTACK
                                                     | TAKEN_SPELL_MELEE_DMG_CLASS | TAKEN_SPELL_RANGED_DMG_CLASS
                                                     | TAKEN_SPELL_NONE_DMG_CLASS_POS | TAKEN_SPELL_NONE_DMG_CLASS_NEG
                                                     | TAKEN_SPELL_MAGIC_DMG_CLASS_POS | TAKEN_SPELL_MAGIC_DMG_CLASS_NEG
                                                     | TAKEN_PERIODIC | TAKEN_DAMAGE,

        REQ_SPELL_PHASE_MASK = SPELL_MASK & DONE_HIT_MASK
    }

    public enum ProcFlagsExLegacy
    {
        NONE = 0x0000000,                 // If none can tigger on Hit/Crit only (passive spells MUST defined by SpellFamily flag)
        NORMAL_HIT = 0x0000001,                 // If set only from normal hit (only damage spells)
        CRITICAL_HIT = 0x0000002,
        MISS = 0x0000004,
        RESIST = 0x0000008,
        DODGE = 0x0000010,
        PARRY = 0x0000020,
        BLOCK = 0x0000040,
        EVADE = 0x0000080,
        IMMUNE = 0x0000100,
        DEFLECT = 0x0000200,
        ABSORB = 0x0000400,
        REFLECT = 0x0000800,
        INTERRUPT = 0x0001000,                 // Melee hit result can be Interrupt (not used)
        FULL_BLOCK = 0x0002000,                 // block al attack damage
        RESERVED2 = 0x0004000,
        NOT_ACTIVE_SPELL = 0x0008000,                 // Spell mustn't do damage/heal to proc
        EX_TRIGGER_ALWAYS = 0x0010000,                 // If set trigger always no matter of hit result
        EX_ONE_TIME_TRIGGER = 0x0020000,                 // If set trigger always but only one time (not implemented yet)
        ONLY_ACTIVE_SPELL = 0x0040000,                 // Spell has to do damage/heal to proc

        // Flags for internal use - do not use these in db!
        INTERNAL_CANT_PROC = 0x0800000,
        INTERNAL_DOT = 0x1000000,
        INTERNAL_HOT = 0x2000000,
        INTERNAL_TRIGGERED = 0x4000000,
        INTERNAL_REQ_FAMILY = 0x8000000
    }

    public enum AttackTypes
    {
        BASE_ATTACK = 0,
        OFFHAND_ATTACK = 1
    }

    public enum TradeSkillCategories
    {
        TRADESKILL_OPTIMAL = 0x0,
        TRADESKILL_MEDIUM = 0x1,
        TRADESKILL_EASY = 0x2,
        TRADESKILL_TRIVIAL = 0x3,
        NUM_TRADESKILL_CATEGORIES = 0x4,
    }

    public enum TradeStatuses
    {
        TRADE_STATUS_PLAYER_BUSY = 0x0,
        TRADE_STATUS_PROPOSED = 0x1,
        TRADE_STATUS_INITIATED = 0x2,
        TRADE_STATUS_CANCELLED = 0x3,
        TRADE_STATUS_ACCEPTED = 0x4,
        TRADE_STATUS_ALREADY_TRADING = 0x5,
        TRADE_STATUS_PLAYER_NOT_FOUND = 0x6,
        TRADE_STATUS_STATE_CHANGED = 0x7,
        TRADE_STATUS_COMPLETE = 0x8,
        TRADE_STATUS_UNACCEPTED = 0x9,
        TRADE_STATUS_TOO_FAR_AWAY = 0xA,
        TRADE_STATUS_WRONG_FACTION = 0xB,
        TRADE_STATUS_FAILED = 0xC,
        TRADE_STATUS_DEAD = 0xD,
        TRADE_STATUS_PETITION = 0xE,
        TRADE_STATUS_PLAYER_IGNORED = 0xF,
    }

    public enum CraftLevelCategories
    {
        CRAFT_NONE = 0x0,
        CRAFT_OPTIMAL = 0x1,
        CRAFT_MEDIUM = 0x2,
        CRAFT_EASY = 0x3,
        CRAFT_TRIVIAL = 0x4,
        NUM_CRAFT_CATEGORIES = 0x5,
    }

    public enum TrainerServices
    {
        TRAINER_SERVICE_AVAILABLE = 0x0,
        TRAINER_SERVICE_UNAVAILABLE = 0x1,
        TRAINER_SERVICE_USED = 0x2,
        TRAINER_SERVICE_NOT_SHOWN = 0x3,
        TRAINER_SERVICE_NEVER = 0x4,
        TRAINER_SERVICE_NO_PET = 0x5,
        NUM_TRAINER_SERVICE_TYPES = 0x6,
    }

    public enum TrainerTypes
    {
        TRAINER_TYPE_GENERAL = 0x0,
        TRAINER_TYPE_TALENTS = 0x1,
        TRAINER_TYPE_TRADESKILLS = 0x2,
        TRAINER_TYPE_PET = 0x3,
    }

    public enum UnitDynamicTypes
    {
        UNIT_DYNAMIC_NONE = 0x0000,
        UNIT_DYNAMIC_LOOTABLE = 0x0001,
        UNIT_DYNAMIC_TRACK_UNIT = 0x0002,
        UNIT_DYNAMIC_TAPPED = 0x0004,       // Lua_UnitIsTapped - Indicates the target as grey for the client.
        UNIT_DYNAMIC_ROOTED = 0x0008,
        UNIT_DYNAMIC_SPECIALINFO = 0x0010,
        UNIT_DYNAMIC_DEAD = 0x0020,
    }

    public enum LootTypes
    {
        LOOT_TYPE_NOTALLOWED = 0,
        LOOT_TYPE_CORPSE = 1,
        LOOT_TYPE_SKINNING = 2,
        LOOT_TYPE_FISHING = 3
    }

    public enum QuestStatuses
    {
        QUEST_STATUS_NONE = 0,
        QUEST_STATUS_COMPLETE = 1,
        QUEST_STATUS_UNAVAILABLE = 2,
        QUEST_STATUS_INCOMPLETE = 3,
        QUEST_STATUS_AVAILABLE = 4,
        QUEST_STATUS_FAILED = 5,
        MAX_QUEST_STATUS
    };

    public enum QuestFailedReasons
    {
        INVALIDREASON_DONT_HAVE_REQ = 0,
        INVALIDREASON_QUEST_FAILED_LOW_LEVEL = 1,        // You are not high enough level for that quest.
        INVALIDREASON_QUEST_FAILED_WRONG_RACE = 6,        // That quest is not available to your race.
        INVALIDREASON_QUEST_ONLY_ONE_TIMED = 12,       // You can only be on one timed quest at a time.
        INVALIDREASON_QUEST_ALREADY_ON = 13,       // You are already on that quest
        INVALIDREASON_QUEST_FAILED_MISSING_ITEMS = 21,       // You don't have the required items with you. Check storage.
        INVALIDREASON_QUEST_FAILED_NOT_ENOUGH_MONEY = 23,       // You don't have enough money for that quest.
    };

    public enum QuestGiverStatuses
    {
        QUEST_GIVER_NONE = 0x0,
        QUEST_GIVER_TRIVIAL = 0x1,
        QUEST_GIVER_FUTURE = 0x2,
        QUEST_GIVER_REWARD = 0x3,
        QUEST_GIVER_QUEST = 0x4,
        QUEST_GIVER_NUMITEMS = 0x5
    };

    public enum SkillTypes
    {
        MAX_SKILL = 1, //These are always max when added i.e. language/riding
        WEAPON_SKILL = 2,
        CLASS_SKILL = 3,
        SECONDARY_SKILL = 4
    };

    public enum GameObjectTypes
    {
        TYPE_DOOR = 0x0,
        TYPE_BUTTON = 0x1,
        TYPE_QUESTGIVER = 0x2,
        TYPE_CHEST = 0x3,
        TYPE_BINDER = 0x4,
        TYPE_GENERIC = 0x5,
        TYPE_TRAP = 0x6,
        TYPE_CHAIR = 0x7,
        TYPE_SPELL_FOCUS = 0x8,
        TYPE_TEXT = 0x9,
        TYPE_GOOBER = 0xA,
        TYPE_TRANSPORT = 0xB,
        TYPE_AREADAMAGE = 0xC,
        TYPE_CAMERA = 0xD,
        TYPE_MAP_OBJECT = 0xE,
        TYPE_MO_TRANSPORT = 0xF,
        TYPE_DUEL_ARBITER = 0x10,
        TYPE_FISHINGNODE = 0x11,
        TYPE_RITUAL = 0x12,
        NUM_GAMEOBJECT_TYPE = 0x13,
    }

    public enum GameObjectStates
    {
        GO_STATE_ACTIVE = 0,                       // show in world as used and not reset (closed door open)
        GO_STATE_READY = 1,                        // show in world as ready (closed door close)
        GO_STATE_ACTIVE_ALTERNATIVE = 2            // show in world as used in alt way and not reset (closed door open by cannon fire)
    }

    public enum Emotes
    {
        NONE = 0,
        AGREE = 1,
        AMAZE = 2,
        ANGRY = 3,
        APOLOGIZE = 4,
        APPLAUD = 5,
        BASHFUL = 6,
        BECKON = 7,
        BEG = 8,
        BITE = 9,
        BLEED = 10,
        BLINK = 11,
        BLUSH = 12,
        BONK = 13,
        BORED = 14,
        BOUNCE = 15,
        BRB = 16,
        BOW = 17,
        BURP = 18,
        BYE = 19,
        CACKLE = 20,
        CHEER = 21,
        CHICKEN = 22,
        CHUCKLE = 23,
        CLAP = 24,
        CONFUSED = 25,
        CONGRATULATE = 26,
        COUGH = 27,
        COWER = 28,
        CRACK = 29,
        CRINGE = 30,
        CRY = 31,
        CURIOUS = 32,
        CURTSEY = 33,
        DANCE = 34,
        DRINK = 35,
        DROOL = 36,
        EAT = 37,
        EYE = 38,
        FART = 39,
        FIDGET = 40,
        FLEX = 41,
        FROWN = 42,
        GASP = 43,
        GAZE = 44,
        GIGGLE = 45,
        GLARE = 46,
        GLOAT = 47,
        GREET = 48,
        GRIN = 49,
        GROAN = 50,
        GROVEL = 51,
        GUFFAW = 52,
        HAIL = 53,
        HAPPY = 54,
        HELLO = 55,
        HUG = 56,
        HUNGRY = 57,
        KISS = 58,
        KNEEL = 59,
        LAUGH = 60,
        LAYDOWN = 61,
        MASSAGE = 62,
        MOAN = 63,
        MOON = 64,
        MOURN = 65,
        NO = 66,
        NOD = 67,
        NOSEPICK = 68,
        PANIC = 69,
        PEER = 70,
        PLEAD = 71,
        POINT = 72,
        POKE = 73,
        PRAY = 74,
        ROAR = 75,
        ROFL = 76,
        RUDE = 77,
        SALUTE = 78,
        SCRATCH = 79,
        SEXY = 80,
        SHAKE = 81,
        SHOUT = 82,
        SHRUG = 83,
        SHY = 84,
        SIGH = 85,
        SIT = 86,
        SLEEP = 87,
        SNARL = 88,
        SPIT = 89,
        STARE = 90,
        SURPRISED = 91,
        SURRENDER = 92,
        TALK = 93,
        TALKEX = 94,
        TALKQ = 95,
        TAP = 96,
        THANK = 97,
        THREATEN = 98,
        TIRED = 99,
        VICTORY = 100,
        WAVE = 101,
        WELCOME = 102,
        WHINE = 103,
        WHISTLE = 104,
        WORK = 105,
        YAWN = 106,
        BOGGLE = 107,
        CALM = 108,
        COLD = 109,
        COMFORT = 110,
        CUDDLE = 111,
        DUCK = 112,
        INSULT = 113,
        INTRODUCE = 114,
        JK = 115,
        LICK = 116,
        LISTEN = 117,
        LOST = 118,
        MOCK = 119,
        PONDER = 120,
        POUNCE = 121,
        PRAISE = 122,
        PURR = 123,
        PUZZLE = 124,
        RAISE = 125,
        READY = 126,
        SHIMMY = 127,
        SHIVER = 128,
        SHOO = 129,
        SLAP = 130,
        SMIRK = 131,
        SNIFF = 132,
        SNUB = 133,
        SOOTHE = 134,
        STINK = 135,
        TAUNT = 136,
        TEASE = 137,
        THIRSTY = 138,
        VETO = 139,
        SNICKER = 140,
        STAND = 141,
        TICKLE = 142,
        VIOLIN = 143,
        SMILE = 163,
    }

    public enum ChatMsgs
    {
        CHAT_MSG_SAY = 0x00,
        CHAT_MSG_PARTY = 0x01,
        CHAT_MSG_GUILD = 0x02,
        CHAT_MSG_OFFICER = 0x03,
        CHAT_MSG_YELL = 0x04,
        CHAT_MSG_WHISPER = 0x05,
        CHAT_MSG_WHISPER_INFORM = 0x06,
        CHAT_MSG_EMOTE = 0x07,
        CHAT_MSG_TEXT_EMOTE = 0x08,
        CHAT_MSG_SYSTEM = 0x09,
        CHAT_MSG_MONSTER_SAY = 0x0A,
        CHAT_MSG_MONSTER_YELL = 0x0B,
        CHAT_MSG_MONSTER_EMOTE = 0x0C,
        CHAT_MSG_CHANNEL = 0x0D,
        CHAT_MSG_CHANNEL_JOIN = 0x0E,
        CHAT_MSG_CHANNEL_LEAVE = 0xF,
        CHAT_MSG_CHANNEL_LIST = 0x10,
        CHAT_MSG_CHANNEL_NOTICE = 0x11,
        CHAT_MSG_CHANNEL_NOTICE_USER = 0x12,
        CHAT_MSG_AFK = 0x13,
        CHAT_MSG_DND = 0x14,
        CHAT_MSG_IGNORED = 0x16,
        CHAT_MSG_SKILL = 0x17,
        CHAT_MSG_LOOT = 0x18,
    }

    public enum ChatFlags
    {
        CHAT_TAG_NONE = 0,
        CHAT_TAG_AFK = 1,
        CHAT_TAG_DND = 2,
        CHAT_TAG_GM = 3,
    }

    public enum MoveFlags
    {
        MOVEFLAG_FORWARD = 0x1,
        MOVEFLAG_BACKWARD = 0x2,
        MOVEFLAG_STRAFE_LEFT = 0x4,
        MOVEFLAG_STRAFE_RIGHT = 0x8,
        MOVEFLAG_LEFT = 0x10,
        MOVEFLAG_RIGHT = 0x20,
        MOVEFLAG_PITCH_UP = 0x40,
        MOVEFLAG_PITCH_DOWN = 0x80,
        MOVEFLAG_WALK = 0x100,
        MOVEFLAG_TIME_VALID = 0x200,
        MOVEFLAG_IMMOBILIZED = 0x400,
        MOVEFLAG_DONTCOLLIDE = 0x800,
        MOVEFLAG_REDIRECTED = 0x1000,
        MOVEFLAG_ROOTED = 0x2000,
        MOVEFLAG_FALLING = 0x4000,
        MOVEFLAG_FALLEN_FAR = 0x8000,
        MOVEFLAG_PENDING_STOP = 0x10000,
        MOVEFLAG_PENDING_UNSTRAFE = 0x20000,
        MOVEFLAG_PENDING_FALL = 0x40000,
        MOVEFLAG_PENDING_FORWARD = 0x80000,
        MOVEFLAG_PENDING_BACKWARD = 0x100000,
        MOVEFLAG_PENDING_STR_LEFT = 0x200000,
        MOVEFLAG_PENDING_STR_RGHT = 0x400000,
        MOVEFLAG_PEND_MOVE_MASK = 0x180000,
        MOVEFLAG_PEND_STRAFE_MASK = 0x600000,
        MOVEFLAG_PENDING_MASK = 0x7F0000,
        MOVEFLAG_MOVED = 0x800000,
        MOVEFLAG_SLIDING = 0x1000000,
        MOVEFLAG_SWIMMING = 0x2000000,
        MOVEFLAG_SPLINE_MOVER = 0x4000000,
        MOVEFLAG_SPEED_DIRTY = 0x8000000,
        MOVEFLAG_HALTED = 0x10000000,
        MOVEFLAG_NUDGE = 0x20000000,
        MOVEFLAG_FALL_MASK = 0x100C000,
        MOVEFLAG_LOCAL = 0x500F400,
        MOVEFLAG_MOVE_MASK = 0x3,
        MOVEFLAG_TURN_MASK = 0x30,
        MOVEFLAG_PITCH_MASK = 0xC0,
        MOVEFLAG_STRAFE_MASK = 0xC,
        MOVEFLAG_MOTION_MASK = 0xFF,
        MOVEFLAG_STOPPED_MASK = 0x3100F,
    }

    public enum BuyResults
    {
        BUY_ERR_CANT_FIND_ITEM = 0,
        BUY_ERR_ITEM_ALREADY_SOLD = 1,
        BUY_ERR_NOT_ENOUGHT_MONEY = 2,
        BUY_ERR_SELLER_DONT_LIKE_YOU = 4,
        BUY_ERR_DISTANCE_TOO_FAR = 5,
        BUY_ERR_ITEM_SOLD_OUT = 7,
        BUY_ERR_CANT_CARRY_MORE = 8,
        BUY_ERR_RANK_REQUIRE = 11,
        BUY_ERR_REPUTATION_REQUIRE = 12
    }

    public enum SellResults
    {
        SELL_OK = 0,
        SELL_ERR_CANT_FIND_ITEM = 1,
        SELL_ERR_CANT_SELL_ITEM = 2,       // merchant doesn't like that item
        SELL_ERR_CANT_FIND_VENDOR = 3,       // merchant doesn't like you
        SELL_ERR_YOU_DONT_OWN_THAT_ITEM = 4,       // you don't own that item
        SELL_ERR_UNK = 5,       // nothing appears...
        SELL_ERR_ONLY_EMPTY_BAG = 6        // can only do with empty bags
    }

    public enum ItemBondingTypes
    {
        NO_BIND = 0,
        BIND_WHEN_PICKED_UP = 1,
        BIND_WHEN_EQUIPPED = 2,
        BIND_WHEN_USE = 3,
        BIND_QUEST_ITEM = 4
    }

    public enum PartyOperations
    {
        PARTY_OP_INVITE = 0,
        PARTY_OP_LEAVE = 2,
    }

    public enum PartyResults
    {
        ERR_PARTY_RESULT_OK = 0,
        ERR_BAD_PLAYER_NAME_S = 1,
        ERR_TARGET_NOT_IN_GROUP_S = 2,
        ERR_TARGET_NOT_IN_INSTANCE_S = 3,
        ERR_GROUP_FULL = 4,
        ERR_ALREADY_IN_GROUP_S = 5,
        ERR_NOT_IN_GROUP = 6,
        ERR_NOT_LEADER = 7,
        ERR_PLAYER_WRONG_FACTION = 8,
        ERR_IGNORING_YOU_S = 9,
        ERR_INVITE_RESTRICTED = 13,
    }

    public enum LootMethods
    {
        LOOT_METHOD_FREEFORALL = 0x0,
        LOOT_METHOD_ROUNDROBIN = 0x1,
        LOOT_METHOD_MASTERLOOTER = 0x2,
        LOOT_METHOD_MAX = 0x3,
    }

    public enum ActionButtonTypes
    {
        ACTION_BUTTON_SPELL = 0x00,
        ACTION_BUTTON_ITEM = 0xFF
    }

    public enum PlayerFlags : byte
    {
        PLAYER_FLAGS_NONE = 0x0,
        PLAYER_FLAGS_GROUP_LEADER = 0x1,
        PLAYER_FLAGS_AFK = 0x4,
        PLAYER_FLAGS_DND = 0x8,
        PLAYER_FLAGS_GM = 0x10
    }

    public enum BankSlots
    {
        BANK_SLOT_ITEM_START = 39,
        BANK_SLOT_ITEM_END = 63,
        BANK_SLOT_BAG_START = 63,
        BANK_SLOT_BAG_END = 69
    }

    public enum BankSlotErrors
    {
        BANKSLOT_ERROR_FAILED_TOO_MANY = 0,
        BANKSLOT_ERROR_INSUFFICIENT_FUNDS = 1,
        BANKSLOT_ERROR_NOTBANKER = 2,
        BANKSLOT_ERROR_OK = 3
    }

    public enum FriendResults
    {
        FRIEND_DB_ERROR = 0x0,
        FRIEND_LIST_FULL = 0x1,
        FRIEND_ONLINE = 0x2,
        FRIEND_OFFLINE = 0x3,
        FRIEND_NOT_FOUND = 0x4,
        FRIEND_REMOVED = 0x5,
        FRIEND_ADDED_ONLINE = 0x6,
        FRIEND_ADDED_OFFLINE = 0x7,
        FRIEND_ALREADY = 0x8,
        FRIEND_SELF = 0x9,
        FRIEND_ENEMY = 0xA,
        FRIEND_IGNORE_FULL = 0xB,
        FRIEND_IGNORE_SELF = 0xC,
        FRIEND_IGNORE_NOT_FOUND = 0xD,
        FRIEND_IGNORE_ALREADY = 0xE,
        FRIEND_IGNORE_ADDED = 0xF,
        FRIEND_IGNORE_REMOVED = 0x10,
    }

    public enum FriendStatuses
    {
        FRIEND_STATUS_OFFLINE = 0,
        FRIEND_STATUS_ONLINE = 1,
        FRIEND_STATUS_AFK = 2,
        FRIEND_STATUS_UNK3 = 3,
        FRIEND_STATUS_DND = 4
    }

    public enum WhoPartyStatuses
    {
        WHO_PARTY_STATUS_NOT_IN_PARTY = 0x0,
        WHO_PARTY_STATUS_IN_PARTY = 0x1,
        WHO_PARTY_STATUS_LFG = 0x2,
    }

    public enum WhoSortTypes
    {
        WHO_SORT_ZONE = 0x0,
        WHO_SORT_LEVEL = 0x1,
        WHO_SORT_CLASS = 0x2,
        WHO_SORT_GROUP = 0x3,
        WHO_SORT_NAME = 0x4,
        WHO_SORT_RACE = 0x5,
        WHO_SORT_GUILD = 0x6,
    }

    public enum SpellTargetType
    {
        TARGET_TYPE_CASTER = 0x0,
        TARGET_TYPE_UNIT = 0x1,
        TARGET_TYPE_FRIENDLY = 0x2,
        TARGET_TYPE_ENEMY = 0x3,
        TARGET_TYPE_PARTY_MEMBER = 0x4,
        TARGET_TYPE_ITEM = 0x5,
        TARGET_TYPE_LOCATION = 0x6,
        TARGET_TYPE_OBJECT = 0x7,
        TARGET_TYPE_PET = 0x8,
        TARGET_TYPE_MAINHAND_ITEM = 0x9,
        TARGET_TYPE_OFFHAND_ITEM = 0xA,
        TARGET_TYPE_PARTY = 0xB,
        TARGET_TYPE_MULTIPLE_UNITS = 0xC,
        TARGET_TYPE_MULTIPLE_ENEMIES = 0xD,
        TARGET_TYPE_MULTIPLE_PARTY_MEMBERS = 0xE,
        TARGET_TYPE_MASTER = 0xF,
        NUM_SPELL_TARGET_TYPES = 0x10,
    }

    public enum SpellFailedReason
    {
        SPELL_FAILED_AFFECTING_COMBAT = 0x0,
        SPELL_FAILED_ALREADY_HAVE_CHARM = 0x1,
        SPELL_FAILED_ALREADY_HAVE_SUMMON = 0x2,
        SPELL_FAILED_ALREADY_OPEN = 0x3,
        SPELL_FAILED_AURA_BOUNCED = 0x4,
        SPELL_FAILED_BAD_IMPLICIT_TARGETS = 0x5,
        SPELL_FAILED_BAD_TARGETS = 0x6,
        SPELL_FAILED_CANT_BE_CHARMED = 0x7,
        SPELL_FAILED_CANT_STEALTH = 0x8,
        SPELL_FAILED_CASTER_AURASTATE = 0x9,
        SPELL_FAILED_CASTER_DEAD = 0xA,
        SPELL_FAILED_DONT_REPORT = 0xB,
        SPELL_FAILED_EQUIPPED_ITEM = 0xC,
        SPELL_FAILED_EQUIPPED_ITEM_CLASS = 0xD,
        SPELL_FAILED_ERROR = 0xE,
        SPELL_FAILED_FIZZLE = 0xF,
        SPELL_FAILED_HUNGER_SATIATED = 0x10,
        SPELL_FAILED_INTERRUPTED = 0x11,
        SPELL_FAILED_INTERRUPTED_COMBAT = 0x12,
        SPELL_FAILED_ITEM_ALREADY_ENCHANTED = 0x13,
        SPELL_FAILED_ITEM_NOT_FOUND = 0x14,
        SPELL_FAILED_ITEM_NOT_READY = 0x15,
        SPELL_FAILED_LEVEL_REQUIREMENT = 0x16,
        SPELL_FAILED_LINE_OF_SIGHT = 0x17,
        SPELL_FAILED_LOWLEVEL = 0x18,
        SPELL_FAILED_LOW_CASTLEVEL = 0x19,
        SPELL_FAILED_MOVING = 0x1A,
        SPELL_FAILED_NEED_AMMO = 0x1B,
        SPELL_FAILED_NEED_AMMO_POUCH = 0x1C,
        SPELL_FAILED_NEED_EXOTIC_AMMO = 0x1D,
        SPELL_FAILED_NOPATH = 0x1E,
        SPELL_FAILED_NOTSTANDING = 0x1F,
        SPELL_FAILED_NOT_BEHIND = 0x20,
        SPELL_FAILED_NOT_BEHIND_OR_SIDE = 0x21,
        SPELL_FAILED_NOT_HERE = 0x22,
        SPELL_FAILED_NOT_KNOWN = 0x23,
        SPELL_FAILED_NOT_MOUNTED = 0x24,
        SPELL_FAILED_NOT_READY = 0x25,
        SPELL_FAILED_NOT_SHAPESHIFT = 0x26,
        SPELL_FAILED_NOT_TRADING = 0x27,
        SPELL_FAILED_NO_AMMO = 0x28,
        SPELL_FAILED_NO_CHARGES_REMAIN = 0x29,
        SPELL_FAILED_NO_ENDURANCE = 0x2A,
        SPELL_FAILED_NO_PET = 0x2B,
        SPELL_FAILED_NO_POWER = 0x2C,
        SPELL_FAILED_ONLY_ABOVEWATER = 0x2D,
        SPELL_FAILED_ONLY_DAYTIME = 0x2E,
        SPELL_FAILED_ONLY_INDOORS = 0x2F,
        SPELL_FAILED_ONLY_MOUNTED = 0x30,
        SPELL_FAILED_ONLY_NIGHTTIME = 0x31,
        SPELL_FAILED_ONLY_OUTDOORS = 0x32,
        SPELL_FAILED_ONLY_SHAPESHIFT = 0x33,
        SPELL_FAILED_ONLY_STEALTHED = 0x34,
        SPELL_FAILED_ONLY_UNDERWATER = 0x35,
        SPELL_FAILED_OUT_OF_RANGE = 0x36,
        SPELL_FAILED_PACIFIED = 0x37,
        SPELL_FAILED_REAGENTS = 0x38,
        SPELL_FAILED_REQUIRES_SPELL_FOCUS = 0x39,
        SPELL_FAILED_SILENCED = 0x3A,
        SPELL_FAILED_SPELL_IN_PROGRESS = 0x3B,
        SPELL_FAILED_SPELL_LEARNED = 0x3C,
        SPELL_FAILED_SPELL_UNAVAILABLE = 0x3D,
        SPELL_FAILED_STUNNED = 0x3E,
        SPELL_FAILED_TARGETS_DEAD = 0x3F,
        SPELL_FAILED_TARGET_AFFECTING_COMBAT = 0x40,
        SPELL_FAILED_TARGET_AURASTATE = 0x41,
        SPELL_FAILED_TARGET_ENEMY = 0x42,
        SPELL_FAILED_TARGET_ENRAGED = 0x43,
        SPELL_FAILED_TARGET_FRIENDLY = 0x44,
        SPELL_FAILED_TARGET_IS_PLAYER = 0x45,
        SPELL_FAILED_TARGET_NOT_DEAD = 0x46,
        SPELL_FAILED_TARGET_NOT_IN_PARTY = 0x47,
        SPELL_FAILED_TARGET_NO_POCKETS = 0x48,
        SPELL_FAILED_THIRST_SATIATED = 0x49,
        SPELL_FAILED_TOO_CLOSE = 0x4A,
        SPELL_FAILED_TOTEMS = 0x4B,
        SPELL_FAILED_TRY_AGAIN = 0x4C,
        SPELL_FAILED_UNIT_NOT_ATSIDE = 0x4D,
        SPELL_FAILED_UNIT_NOT_BEHIND = 0x4E,
        SPELL_FAILED_UNIT_NOT_INFRONT = 0x4F,
        SPELL_FAILED_NO_MOUNTS_ALLOWED = 0x50,
        SPELL_FAILED_CHEST_IN_USE = 0x51,
        SPELL_FAILED_NO_COMBO_POINTS = 0x52,
        SPELL_FAILED_TARGET_NOT_PLAYER = 0x53,
        SPELL_FAILED_TARGET_DUELING = 0x54,
        SPELL_FAILED_NOTUNSHEATHED = 0x55,
        SPELL_FAILED_NOT_FISHABLE = 0x56,
        SPELL_FAILED_NO_REASON = 0x57
    }

    public enum SpellAttributes : uint
    {
        SPELL_ATTR_UNK0 = 0x00000001,            // 0
        SPELL_ATTR_RANGED = 0x00000002,            // 1 All ranged abilites have this flag
        SPELL_ATTR_ON_NEXT_SWING_1 = 0x00000004,            // 2 on next swing
        SPELL_ATTR_UNK3 = 0x00000008,            // 3 not set in 2.4.2
        SPELL_ATTR_UNK4 = 0x00000010,            // 4 isAbility
        SPELL_ATTR_TRADESPELL = 0x00000020,            // 5 trade spells, will be added by client to a sublist of profession spell
        SPELL_ATTR_PASSIVE = 0x00000040,            // 6 Passive spell
        SPELL_ATTR_UNK7 = 0x00000080,            // 7 can't be linked in chat?
        SPELL_ATTR_UNK8 = 0x00000100,            // 8 hide created item in tooltip (for effect=24)
        SPELL_ATTR_UNK9 = 0x00000200,            // 9
        SPELL_ATTR_ON_NEXT_SWING_2 = 0x00000400,            // 10 on next swing 2
        SPELL_ATTR_UNK11 = 0x00000800,            // 11
        SPELL_ATTR_DAYTIME_ONLY = 0x00001000,            // 12 only useable at daytime, not set in 2.4.2
        SPELL_ATTR_NIGHT_ONLY = 0x00002000,            // 13 only useable at night, not set in 2.4.2
        SPELL_ATTR_INDOORS_ONLY = 0x00004000,            // 14 only useable indoors, not set in 2.4.2
        SPELL_ATTR_OUTDOORS_ONLY = 0x00008000,            // 15 Only useable outdoors.
        SPELL_ATTR_NOT_SHAPESHIFT = 0x00010000,            // 16 Not while shapeshifted
        SPELL_ATTR_ONLY_STEALTHED = 0x00020000,            // 17 Must be in stealth
        SPELL_ATTR_UNK18 = 0x00040000,            // 18
        SPELL_ATTR_LEVEL_DAMAGE_CALCULATION = 0x00080000,            // 19 spelldamage depends on caster level
        SPELL_ATTR_STOP_ATTACK_TARGET = 0x00100000,            // 20 Stop attack after use this spell (and not begin attack if use)
        SPELL_ATTR_IMPOSSIBLE_DODGE_PARRY_BLOCK = 0x00200000,            // 21 Cannot be dodged/parried/blocked
        SPELL_ATTR_SET_TRACKING_TARGET = 0x00400000,            // 22 SetTrackingTarget
        SPELL_ATTR_UNK23 = 0x00800000,            // 23 castable while dead?
        SPELL_ATTR_CASTABLE_WHILE_MOUNTED = 0x01000000,            // 24 castable while mounted
        SPELL_ATTR_DISABLED_WHILE_ACTIVE = 0x02000000,            // 25 Activate and start cooldown after aura fade or remove summoned creature or go
        SPELL_ATTR_UNK26 = 0x04000000,            // 26
        SPELL_ATTR_CASTABLE_WHILE_SITTING = 0x08000000,            // 27 castable while sitting
        SPELL_ATTR_CANT_USED_IN_COMBAT = 0x10000000,            // 28 Cannot be used in combat
        SPELL_ATTR_UNAFFECTED_BY_INVULNERABILITY = 0x20000000,            // 29 unaffected by invulnerability (hmm possible not...)
        SPELL_ATTR_UNK30 = 0x40000000,            // 30 breakable by damage?
        SPELL_ATTR_CANT_CANCEL = 0x80000000,            // 31 positive aura can't be canceled
    }

    public enum SpellAttributesEx : uint
    {
        SPELL_ATTR_EX_UNK0 = 0x00000001,            // 0
        SPELL_ATTR_EX_DRAIN_ALL_POWER = 0x00000002,            // 1 use all power (Only paladin Lay of Hands and Bunyanize)
        SPELL_ATTR_EX_CHANNELED_1 = 0x00000004,            // 2 channeled 1
        SPELL_ATTR_EX_UNK3 = 0x00000008,            // 3
        SPELL_ATTR_EX_UNK4 = 0x00000010,            // 4
        SPELL_ATTR_EX_NOT_BREAK_STEALTH = 0x00000020,            // 5 Not break stealth
        SPELL_ATTR_EX_CHANNELED_2 = 0x00000040,            // 6 channeled 2
        SPELL_ATTR_EX_NEGATIVE = 0x00000080,            // 7
        SPELL_ATTR_EX_NOT_IN_COMBAT_TARGET = 0x00000100,            // 8 Spell req target not to be in combat state
        SPELL_ATTR_EX_UNK9 = 0x00000200,            // 9
        SPELL_ATTR_EX_NO_THREAT = 0x00000400,            // 10 no generates threat on cast 100%
        SPELL_ATTR_EX_UNK11 = 0x00000800,            // 11
        SPELL_ATTR_EX_UNK12 = 0x00001000,            // 12
        SPELL_ATTR_EX_FARSIGHT = 0x00002000,            // 13 related to farsight
        SPELL_ATTR_EX_UNK14 = 0x00004000,            // 14
        SPELL_ATTR_EX_DISPEL_AURAS_ON_IMMUNITY = 0x00008000,            // 15 remove auras on immunity
        SPELL_ATTR_EX_UNAFFECTED_BY_SCHOOL_IMMUNE = 0x00010000,            // 16 unaffected by school immunity
        SPELL_ATTR_EX_UNK17 = 0x00020000,            // 17 for auras SPELL_AURA_TRACK_CREATURES, SPELL_AURA_TRACK_RESOURCES and SPELL_AURA_TRACK_STEALTHED select non-stacking tracking spells
        SPELL_ATTR_EX_UNK18 = 0x00040000,            // 18
        SPELL_ATTR_EX_UNK19 = 0x00080000,            // 19
        SPELL_ATTR_EX_REQ_TARGET_COMBO_POINTS = 0x00100000,            // 20 Req combo points on target
        SPELL_ATTR_EX_UNK21 = 0x00200000,            // 21
        SPELL_ATTR_EX_REQ_COMBO_POINTS = 0x00400000,            // 22 Use combo points (in 4.x not required combo point target selected)
        SPELL_ATTR_EX_UNK23 = 0x00800000,            // 23
        SPELL_ATTR_EX_UNK24 = 0x01000000,            // 24 Req fishing pole??
        SPELL_ATTR_EX_UNK25 = 0x02000000,            // 25 not set in 2.4.2
        SPELL_ATTR_EX_UNK26 = 0x04000000,            // 26
        SPELL_ATTR_EX_UNK27 = 0x08000000,            // 27
        SPELL_ATTR_EX_UNK28 = 0x10000000,            // 28
        SPELL_ATTR_EX_UNK29 = 0x20000000,            // 29
        SPELL_ATTR_EX_UNK30 = 0x40000000,            // 30 overpower
        SPELL_ATTR_EX_UNK31 = 0x80000000,            // 31
    }

    public enum SpellSchools
    {
        SPELL_SCHOOL_NORMAL = 0,                //< Physical, Armor
        SPELL_SCHOOL_HOLY = 1,
        SPELL_SCHOOL_FIRE = 2,
        SPELL_SCHOOL_NATURE = 3,
        SPELL_SCHOOL_FROST = 4,
        SPELL_SCHOOL_SHADOW = 5
    }

    public enum SpellSchoolMask
    {
        SPELL_SCHOOL_MASK_NONE = 0x00,                       // not exist
        SPELL_SCHOOL_MASK_NORMAL = (1 << SpellSchools.SPELL_SCHOOL_NORMAL), // PHYSICAL (Armor)
        SPELL_SCHOOL_MASK_HOLY = (1 << SpellSchools.SPELL_SCHOOL_HOLY),
        SPELL_SCHOOL_MASK_FIRE = (1 << SpellSchools.SPELL_SCHOOL_FIRE),
        SPELL_SCHOOL_MASK_NATURE = (1 << SpellSchools.SPELL_SCHOOL_NATURE),
        SPELL_SCHOOL_MASK_FROST = (1 << SpellSchools.SPELL_SCHOOL_FROST),
        SPELL_SCHOOL_MASK_SHADOW = (1 << SpellSchools.SPELL_SCHOOL_SHADOW),
        SPELL_SCHOOL_MASK_SPELL = (SPELL_SCHOOL_MASK_FIRE | SPELL_SCHOOL_MASK_NATURE | SPELL_SCHOOL_MASK_FROST | SPELL_SCHOOL_MASK_SHADOW),
        SPELL_SCHOOL_MASK_MAGIC = (SPELL_SCHOOL_MASK_HOLY | SPELL_SCHOOL_MASK_SPELL),
        SPELL_SCHOOL_MASK_ALL = (SPELL_SCHOOL_MASK_NORMAL | SPELL_SCHOOL_MASK_MAGIC)
    }

    public enum SpellState
    {
        SPELL_STATE_PREPARING = 0,                              // cast time delay period, non channeled spell
        SPELL_STATE_CASTING = 1,                              // channeled time period spell casting state
        SPELL_STATE_FINISHED = 2,                              // cast finished to success or fail
        SPELL_STATE_DELAYED = 3                               // spell casted but need time to hit target(s)
    }

    public enum CurrentSpellType
    {
        CURRENT_MELEE_SPELL = 0,
        CURRENT_GENERIC_SPELL = 1,
        CURRENT_CHANNELED_SPELL = 2
    }

    public enum SpellEffects
    {
        SPELL_EFFECT_NONE = 0x0,
        SPELL_EFFECT_INSTAKILL = 0x1,
        SPELL_EFFECT_SCHOOL_DAMAGE = 0x2,
        SPELL_EFFECT_DUMMY = 0x3,
        //SPELL_EFFECT_PORTAL_TELEPORT = 0x4, //Not used
        SPELL_EFFECT_TELEPORT_UNITS = 0x5,
        SPELL_EFFECT_APPLY_AURA = 0x6,
        SPELL_EFFECT_POWER_DRAIN = 0x8,
        SPELL_EFFECT_HEALTH_LEECH = 0x9,
        SPELL_EFFECT_HEAL = 0xA,
        SPELL_EFFECT_BIND = 0xB,
        SPELL_EFFECT_PORTAL = 0xC,
        //SPELL_EFFECT_RITUAL_BASE = 0xD, //Not used
        //SPELL_EFFECT_RITUAL_SPECIALIZE = 0xE,//Not used
        //SPELL_EFFECT_RITUAL_ACTIVATE_PORTAL = 0xF, //Not used
        SPELL_EFFECT_QUEST_COMPLETE = 0x10,
        SPELL_EFFECT_WEAPON_DAMAGE = 0x11,
        SPELL_EFFECT_RESURRECT = 0x12,
        SPELL_EFFECT_EXTRA_ATTACKS = 0x13,
        SPELL_EFFECT_DODGE = 0x14,
        SPELL_EFFECT_EVADE = 0x15,
        SPELL_EFFECT_PARRY = 0x16,
        SPELL_EFFECT_BLOCK = 0x17,
        SPELL_EFFECT_CREATE_ITEM = 0x18,
        SPELL_EFFECT_WEAPON = 0x19,
        SPELL_EFFECT_DEFENSE = 0x1A,
        SPELL_EFFECT_PERSISTENT_AREA_AURA = 0x1B,
        SPELL_EFFECT_SUMMON = 0x1C,
        SPELL_EFFECT_LEAP = 0x1D,
        SPELL_EFFECT_ENERGIZE = 0x1E,
        SPELL_EFFECT_WEAPON_PERC_DMG = 0x1F,
        //SPELL_EFFECT_TRIGGER_MISSILE = 0x20, //Not used
        SPELL_EFFECT_OPEN_LOCK = 0x21,
        SPELL_EFFECT_SUMMON_MOUNT = 0x22,
        SPELL_EFFECT_APPLY_AREA_AURA = 0x23,
        SPELL_EFFECT_LEARN_SPELL = 0x24,
        SPELL_EFFECT_SPELL_DEFENSE = 0x25,
        SPELL_EFFECT_DISPEL = 0x26,
        SPELL_EFFECT_LANGUAGE = 0x27,
        SPELL_EFFECT_DUAL_WIELD = 0x28,
        SPELL_EFFECT_SUMMON_WILD = 0x29,
        SPELL_EFFECT_SUMMON_GUARDIAN = 0x2A,
        //SPELL_EFFECT_SKILL_RANK = 0x2B, //Not used
        SPELL_EFFECT_SKILL_STEP = 0x2C,
        //SPELL_EFFECT_SKILL_POTENTIAL = 0x2D, //Not used
        SPELL_EFFECT_SPAWN = 0x2E,
        SPELL_EFFECT_SPELL_CAST_UI = 0x2F,
        SPELL_EFFECT_STEALTH = 0x30,
        SPELL_EFFECT_DETECT = 0x31,
        SPELL_EFFECT_SUMMON_OBJECT = 0x32,
        //SPELL_EFFECT_FORCE_CRITICAL_HIT = 0x33, //Not used
        SPELL_EFFECT_GUARANTEE_HIT = 0x34,
        SPELL_EFFECT_ENCHANT_ITEM_PERMANENT = 0x35,
        SPELL_EFFECT_ENCHANT_ITEM_TEMPORARY = 0x36,
        SPELL_EFFECT_TAME_CREATURE = 0x37,
        SPELL_EFFECT_SUMMON_PET = 0x38,
        SPELL_EFFECT_LEARN_PET_SPELL = 0x39,
        SPELL_EFFECT_WEAPON_DAMAGE_PLUS = 0x3A,
        SPELL_EFFECT_OPEN_LOCK_ITEM = 0x3B,
        SPELL_EFFECT_PROFICIENCY = 0x3C,
        SPELL_EFFECT_SEND_EVENT = 0x3D,
        SPELL_EFFECT_POWER_BURN = 0x3E,
        SPELL_EFFECT_THREAT = 0x3F,
        SPELL_EFFECT_TRIGGER_SPELL = 0x40,
        //SPELL_EFFECT_HEALTH_FUNNEL = 0x41, //Not used
        //SPELL_EFFECT_MANA_FUNNEL = 0x42, //Not used
        SPELL_EFFECT_HEAL_MAX_HEALTH = 0x43,
        SPELL_EFFECT_INTERRUPT_CAST = 0x44,
        SPELL_EFFECT_DISTRACT = 0x45,
        SPELL_EFFECT_PULL = 0x46,
        SPELL_EFFECT_PICKPOCKET = 0x47,
        SPELL_EFFECT_ADD_FARSIGHT = 0x48,
        SPELL_EFFECT_SUMMON_POSSESSED = 0x49,
        SPELL_EFFECT_SUMMON_TOTEM = 0x4A,
        //SPELL_EFFECT_HEAL_MECHANICAL = 0x4B, //Not used
        SPELL_EFFECT_SUMMON_OBJECT_WILD = 0x4C,
        SPELL_EFFECT_SCRIPT_EFFECT = 0x4D,
        SPELL_EFFECT_ATTACK = 0x4E,
        SPELL_EFFECT_SANCTUARY = 0x4F,
        SPELL_EFFECT_ADD_COMBO_POINTS = 0x50,
        SPELL_EFFECT_CREATE_HOUSE = 0x51,
        SPELL_EFFECT_BIND_SIGHT = 0x52,
        SPELL_EFFECT_DUEL = 0x53,
        SPELL_EFFECT_STUCK = 0x54,
        SPELL_EFFECT_SUMMON_PLAYER = 0x55,
        SPELL_EFFECT_ACTIVATE_OBJECT = 0x56
    }

    public enum SpellInterruptFlags
    {
        SPELL_INTERRUPT_FLAG_MOVEMENT = 0x01,
        SPELL_INTERRUPT_FLAG_DAMAGE = 0x02,
        SPELL_INTERRUPT_FLAG_INTERRUPT = 0x04,
        SPELL_INTERRUPT_FLAG_AUTOATTACK = 0x08,
        SPELL_INTERRUPT_FLAG_ABORT_ON_DMG = 0x10,               // _complete_ interrupt on direct damage
                                                                // SPELL_INTERRUPT_UNK             = 0x20               // unk, 564 of 727 spells having this spell start with "Glyph"
    }

    public enum SpellChannelInterruptFlags
    {
        CHANNEL_FLAG_DAMAGE = 0x0002,
        CHANNEL_FLAG_MOVEMENT = 0x0008,
        CHANNEL_FLAG_TURNING = 0x0010,
        CHANNEL_FLAG_DAMAGE2 = 0x0080,
        CHANNEL_FLAG_DELAY = 0x4000
    }

    public enum SpellAuraInterruptFlags
    {
        AURA_INTERRUPT_FLAG_UNK0 = 0x00000001,   // 0    removed when getting hit by a negative spell?
        AURA_INTERRUPT_FLAG_DAMAGE = 0x00000002,   // 1    removed by any damage
        AURA_INTERRUPT_FLAG_UNK2 = 0x00000004,   // 2
        AURA_INTERRUPT_FLAG_MOVE = 0x00000008,   // 3    removed by any movement
        AURA_INTERRUPT_FLAG_TURNING = 0x00000010,   // 4    removed by any turning
        AURA_INTERRUPT_FLAG_ENTER_COMBAT = 0x00000020,   // 5    removed by entering combat
        AURA_INTERRUPT_FLAG_NOT_MOUNTED = 0x00000040,   // 6    removed by unmounting
        AURA_INTERRUPT_FLAG_NOT_ABOVEWATER = 0x00000080,   // 7    removed by entering water
        AURA_INTERRUPT_FLAG_NOT_UNDERWATER = 0x00000100,   // 8    removed by leaving water
        AURA_INTERRUPT_FLAG_NOT_SHEATHED = 0x00000200,   // 9    removed by unsheathing
        AURA_INTERRUPT_FLAG_UNK10 = 0x00000400,   // 10
        AURA_INTERRUPT_FLAG_UNK11 = 0x00000800,   // 11
        AURA_INTERRUPT_FLAG_UNK12 = 0x00001000,   // 12   removed by attack?
        AURA_INTERRUPT_FLAG_UNK13 = 0x00002000,   // 13
        AURA_INTERRUPT_FLAG_UNK14 = 0x00004000,   // 14
        AURA_INTERRUPT_FLAG_UNK15 = 0x00008000,   // 15   removed by casting a spell?
        AURA_INTERRUPT_FLAG_UNK16 = 0x00010000,   // 16
        AURA_INTERRUPT_FLAG_MOUNTING = 0x00020000,   // 17   removed by mounting
        AURA_INTERRUPT_FLAG_NOT_SEATED = 0x00040000,   // 18   removed by standing up (used by food and drink mostly and sleep/Fake Death like)
        AURA_INTERRUPT_FLAG_CHANGE_MAP = 0x00080000,   // 19   leaving map/getting teleported
        AURA_INTERRUPT_FLAG_IMMUNE_OR_LOST_SELECTION = 0x00100000,   // 20   removed by auras that make you invulnerable, or make other to loose selection on you
        AURA_INTERRUPT_FLAG_UNK21 = 0x00200000,   // 21
        AURA_INTERRUPT_FLAG_UNK22 = 0x00400000,   // 22
        AURA_INTERRUPT_FLAG_ENTER_PVP_COMBAT = 0x00800000,   // 23   removed by entering pvp combat
        AURA_INTERRUPT_FLAG_DIRECT_DAMAGE = 0x01000000    // 24   removed by any direct damage
    }

    public enum SpellImplicitTargets : byte
    {
        TARGET_NOTHING = 0,
        TARGET_SELF = 1,
        TARGET_RANDOM_ENEMY_CHAIN_IN_AREA = 2,        //Only one spell has this one, but regardless, it's a target type after all
        TARGET_PET = 5,
        TARGET_CHAIN_DAMAGE = 6,
        TARGET_AREAEFFECT_CUSTOM = 8,
        TARGET_INNKEEPER_COORDINATES = 9,        //Used in teleport to innkeeper spells
        TARGET_ALL_ENEMY_IN_AREA = 15,
        TARGET_ALL_ENEMY_IN_AREA_INSTANT = 16,
        TARGET_TABLE_X_Y_Z_COORDINATES = 17,        //Used in teleport spells and some other
        TARGET_EFFECT_SELECT = 18,        //Highly depends on the spell effect
        TARGET_AROUND_CASTER_PARTY = 20,
        TARGET_SELECTED_FRIEND = 21,
        TARGET_AROUND_CASTER_ENEMY = 22,        //Used only in TargetA, target selection dependent from TargetB
        TARGET_SELECTED_GAMEOBJECT = 23,
        TARGET_INFRONT = 24,
        TARGET_DUEL_VS_PLAYER = 25,        //Used when part of spell is casted on another target
        TARGET_GAMEOBJECT_AND_ITEM = 26,
        TARGET_MASTER = 27,        //not tested
        TARGET_AREA_EFFECT_ENEMY_CHANNEL = 28,
        TARGET_ALL_FRIENDLY_UNITS_AROUND_CASTER = 30,        //In TargetB used only with TARGET_ALL_AROUND_CASTER and in self casting range in TargetA
        TARGET_ALL_FRIENDLY_UNITS_IN_AREA = 31,
        TARGET_MINION = 32,        //Summons your pet to you.
        TARGET_ALL_PARTY = 33,
        TARGET_ALL_PARTY_AROUND_CASTER_2 = 34,        //Used in Tranquility
        TARGET_SINGLE_PARTY = 35,
        TARGET_AREAEFFECT_PARTY = 37,        //Power infuses the target's party, increasing their Shadow resistance by $s1 for $d.
        TARGET_SCRIPT = 38,
        TARGET_SELF_FISHING = 39,        //Equip a fishing pole and find a body of water to fish.
        TARGET_TOTEM_EARTH = 41,
        TARGET_TOTEM_WATER = 42,
        TARGET_TOTEM_AIR = 43,
        TARGET_TOTEM_FIRE = 44,
        TARGET_CHAIN_HEAL = 45,
        TARGET_DYNAMIC_OBJECT = 47,
        TARGET_AREA_EFFECT_SELECTED = 53,        //Inflicts $s1 Fire damage to all enemies in a selected area.
        TARGET_UNK54 = 54,
        TARGET_RANDOM_RAID_MEMBER = 56,
        TARGET_SINGLE_FRIEND_2 = 57,
        TARGET_AREAEFFECT_PARTY_AND_CLASS = 61,
        TARGET_DUELVSPLAYER_COORDINATES = 63,
        TARGET_BEHIND_VICTIM = 65,        // uses in teleport behind spells
        TARGET_SINGLE_ENEMY = 77,
        TARGET_SELF2 = 87,
        TARGET_NONCOMBAT_PET = 90
    }

    public enum SpellMissInfo
    {
        MISS_NONE = 0x0,
        MISS_PHYSICAL = 0x1,
        MISS_RESIST = 0x2,
        MISS_IMMUNE = 0x3,
        MISS_EVADED = 0x4,
        MISS_DODGED = 0x5,
        MISS_PARRIED = 0x6,
        MISS_BLOCKED = 0x7,
        MISS_TEMPIMMUNE = 0x8,
        MISS_DEFLECTED = 0x9,
        MISS_NUMMISSTYPES = 0xA,
    }

    public enum SpellDamageType
    {
        SPELL_TYPE_NONMELEE = 0,
        SPELL_TYPE_DOT = 1,
        SPELL_TYPE_HEAL = 2,
        SPELL_TYPE_HEALDOT = 3
    }
}

