using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Game.Structs
{
    // 0 GAMEOBJECT_TYPE_DOOR
    struct Door
    {
        uint startOpen;                               // 0 used client side to determine GO_ACTIVATED means open/closed
        uint lockId;                                  // 1 -> Lock.dbc
        uint autoCloseTime;                           // 2 secs till autoclose = autoCloseTime / 0x10000
        uint noDamageImmune;                          // 3 break opening whenever you recieve damage?
        uint openTextID;                              // 4 can be used to replace castBarCaption?
        uint closeTextID;                             // 5
    }
    // 1 GAMEOBJECT_TYPE_BUTTON
    struct Button
    {
        uint startOpen;                               // 0
        uint lockId;                                  // 1 -> Lock.dbc
        uint autoCloseTime;                           // 2 secs till autoclose = autoCloseTime / 0x10000
        uint linkedTrapId;                            // 3
        uint noDamageImmune;                          // 4 isBattlegroundObject
        uint large;                                   // 5
        uint openTextID;                              // 6 can be used to replace castBarCaption?
        uint closeTextID;                             // 7
        uint losOK;                                   // 8
    }
    // 2 GAMEOBJECT_TYPE_QUESTGIVER
    struct QuestGiver
    {
        uint lockId;                                  // 0 -> Lock.dbc
        uint questList;                               // 1
        uint pageMaterial;                            // 2
        uint gossipID;                                // 3
        uint customAnim;                              // 4
        uint noDamageImmune;                          // 5
        uint openTextID;                              // 6 can be used to replace castBarCaption?
        uint losOK;                                   // 7
        uint allowMounted;                            // 8
        uint large;                                   // 9
    }
    // 3 GAMEOBJECT_TYPE_CHEST
    struct Chest
    {
        uint lockId;                                  // 0 -> Lock.dbc
        uint lootId;                                  // 1
        uint chestRestockTime;                        // 2
        uint consumable;                              // 3
        uint minSuccessOpens;                         // 4
        uint maxSuccessOpens;                         // 5
        uint eventId;                                 // 6 lootedEvent
        uint linkedTrapId;                            // 7
        uint questId;                                 // 8 not used currently but store quest required for GO activation for player
        uint level;                                   // 9
        uint losOK;                                   // 10
        uint leaveLoot;                               // 11
        uint notInCombat;                             // 12
        uint logLoot;                                 // 13
        uint openTextID;                              // 14 can be used to replace castBarCaption?
        uint groupLootRules;                          // 15
    }
    // 4 GAMEOBJECT_TYPE_BINDER - empty
    // 5 GAMEOBJECT_TYPE_GENERIC
    struct Generic
    {
        uint floatingTooltip;                         // 0
        uint highlight;                               // 1
        uint serverOnly;                              // 2
        uint large;                                   // 3
        uint floatOnWater;                            // 4
        uint questID;                                 // 5
    }
    // 6 GAMEOBJECT_TYPE_TRAP
    struct Trap
    {
        uint lockId;                                  // 0 -> Lock.dbc
        uint level;                                   // 1
        uint radius;                                  // 2 radius for trap activation
        uint spellId;                                 // 3
        uint charges;                                 // 4 need respawn (if > 0)
        uint cooldown;                                // 5 time in secs
        uint autoCloseTime;                           // 6
        uint startDelay;                              // 7
        uint serverOnly;                              // 8
        uint stealthed;                               // 9
        uint large;                                   // 10
        uint stealthAffected;                         // 11
        uint openTextID;                              // 12 can be used to replace castBarCaption?
        uint closeTextID;                             // 13
    }
    // 7 GAMEOBJECT_TYPE_CHAIR
    struct Chair
    {
        uint slots;                                   // 0
        uint height;                                  // 1
        uint onlyCreatorUse;                          // 2
    }
    // 8 GAMEOBJECT_TYPE_SPELL_FOCUS
    struct SpellFocus
    {
        uint focusId;                                 // 0
        uint dist;                                    // 1
        uint linkedTrapId;                            // 2
        uint serverOnly;                              // 3
        uint questID;                                 // 4
        uint large;                                   // 5
    }
    // 9 GAMEOBJECT_TYPE_TEXT
    struct Text
    {
        uint pageID;                                  // 0
        uint language;                                // 1
        uint pageMaterial;                            // 2
        uint allowMounted;                            // 3
    }
    // 10 GAMEOBJECT_TYPE_GOOBER
    struct Goober
    {
        uint lockId;                                  // 0 -> Lock.dbc
        uint questId;                                 // 1
        uint eventId;                                 // 2
        uint autoCloseTime;                           // 3
        uint customAnim;                              // 4
        uint consumable;                              // 5
        uint cooldown;                                // 6
        uint pageId;                                  // 7
        uint language;                                // 8
        uint pageMaterial;                            // 9
        uint spellId;                                 // 10
        uint noDamageImmune;                          // 11
        uint linkedTrapId;                            // 12
        uint large;                                   // 13
        uint openTextID;                              // 14 can be used to replace castBarCaption?
        uint closeTextID;                             // 15
        uint losOK;                                   // 16 isBattlegroundObject
        uint allowMounted;                            // 17
        uint floatingTooltip;                         // 18
        uint gossipID;                                // 19
    }
    // 11 GAMEOBJECT_TYPE_TRANSPORT
    struct Transport
    {
        uint pause;                                   // 0
        uint startOpen;                               // 1
        uint autoCloseTime;                           // 2 secs till autoclose = autoCloseTime / 0x10000
    }
    // 12 GAMEOBJECT_TYPE_AREADAMAGE
    struct AreaDamage
    {
        uint lockId;                                  // 0
        uint radius;                                  // 1
        uint damageMin;                               // 2
        uint damageMax;                               // 3
        uint damageSchool;                            // 4
        uint autoCloseTime;                           // 5 secs till autoclose = autoCloseTime / 0x10000
        uint openTextID;                              // 6
        uint closeTextID;                             // 7
    }
    // 13 GAMEOBJECT_TYPE_CAMERA
    struct Camera
    {
        uint lockId;                                  // 0 -> Lock.dbc
        uint cinematicId;                             // 1
        uint eventID;                                 // 2
        uint openTextID;                              // 3 can be used to replace castBarCaption?
    }
    // 14 GAMEOBJECT_TYPE_MAPOBJECT - empty
    // 15 GAMEOBJECT_TYPE_MO_TRANSPORT
    struct MoTransport
    {
        uint taxiPathId;                              // 0
        uint moveSpeed;                               // 1
        uint accelRate;                               // 2
        uint startEventID;                            // 3
        uint stopEventID;                             // 4
        uint transportPhysics;                        // 5
        uint mapID;                                   // 6
    }
    // 16 GAMEOBJECT_TYPE_DUELFLAG - empty
    // 17 GAMEOBJECT_TYPE_FISHINGNODE
    struct FishNode
    {
        uint _data0;                                  // 0
        uint lootId;                                  // 1
    }
    // 18 GAMEOBJECT_TYPE_SUMMONING_RITUAL
    struct SummoningRitual
    {
        uint reqParticipants;                         // 0
        uint spellId;                                 // 1
        uint animSpell;                               // 2
        uint ritualPersistent;                        // 3
        uint casterTargetSpell;                       // 4
        uint casterTargetSpellTargets;                // 5
        uint castersGrouped;                          // 6
        uint ritualNoTargetCheck;                     // 7
    }
}
