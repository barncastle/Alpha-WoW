using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common.Constants
{
    #region UpdateFields
    public struct CGUnitData
    {
        ulong bytem;
        ulong summon;
        ulong bytemedBy;
        ulong summonedBy;
        ulong createdBy;
        ulong target;
        ulong comboTarget;
        ulong channelObject;
        int health;
        int[] power; //int[4];
        int maxHealth;
        int[] maxPower;// int[4];
        int level;
        int factionTemplate;
        byte race;
        byte classId;
        byte sex;
        byte displayPower;
        int[] stats; //int[5];
        int[] baseStats; //int[5];
        uint[] virtualItemDisplay; //uint[3];
        VirtualItemInfo[] virtualItemInfo; //VirtualItemInfo[3];
        uint flags;
        uint coinage;
        int[] auras; //int[56];
        byte[] auraFlags; //byte[28];
        uint auraState;
        int[] modDamageDone; //int[6];
        int[] modDamageTaken; //int[6];
        int[] modCreatureDamageDone; //int[8];
        uint[] attackRoundBaseTime; //uint[2];
        int[] resistances; //int[6];
        float boundingRadius;
        float combatReach;
        float weaponReach;
        int displayID;
        int mountDisplayID;
        ushort minDamage;
        ushort maxDamage;
        int[] resistanceBuffModsPositive; //int[6];
        int[] resistanceBuffModsNegative; //int[6];
        int[] resistanceItemMods; //int[6];
        byte standState;
        byte npcFlags;
        byte shapeshiftForm;
        byte weaponMode;
        uint petNumber;
        uint petNameTimestamp;
        uint petExperience;
        uint petNextLevelExperience;
        uint dynamicFlags;
        uint emoteState;
        int channelSpell;
        int modCastingSpeed;
        int createdBySpell;
        byte comboPoints;
        byte bytepad1;
        byte bytepad2;
        byte bytepad3;
        uint pad;
    }

   /* public struct CGPlayerData
    {
        UInt64[] invSlots; //[69];
        ulong selection;
        ulong farsightObject;
        ulong duelArbiter;
        uint numInvSlots;
        uint guildID;
        uint guildRank;
        string skinID;
        string faceID;
        string hairStyleID;
        string hairColorID;
        int XP;
        int nextLevelXP;
        MirrorSkillInfo[] skillInfo; //[64];
        string playerFlags;
        string facialHairStyleID;
        string numBankSlots;
        string padByte;
        CQuestLogData[] questLog; //[16];
        int[] stringacterPoints; //[2];
        uint trackCreatureMask;
        uint trackResourceMask;
        uint chatFilters;
        uint duelTeam;
        float blockPercentage;
        float dodgePercentage;
        float parryPercentage;
        int baseMana;
        int guildTimeStamp;
    } */

    public struct CGItemData
    {
        ulong m_owner;
        ulong m_containedIn;
        ulong m_creator;
        uint m_stackCount;
        int m_expiration;
        int[] m_spellstringges; //[5];
        Int16 m_staticFlags;
        Int16 m_dynamicFlags;
        ItemEnchantment[] m_enchantment; //[5];
        int pad;
    }

    public struct CGContainerData
    {
        uint m_numSlots;
        uint m_pad;
        UInt64[] m_slots; //[20];
    }

    public struct CGGameObjectData
    {
        int m_displayID;
        uint m_flags;
        Quaternion m_rotation;
        int m_state;
        uint m_timestamp;
        Vector m_position;
        float m_facing;
        uint m_dynamicFlags;
        int m_factionTemplate;
    }

    public struct CGDynamicObjectData
    {
        ulong m_caster;
        string m_type;
        string m_typeFlags;
        string[] m_padding; //[2];
        int m_spellID;
        float m_radius;
        Vector m_position;
        float m_facing;
        int m_morePadding;
    }

    public struct CGCorpseData
    {
        ulong m_owner;
        float m_facing;
        Vector m_position;
        uint m_displayID;
        uint[] m_items; //[19];
        string m_unused;
        string m_raceID;
        string m_sex;
        string m_skinID;
        string m_faceID;
        string m_hairStyleID;
        string m_hairColorID;
        string m_facialHairStyleID;
        uint m_guildID;
        uint m_level;
    }

    public struct CGPetition
    {
        int m_petitionID;
        ulong m_petitioner;
        string m_title; //[256];
        string m_bodyText; //[4096];
        int m_flags;
        int m_minSignatures;
        int m_maxSignatures;
        int m_deadLine;
        int m_issueDate;
        int m_allowedGuildID;
        int m_allowedClasses;
        int m_allowedRaces;
        Int16 m_allowedGender;
        int m_allowedMinLevel;
        int m_allowedMaxLevel;
        string[,] m_choicetext; //[10][64];
        int m_numChoices;
        uint m_muid;
    }
    #endregion

    public struct VirtualItemInfo
    {
        public byte m_classID;
        public byte m_subclassID;
        public byte m_material;
        public byte m_inventoryType;
        public byte m_sheatheType;
        public byte m_padding0;
        public byte m_padding1;
        public byte m_padding2;

        public uint m_display;
    }
    
    public struct CQuestLogData
    {
        int m_questID;
        int m_questGiverID;
        int m_questRewarderID;
        uint m_questFlags;
        int m_questFailureTime;
        int m_qtyMonsterToKill;
    }

    public struct ItemEnchantment
    {
        int id;
        int expiration;
        int stringgesRemaining;
    }

    public struct CMovementStatus
    {
        ulong transport;
        Vector transRelPosition;
        float transRelFacing;
        Vector worldPosition;
        float worldFacing;
        float pitch;
        uint moveFlags;
    }

    public struct CClientMoveUpdate
    {
        CMovementStatus status;
        uint timeFallen;
        float walkSpeed;
        float runSpeed;
        float swimSpeed;
        float turnRate;
        CMoveSpline spline;
    }

    public struct CMoveSpline
    {
        Vector spot;
        ulong guid;
        float facing;
    };

    public struct CClientObjCreate
    {
        CClientMoveUpdate move;
        uint flags;
        uint attackCycle;
        uint timerID;
        ulong victim;
    }

    public struct GameObjectStats
    {
        int m_typeID;
        int m_displayID;
        string[] m_name; //[4];
        int[] m_propValue; //[10];
    }

    public struct GuildStats
    {
        uint m_guildID;
        string m_guildName; //[24];
        int m_emblemStyle;
        int m_emblemColor;
        int m_borderStyle;
        int m_borderColor;
        int m_backgroundColor;
    }

    public struct ItemStats
    {
        int m_class;
        int m_subclass;
        string m_displayName;
        int m_displayInfoID;
        int m_overallQualityID;
        int m_flags;
        int m_buyPrice;
        int m_sellPrice;
        int m_inventoryType;
        int m_allowableClass;
        int m_allowableRace;
        int m_itemLevel;
        int m_requiredLevel;
        int m_requiredSkill;
        int m_requiredSkillRank;
        int m_maxCount;
        int m_stackable;
        int m_containerSlots;
        int[] m_bonusStat; //[10];
        int[] m_bonusAmount; //[10];
        int[] m_minDamage; //[5];
        int[] m_maxDamage; //[5];
        int[] m_damageType; //[5];
        int[] m_resistances; //[6];
        int m_delay;
        int m_ammunitionType;
        int m_maxDurability;
        int[] m_spellID; //[5];
        int[] m_spellTrigger; //[5];
        int[] m_spellstringges; //[5];
        int[] m_spellCooldown; //[5];
        int[] m_spellCategory; //[5];
        int[] m_spellCategoryCooldown; //[5];
        int m_bonding;
        string m_description;
        int m_pageText;
        int m_languageID;
        int m_pageMaterial;
        int m_startQuestID;
        int m_lockID;
        int m_material;
        int m_sheatheType;
    }

    public struct QuestInfo
    {
        int id;
        int level;
        string name; //[64];
        int turnIn;
    }

    public struct QuestItemInfo
    {
        int rewardItemID;
        int rewardDisplayID;
        int rewardAmount;
        int choiceItemID;
        int choiceDisplayID;
        int choiceAmount;
        int requiredItemID;
        int requiredDisplayID;
        int requiredAmount;
    }

    public struct TradeSkillInfo
    {
        int spellID;
        TradeSkillCategories category;
        int classID;
        int subClassID;
        int invSlots;
        int itemLevel;
        int numAvailable;
        int enabled;
    }

    public struct TradeSkillSubClassInfo
    {
        int classID;
        int subClassID;
        int filteredCount;
        int enabled;
        int collapsed;
    }

    public struct VendorItemData
    {
        uint m_muid;
        uint m_itemType;
        uint m_itemDisplayID;
        int m_quantity;
        int m_price;
        int m_durability;
        int m_stackCount;
    };

    public struct Friend
    {
        string m_connected;
        string m_name;
        ulong guid;
        int m_level;
        int m_class;
        int m_area;
    };

    public struct FriendList
    {
        Friend[] m_friends; //[50];
        uint m_friendNamesPending;
        ulong m_selectedFriend;
        UInt64[] m_ignore; //[25];
        uint m_ignoreNamesPending;
        ulong m_selectedIgnore;
    }

    public struct TradeItemData
    {
        uint entryID;
        uint displayID;
        uint count;
        uint enchantmentID;
        ulong creator;
    }

    public struct CraftInfo
    {
        int spellID;
        int skillLine;
        CraftLevelCategories category;
    }

    public struct CraftSkillLineInfo
    {
        int skillLine;
        int filteredCount;
        int collapsed;
    }

    public struct PetitionVendorItem
    {
        uint m_muid;
        uint m_itemID;
        uint m_itemDisplayID;
        int m_price;
        int m_flags;
    }

    public struct SkillInfoData
    {
        int isProf;
        int skillID;
        int profLevel;
        string profName; //[64];
    }

    public struct ProficiencyInfo
    {
        int minLevel;
        int slot;
    }

    public struct CGBuffDesc
    {
        int m_auraIndex;
        int m_auraSpell;
        string m_auraFlags;
        int m_untilCancelled;
    }

    public struct TaxiNode
    {
        int id;
        float offsetx;
        float offsety;
    }

    public struct QuestLogInfo
    {
        int questID;
        int logIndex;
        int isHeader;
    }

    public struct QuestCache
    {
        public uint m_questId;
        public uint m_questType;
        public uint m_questLevel;
        public uint m_questSortID;
        public uint m_questInfoID;
        public uint m_rewardNextQuest;
        public uint m_rewardMoney;
        public uint m_startItem;
        public uint[] m_rewardItems; //[4];
        public UInt32[] m_rewardAmount; //[4];
        public ulong[] m_rewardChoiceItems; //[6];
        public UInt32[] m_rewardChoiceAmount; //[6];
        public uint m_POIContinent;
        public float m_POIx;
        public float m_POIy;
        public uint m_POIPriority;
        public string m_logTitle;
        public string m_logDescription;
        public string m_questDescription;
        public string m_areaDescription;
        public ulong[] m_monsterToKill; //[4];
        public UInt32[] m_monsterToKillQuantity; //[4];
        public ulong[] m_itemToGet; //[4];
        public UInt32[] m_itemToGetQuantity; //[4];
        public string[,] m_getDescription; //[4][64];
    }

    public struct TrainerServiceInfo
    {
        int spellID;
        int skillLine;
        uint moneyCost;
        string[] pointCost; //[2];
        string reqLevel;
        uint reqSkillLine;
        uint reqSkillRank;
        uint reqSkillStep;
        int[] reqAbility; //[3];
        string usable;
        int enabled;
    }

    public struct TrainerSkillLineInfo
    {
        int skillLine;
        uint[] numSkills; //[6];
        int enabled;
        int collapsed;
        int allCostPoints;
    }

    public struct WorldMapContinentInfo
    {
        int continentID;
        int mapAreaID;
        HashSet<int> zoneList;
        int[,] chunkZones; //[128][128];
                           // NTempest::CRect hitRect;
    }

    public struct WorldMapLandmarkInfo
    {
        int entryID;
        float x;
        float y;
        int isPortLoc;
    }

    #region DBC Structures

    public struct SpellRadiusRec
    {
        int m_ID;
        float m_radius;
        float m_radiusPerLevel;
        float m_radiusMax;
    }

    public struct SpellDurationRec
    {
        int m_ID;
        int m_duration;
        int m_durationPerLevel;
        int m_maxDuration;
    }

    public struct SpellRangeRec
    {
        int m_ID;
        float m_rangeMin;
        float m_rangeMax;
        int m_flags;
        string m_displayName_lang;
        int m_displayName_flag;
        string m_displayNameShort_lang;
        int m_displayNameShort_flag;
    }

    public struct AreaPOIRec
    {
        int m_ID;
        int m_importance;
        int m_icon;
        int m_factionID;
        float m_x;
        float m_y;
        float m_z;
        int m_continentID;
        int m_flags;
        string m_name_lang;
        int m_name_flag;
    }

    public struct PageTextMaterialRec
    {
        int m_ID;
        string m_name;
    }

    public struct SkillLineRec
    {
        int m_ID;
        int m_raceMask;
        int m_classMask;
        int m_excludeRace;
        int m_excludeClass;
        int m_categoryID;
        int m_skillType;
        int m_minstringLevel;
        int m_maxRank;
        int m_abandonable;
        string m_displayName_lang;
        int m_displayName_flag;
    }

    public struct ItemClassRec
    {
        int m_classID;
        int m_subclassMapID;
        int m_flags;
        string m_className_lang;
        int m_className_flag;
        int m_generatedID;
    }

    public struct ItemSubClassRec
    {
        int m_classID;
        int m_subClassID;
        int m_prerequisiteProficiency;
        int m_postrequisiteProficiency;
        int m_flags;
        int m_displayFlags;
        int m_weaponParrySeq;
        int m_weaponReadySeq;
        int m_weaponAttackSeq;
        int m_WeaponSwingSize;
        string m_displayName_lang;
        int m_displayName_flag;
        string m_verboseName_lang;
        int m_verboseName_flag;
        int m_generatedID;
    }

    public struct SpellFocusObjectRec
    {
        int m_ID;
        string m_name_lang;
        int m_name_flag;
    }

    public struct SkillLineAbilityRec
    {
        int m_ID;
        int m_skillLine;
        int m_spell;
        int m_raceMask;
        int m_classMask;
        int m_excludeRace;
        int m_excludeClass;
        int m_minSkillLineRank;
        int m_supercededBySpell;
        int m_trivialSkillLineRankHigh;
        int m_trivialSkillLineRankLow;
        int m_abandonable;
    }

    public struct BankBagSlotPricesRec
    {
        int m_ID;
        int m_Cost;
    }

    public struct SpellIconRec
    {
        int m_ID;
        string m_textureFilename;
    }

    public struct FactionRec
    {
        int m_ID;
        int m_reputationIndex;
        int[] m_reputationRaceMask; //[4];
        int[] m_reputationClassMask; //[4];
        int[] m_reputationBase; //[4];
        string m_name_lang;
        int m_name_flag;
    }

    public struct ChrClassesRec
    {
        int m_ID;
        int m_PlayerClass;
        int m_DamageBonusStat;
        int m_DisplayPower;
        string m_petNameToken;
        string m_name_lang;
        int m_name_flag;
    }

    public struct AreaTableRec
    {
        int m_ID;
        int m_AreaNumber;
        int m_ContinentID;
        int m_ParentAreaNum;
        int m_AreaBit;
        int m_flags;
        int m_SoundProviderPref;
        int m_SoundProviderPrefUnderwater;
        int m_MIDIAmbience;
        int m_MIDIAmbienceUnderwater;
        int m_ZoneMusic;
        int m_IntroSound;
        int m_IntroPriority;
        string m_AreaName_lang;
        int m_AreaName_flag;
    }

    public struct CharacterCreateCamerasRec
    {
        int m_Race;
        int m_Sex;
        int m_Camera;
        float m_Height;
        float m_Radius;
        float m_Target;
        int m_generatedID;
    }

    public struct FactionGroupRec
    {
        int m_ID;
        int m_maskID;
        string m_internalName;
        string m_name_lang;
        int m_name_flag;
    }

    public struct FactionTemplateRec
    {
        int m_ID;
        int m_faction;
        int m_factionGroup;
        int m_friendGroup;
        int m_enemyGroup;
        int[] m_enemies; //[4];
        int[] m_friend; //[4];
    };

    public struct CharBaseInfoRec
    {
        string m_raceID;
        string m_classID;
        int m_proficiency;
        int m_generatedID;
    }

    public struct CharStartOutfitRec
    {
        int m_ID;
        string m_raceID;
        string m_classID;
        string m_sexID;
        string m_outfitID;
        int[] m_ItemID; //[12];
        int[] m_DisplayItemID; //[12];
        int[] m_InventoryType; //[12];
    }

    public struct ItemVisualEffectsRec
    {
        int m_ID;
        string m_Model;
    }

    public struct CharVariationsRec
    {
        int m_RaceID;
        int m_SexID;
        int[] m_TextureHoldLayer; //[4];
        int m_generatedID;
    }

    public struct CharacterFacialHairStylesRec
    {
        int m_RaceID;
        int m_SexID;
        int m_VariationID;
        int m_BeardGeoset;
        int m_MoustacheGeoset;
        int m_SideburnGeoset;
        int m_generatedID;
    }

    public struct CharTextureVariationsV2Rec
    {
        int m_ID;
        int m_RaceID;
        int m_SexID;
        int m_SectionID;
        int m_VariationID;
        int m_ColorID;
        int m_IsNPC;
        string m_TextureName;
    }

    public struct CharHairGeosetsRec
    {
        int m_ID;
        int m_RaceID;
        int m_SexID;
        int m_VariationID;
        int m_GeosetID;
        int m_Showscalp;
    }

    public struct ChrRacesRec
    {
        int m_ID;
        int m_flags;
        int m_factionID;
        int m_MaleDisplayId;
        int m_FemaleDisplayId;
        string m_ClientPrefix;
        float m_MountScale;
        int m_BaseLanguage;
        int m_creatureType;
        int m_LoginEffectSpellID;
        int m_CombatStunSpellID;
        int m_ResSicknessSpellID;
        int m_SplashSoundID;
        int m_startingTaxiNodes;
        string m_clientFileString;
        int m_cinematicSequenceID;
        string m_name_lang;
        int m_name_flag;
    }

    public struct AreaMIDIAmbiencesRec
    {
        int m_ID;
        string m_DaySequence;
        string m_NightSequence;
        string m_DLSFile;
        float m_volume;
    }

    public struct SoundEntriesRec
    {
        int m_ID;
        int m_soundType;
        string m_name;
        string m_File;
        int[] m_Freq; //[10];
        string m_DirectoryBase;
        float m_volumeFloat;
        float m_pitch;
        float m_pitchVariation;
        int m_priority;
        int m_channel;
        int m_flags;
        float m_minDistance;
        float m_maxDistance;
        float m_distanceCutoff;
        int m_EAXDef;
    }

    public struct SoundWaterTypeRec
    {
        int m_ID;
        int m_soundType;
        int m_soundSubtype;
        int m_SoundID;
    }

    public struct ZoneMusicRec
    {
        int m_ID;
        float m_VolumeFloat;
        string[] m_MusicFile; //[2];
        int[] m_SilenceIntervalMin; //[2];
        int[] m_SilenceIntervalMax; //[2];
        int[] m_SegmentLength; //[2];
        int[] m_SegmentPlayMin; //[2];
        int[] m_SegmentPlayMax; //[2];
        int[] m_Sounds; //[2];
    }

    public struct SheatheSoundLookupsRec
    {
        int m_ID;
        int m_classID;
        int m_subclassID;
        int m_material;
        int m_checkMaterial;
        int m_sheatheSound;
        int m_unsheatheSound;
    }

    public struct SoundSamplePreferencesRec
    {
        int m_ID;
        float m_EAX1EffectLevel;
        int m_EAX2SampleDirect;
        int m_EAX2SampleDirectHF;
        int m_EAX2SampleRoom;
        int m_EAX2SampleRoomHF;
        float m_EAX2SampleObstruction;
        float m_EAX2SampleObstructionLFRatio;
        float m_EAX2SampleOcclusion;
        float m_EAX2SampleOcclusionLFRatio;
        float m_EAX2SampleOcclusionRoomRatio;
        float m_EAX2SampleRoomRolloff;
        float m_EAX2SampleAirAbsorption;
        int m_EAX2SampleOutsideVolumeHF;
        float m_EAX3SampleOcclusionDirectRatio;
        float m_EAX3SampleExclusion;
        float m_EAX3SampleExclusionLFRatio;
        float m_EAX3SampleDopplerFactor;
        float m_Fast2DPredelayTime;
        float m_Fast2DDamping;
        float m_Fast2DReverbTime;
    }

    public struct WeaponSwingSounds2Rec
    {
        int m_ID;
        int m_SwingType;
        int m_Crit;
        int m_SoundID;
    }

    public struct WeaponImpactSoundsRec
    {
        int m_ID;
        int m_WeaponSubClassID;
        int m_ParrySoundType;
        int[] m_impactSoundID; //[10];
        int[] m_critImpactSoundID; //[10];
    }

    public struct MaterialRec
    {
        int m_materialID;
        int m_flags;
        int m_foleySoundID;
    }

    public struct SoundProviderPreferencesRec
    {
        int m_ID;
        string m_Description;
        int m_Flags;
        int m_EAXEnvironmentSelection;
        float m_EAXEffectVolume;
        float m_EAXDecayTime;
        float m_EAXDamping;
        float m_EAX2EnvironmentSize;
        float m_EAX2EnvironmentDiffusion;
        int m_EAX2Room;
        int m_EAX2RoomHF;
        float m_EAX2DecayHFRatio;
        int m_EAX2Reflections;
        float m_EAX2ReflectionsDelay;
        int m_EAX2Reverb;
        float m_EAX2ReverbDelay;
        float m_EAX2RoomRolloff;
        float m_EAX2AirAbsorption;
        int m_EAX3RoomLF;
        float m_EAX3DecayLFRatio;
        float m_EAX3EchoTime;
        float m_EAX3EchoDepth;
        float m_EAX3ModulationTime;
        float m_EAX3ModulationDepth;
        float m_EAX3HFReference;
        float m_EAX3LFReference;
    }

    public struct VocalUISoundsRec
    {
        int m_ID;
        int m_vocalUIEnum;
        int m_raceID;
        int[] m_NormalSoundID; //[2];
        int[] m_PissedSoundID; //[2];
    }

    public struct ResistancesRec
    {
        int m_ID;
        int m_Flags;
        int m_FizzleSoundID;
        string m_name_lang;
        int m_name_flag;
    }

    public struct TerrainTypeRec
    {
        int m_TerrainID;
        string m_TerrainDesc;
        int m_FootstepSprayRun;
        int m_FootstepSprayWalk;
        int m_SoundID;
        int m_Flags;
        int m_generatedID;
    }

    public struct ItemGroupSoundsRec
    {
        int m_ID;
        int[] m_sound; //[4];
    }

    public struct FootstepTerrainLookupRec
    {
        int m_ID;
        int m_CreatureFootstepID;
        int m_TerrainSoundID;
        int m_SoundID;
        int m_SoundIDSplash;
    }

    public struct ChrProficiencyRec
    {
        int m_ID;
        int[] m_proficiency_minLevel; //[16];
        int[] m_proficiency_acquireMethod; //[16];
        int[] m_proficiency_itemClass; //[16];
        int[] m_proficiency_itemSubClassMask; //[16];
    }

    public struct PaperDollItemFrameRec
    {
        string m_ItemButtonName;
        string m_SlotIcon;
        int m_SlotNumber;
        int m_generatedID;
    }

    public struct SpellShapeshiftFormRec
    {
        int m_ID;
        int m_bonusActionBar;
        string m_name_lang;
        int m_name_flag;
        int m_flags;
    }

    public struct SpellRec
    {
        int m_ID;
        int m_school;
        int m_category;
        int m_castUI;
        int m_attributes;
        int m_attributesEx;
        int m_shapeshiftMask;
        int m_targets;
        int m_targetCreatureType;
        int m_requiresSpellFocus;
        int m_casterAuraState;
        int m_targetAuraState;
        int m_castingTimeIndex;
        int m_recoveryTime;
        int m_categoryRecoveryTime;
        int m_interruptFlags;
        int m_auraInterruptFlags;
        int m_channelInterruptFlags;
        int m_procFlags;
        int m_procChance;
        int m_procstringges;
        int m_maxLevel;
        int m_baseLevel;
        int m_spellLevel;
        int m_durationIndex;
        int m_powerType;
        int m_manaCost;
        int m_manaCostPerLevel;
        int m_manaPerSecond;
        int m_manaPerSecondPerLevel;
        int m_rangeIndex;
        float m_speed;
        int m_modalNextSpell;
        int[] m_totem; //[2];
        int[] m_reagent; //[8];
        int[] m_reagentCount; //[8];
        int m_equippedItemClass;
        int m_equippedItemSubclass;
        int[] m_effect; //[3];
        int[] m_effectDieSides; //[3];
        int[] m_effectBaseDice; //[3];
        int[] m_effectDicePerLevel; //[3];
        float[] m_effectRealPointsPerLevel; //[3];
        int[] m_effectBasePoints; //[3];
        int[] m_implicitTargetA; //[3];
        int[] m_implicitTargetB; //[3];
        int[] m_effectRadiusIndex; //[3];
        int[] m_effectAura; //[3];
        int[] m_effectAuraPeriod; //[3];
        float[] m_effectAmplitude; //[3];
        int[] m_effectChainTargets; //[3];
        int[] m_effectItemType; //[3];
        int[] m_effectMiscValue; //[3];
        int[] m_effectTriggerSpell; //[3];
        int m_spellVisualID;
        int m_spellIconID;
        int m_activeIconID;
        int m_spellPriority;
        string m_name_lang;
        int m_name_flag;
        string m_nameSubtext_lang;
        int m_nameSubtext_flag;
        string m_description_lang;
        int m_description_flag;
        int m_manaCostPct;
        int m_startRecoveryCategory;
        int m_startRecoveryTime;
    }

    public struct ItemDisplayInfoRec
    {
        int m_ID;
        string m_modelName; //[2];
        string m_modelTexture; //[2];
        string m_inventoryIcon;
        string m_groundModel;
        int[] m_geosetGroup; //[4];
        int m_flags;
        int m_spellVisualID;
        int m_groupSoundIndex;
        int m_itemSize;
        int m_helmetGeosetVisID;
        string m_texture; //[8];
        int m_itemVisual;
    }

    public struct ItemVisualsRec
    {
        int m_ID;
        int[] m_Slot; //[5];
    }

    public struct TaxiNodesRec
    {
        int m_ID;
        int m_ContinentID;
        float m_X;
        float m_Y;
        float m_Z;
        string m_Name_lang;
        int m_Name_flag;
    }

    public struct QuestSortRec
    {
        int m_ID;
        string m_SortName_lang;
        int m_SortName_flag;
    }

    public struct QuestInfoRec
    {
        int m_ID;
        string m_InfoName_lang;
        int m_InfoName_flag;
    }

    public struct SpellCastTimesRec
    {
        int m_ID;
        int m_base;
        int m_perLevel;
        int m_minimum;
    }

    public struct SpellItemEnchantmentRec
    {
        int m_ID;
        int[] m_effect; //[3];
        int[] m_effectPointsMin; //[3];
        int[] m_effectPointsMax; //[3];
        int[] m_effectArg; //[3];
        string m_name_lang;
        int m_name_flag;
        int m_itemVisual;
    }

    public struct WorldMapAreaRec
    {
        int m_ID;
        int m_mapID;
        int m_areaID;
        int m_leftBoundary;
        int m_rightBoundary;
        int m_topBoundary;
        int m_bottomBoundary;
        string m_areaName;
    }

    public struct WorldMapContinentRec
    {
        int m_ID;
        int m_mapID;
        int m_leftBoundary;
        int m_rightBoundary;
        int m_topBoundary;
        int m_bottomBoundary;
        float m_continentOffsetX;
        float m_continentOffsetY;
    }

    public struct WorldSafeLocsRec
    {
        int m_ID;
        int m_continent;
        float m_locX;
        float m_locY;
        float m_locZ;
        string m_AreaName_lang;
        int m_AreaName_flag;
    }

    public struct LanguagesRec
    {
        int m_ID;
        string m_name_lang;
        int m_name_flag;
    }

    public struct LanguageWordsRec
    {
        int m_ID;
        int m_languageID;
        string m_word;
    }

    public struct EmotesTextRec
    {
        int m_ID;
        string m_name;
        int m_emoteID;
        int[] m_emoteText; //[16];
    }

    public struct EmotesTextDataRec
    {
        int m_ID;
        string m_text_lang;
        int m_text_flag;
    }

    public struct CinematicCameraRec
    {
        int m_ID;
        string m_model;
        int m_soundID;
        float m_originX;
        float m_originY;
        float m_originZ;
        float m_originFacing;
    }

    public struct CinematicSequencesRec
    {
        int m_ID;
        int m_soundID;
        int[] m_camera; //[8];
    }

    public struct CameraShakesRec
    {
        int m_ID;
        int m_shakeType;
        int m_direction;
        float m_amplitude;
        float m_frequency;
        float m_duration;
        float m_phase;
        float m_coefficient;
    }

    public struct SpellEffectNamesRec
    {
        int m_EnumID;
        string m_name_lang;
        int m_name_flag;
        int m_generatedID;
    }

    public struct SpellAuraNamesRec
    {
        int m_EnumID;
        int m_specialMiscValue;
        string m_globalstrings_tag;
        string m_name_lang;
        int m_name_flag;
        int m_generatedID;
    }

    public struct SpellDispelTypeRec
    {
        int m_ID;
        string m_name_lang;
        int m_name_flag;
    }

    public struct CreatureTypeRec
    {
        int m_ID;
        string m_name_lang;
        int m_name_flag;
    }

    public struct LockRec
    {
        int m_ID;
        int[] m_Type; //[4];
        int[] m_Index; //[4];
        int[] m_Skill; //[4];
        int[] m_Action; //[4];
    }

    public struct LockTypeRec
    {
        int m_ID;
        string m_name_lang;
        int m_name_flag;
        string m_resourceName_lang;
        int m_resourceName_flag;
        string m_verb_lang;
        int m_verb_flag;
    }

    public struct WMOAreaTableRec
    {
        int m_ID;
        int m_WMOID;
        int m_NameSetID;
        int m_WMOGroupID;
        int m_DayAmbienceSoundID;
        int m_NightAmbienceSoundID;
        int m_SoundProviderPref;
        int m_SoundProviderPrefUnderwater;
        int m_MIDIAmbience;
        int m_MIDIAmbienceUnderwater;
        int m_ZoneMusic;
        int m_IntroSound;
        int m_IntroPriority;
        int m_Flags;
        string m_AreaName_lang;
        int m_AreaName_flag;
    }

    public struct NamesReservedRec
    {
        int m_ID;
        string m_Name;
    }

    public struct NamesProfanityRec
    {
        int m_ID;
        string m_Name;
    }

    public struct SpellChainEffectsRec
    {
        int m_ID;
        float m_AvgSegLen;
        float m_Width;
        float m_NoiseScale;
        float m_TexCoordScale;
        int m_SegDuration;
        int m_SegDelay;
        string m_Texture;
    }

    public struct TransportAnimationRec
    {
        int m_ID;
        int m_TransportID;
        int m_TimeIndex;
        float m_PosX;
        float m_PosY;
        float m_PosZ;
    }

    public struct SpellVisualPrecastTransitionsRec
    {
        int m_ID;
        string m_PrecastLoadAnimName;
        string m_PrecastHoldAnimName;
    }

    public struct EmotesRec
    {
        int m_ID;
        int m_EmoteAnimID;
        int m_EmoteFlags;
        int m_EmoteSpecProc;
        int m_EmoteSpecProcParam;
    }

    public struct EmoteAnimsRec
    {
        int m_ID;
        int m_ProcessedAnimIndex;
        string m_AnimName;
    }

    struct UnitBloodRec
    {
        int m_ID;
        int[] m_CombatBloodSpurtFront; //[2];
        int[] m_CombatBloodSpurtBack; //[2];
        string[] m_GroundBlood; //[5];
    }

    public struct DeathThudLookupsRec
    {
        int m_ID;
        int m_SizeClass;
        int m_TerrainTypeSoundID;
        int m_SoundEntryID;
        int m_SoundEntryIDWater;
    }

    public struct TaxiPathNodeRec
    {
        int m_ID;
        int m_PathID;
        int m_NodeIndex;
        int m_ContinentID;
        float m_LocX;
        float m_LocY;
        float m_LocZ;
        int m_flags;
    }

    public struct TaxiPathRec
    {
        int m_ID;
        int m_FromTaxiNode;
        int m_ToTaxiNode;
        int m_Cost;
    }

    public struct GameObjectDisplayInfoRec
    {
        int m_ID;
        string m_modelName;
        int[] m_Sound; //[10];
    }

    public struct StringLookupsRec
    {
        int m_ID;
        string m_String;
    }

    public struct SpellEffectCameraShakesRec
    {
        int m_ID;
        int[] m_CameraShake; //[3];
    }

    public struct FootprintTexturesRec
    {
        int m_ID;
        string m_FootstepFilename;
    }

    public struct UISoundLookupsRec
    {
        int m_ID;
        int m_SoundID;
        string m_SoundName;
    }

    public struct AttackAnimTypesRec
    {
        int m_AnimID;
        string m_AnimName;
    }

    public struct AttackAnimKitsRec
    {
        int m_ID;
        int m_ItemSubclassID;
        int m_AnimTypeID;
        int m_AnimFrequency;
        int m_WhichHand;
    }

    public struct SpellVisualAnimNameRec
    {
        int m_AnimID;
        string m_name;
        int m_generatedID;
    }

    public struct AreaTriggerRec
    {
        int m_ID;
        int m_ContinentID;
        float m_x;
        float m_y;
        float m_z;
        float m_radius;
    }

    public struct SpellVisualEffectNameRec
    {
        int m_ID;
        string m_fileName;
        int m_specialID;
        int m_specialAttachPoint;
        float m_areaEffectSize;
        int m_VisualEffectNameFlags;
    }

    public struct SpellVisualRec
    {
        int m_ID;
        int m_precastKit;
        int m_castKit;
        int m_impactKit;
        int m_stateKit;
        int m_channelKit;
        int m_hasMissile;
        int m_missileModel;
        int m_missilePathType;
        int m_missileDestinationAttachment;
        int m_missileSound;
        int m_hasAreaEffect;
        int m_areaModel;
        int m_areaKit;
        int m_animEventSoundID;
        string m_weaponTrailRed;
        string m_weaponTrailGreen;
        string m_weaponTrailBlue;
        string m_weaponTrailAlpha;
        string m_weaponTrailFadeoutRate;
        int m_weaponTrailDuration;
    }

    public struct TabardEmblemTexturesRec
    {
        int m_ID;
        string[] m_TorsoTexture; //[2];
    }

    public struct TabardBackgroundTexturesRec
    {
        int m_ID;
        string[] m_TorsoTexture; //[2];
    }

    public struct CreatureFamilyRec
    {
        int m_ID;
        float m_minScale;
        int m_minScaleLevel;
        float m_maxScale;
        int m_maxScaleLevel;
        int[] m_skillLine; //[2];
    }

    public struct GroundEffectDoodadRec
    {
        int m_ID;
        int m_doodadIdTag;
        string m_doodadpath;
    }

    public struct GroundEffectTextureRec
    {
        int m_ID;
        int m_datestamp;
        int m_continentId;
        int m_zoneId;
        int m_textureId;
        string m_textureName;
        int[] m_doodadId; //[4];
        int m_density;
        int m_sound;
    }

    #endregion

    #region Damage Structs
    public struct ENVIRONMENTALDAMAGE
    {
        ulong victim;
        int school;
        int amount;
    }

    public struct ENCHANTMENTLOG
    {
        ulong attacker;
        ulong victim;
        int enchantment;
        int itemID;
        int flags;
    }

    public struct RESISTLOG
    {
        ulong attacker;
        ulong victim;
        int spell;
        float resistRollNeeded;
        float resistRoll;
        int flags;
        int castLevel;
    }

    public struct PARTYKILLLOG
    {
        ulong killer;
        ulong victim;
    }

    public struct SPELLMISSLOG
    {
        ulong attacker;
        ulong victim;
        uint spellID;
        uint reason;
        float hitRoll;
        float hitRollNeeded;
        float dodgeRoll;
        float dodgeRollNeeded;
        float parryRoll;
        float parryRollNeeded;
        float blockRoll;
        float blockRollNeeded;
        uint flags;
    }

    public struct DAMAGELOGBASE
    {
        uint auraEffectID;
        uint spellID;
        uint damageType;
        float resistanceCoefficient;
    }

    public struct LOGBASE
    {
        int damage;
        ulong victim;
        int amount;
    }
    #endregion

    #region Item Structs
    /*public struct LootItem
    {
        uint m_itemID;
        uint m_displayID;
        uint m_quantity;
    }*/

    public struct ITEMSWAP
    {
        ulong bagA;
        ulong bagB;
        int slotA;
        int slotB;
        int pendingID;
    }

    public struct CGLootSlot
    {
        int pending;
        int itemID;
        int itemDisplayID;
        int quantity;
        string slot;
    }
    #endregion

    #region Spell Structs
    public struct Ranges
    {
        float m_min;
        float m_max;
    }

    public struct RangeList
    {
        int m_numranges;
        Ranges[] m_ranges;
    }

    public struct SpellCastResult
    {
        ulong caster;
        ulong casterUnit;
        int spellID;
        UInt16 targets;
        ulong unitTarget;
        ulong itemTarget;
        ulong selectedTarget;
        Vector sourceLocation;
        Vector destLocation;
        float destFacing;
        uint destZoneID;
        uint castTime;
        uint castEndTime;
        int spellIndex;
        uint spellLevel;
        ulong ammoItem;
        ulong reflector;
        string targetString; //[128];
        int overrideRank;
        short flags;
    }
    #endregion


}

