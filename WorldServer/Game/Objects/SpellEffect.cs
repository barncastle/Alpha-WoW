using Common.Constants;
using Common.Database.DBC;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorldServer.Game.Objects.PlayerExtensions.Quests;
using WorldServer.Game.Structs;

namespace WorldServer.Game.Objects
{
    public class SpellEffect
    {
        public static Dictionary<SpellEffects, SpellEffectHandler> EffectHandlers = new Dictionary<SpellEffects, SpellEffectHandler>();
        public delegate SpellFailedReason SpellEffectHandler(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item);

        public static void InitSpellEffects()
        {
            LoadCommandDefinitions();
        }

        public static void DefineEffect(SpellEffects command, SpellEffectHandler handler)
        {
            EffectHandlers[command] = handler;
        }

        public static SpellFailedReason InvokeHandler(SpellEffects command, SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            if (EffectHandlers.ContainsKey(command))
                return EffectHandlers[command]?.Invoke(Spell, Targets, Index, Item) ?? SpellFailedReason.SPELL_FAILED_NO_REASON;
            else
                return SpellFailedReason.SPELL_FAILED_ERROR;
        }

        public static void LoadCommandDefinitions()
        {
            DefineEffect(SpellEffects.SPELL_EFFECT_NONE, SPELL_EFFECT_NONE);
            DefineEffect(SpellEffects.SPELL_EFFECT_INSTAKILL, SPELL_EFFECT_INSTAKILL);
            DefineEffect(SpellEffects.SPELL_EFFECT_SCHOOL_DAMAGE, SPELL_EFFECT_SCHOOL_DAMAGE);
            DefineEffect(SpellEffects.SPELL_EFFECT_DUMMY, SPELL_EFFECT_DUMMY);
            DefineEffect(SpellEffects.SPELL_EFFECT_TELEPORT_UNITS, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_APPLY_AURA, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_POWER_DRAIN, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_HEALTH_LEECH, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_HEAL, SPELL_EFFECT_HEAL);
            DefineEffect(SpellEffects.SPELL_EFFECT_BIND, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_PORTAL, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_QUEST_COMPLETE, SPELL_EFFECT_QUEST_COMPLETE); 
            DefineEffect(SpellEffects.SPELL_EFFECT_WEAPON_DAMAGE, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_RESURRECT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_EXTRA_ATTACKS, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_DODGE, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_EVADE, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_PARRY, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_BLOCK, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_CREATE_ITEM, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_WEAPON, SPELL_EFFECT_UNUSED);
            DefineEffect(SpellEffects.SPELL_EFFECT_DEFENSE, SPELL_EFFECT_UNUSED);
            DefineEffect(SpellEffects.SPELL_EFFECT_PERSISTENT_AREA_AURA, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_LEAP, SPELL_EFFECT_LEAP);
            DefineEffect(SpellEffects.SPELL_EFFECT_ENERGIZE, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_WEAPON_PERC_DMG, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_OPEN_LOCK, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_MOUNT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_APPLY_AREA_AURA, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_LEARN_SPELL, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SPELL_DEFENSE, SPELL_EFFECT_UNUSED);
            DefineEffect(SpellEffects.SPELL_EFFECT_DISPEL, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_LANGUAGE, SPELL_EFFECT_UNUSED);
            DefineEffect(SpellEffects.SPELL_EFFECT_DUAL_WIELD, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_WILD, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_GUARDIAN, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SKILL_STEP, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SPAWN, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SPELL_CAST_UI, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_STEALTH, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_DETECT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_OBJECT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_GUARANTEE_HIT, SPELL_EFFECT_UNUSED);
            DefineEffect(SpellEffects.SPELL_EFFECT_ENCHANT_ITEM_PERMANENT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_ENCHANT_ITEM_TEMPORARY, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_TAME_CREATURE, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_PET, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_LEARN_PET_SPELL, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_WEAPON_DAMAGE_PLUS, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_OPEN_LOCK_ITEM, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_PROFICIENCY, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SEND_EVENT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_POWER_BURN, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_THREAT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_TRIGGER_SPELL, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_HEAL_MAX_HEALTH, SPELL_EFFECT_HEAL_MAX_HEALTH);
            DefineEffect(SpellEffects.SPELL_EFFECT_INTERRUPT_CAST, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_DISTRACT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_PULL, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_PICKPOCKET, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_ADD_FARSIGHT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_POSSESSED, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_TOTEM, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_OBJECT_WILD, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SCRIPT_EFFECT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_ATTACK, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SANCTUARY, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_ADD_COMBO_POINTS, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_CREATE_HOUSE, SPELL_EFFECT_UNUSED);
            DefineEffect(SpellEffects.SPELL_EFFECT_BIND_SIGHT, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_DUEL, SPELL_EFFECT_UNUSED);
            DefineEffect(SpellEffects.SPELL_EFFECT_STUCK, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_SUMMON_PLAYER, null);
            DefineEffect(SpellEffects.SPELL_EFFECT_ACTIVATE_OBJECT, null);
        }

        public static SpellFailedReason SPELL_EFFECT_UNUSED(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_NONE(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_INSTAKILL(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            foreach (WorldObject t in Targets)
                if (t.IsTypeOf(ObjectTypes.TYPE_UNIT))
                    ((Creature)t).Die(Spell.Caster);

            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_SCHOOL_DAMAGE(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            int damage = 0;
            int current = 0;

            foreach(WorldObject t in Targets)
            {
                if (!t.IsTypeOf(ObjectTypes.TYPE_UNIT))
                    continue;

                Unit u = (Unit)t;
                damage = Spell.GetValue(u, Index);

                if (current > 0)
                    damage *= 1 ^ current;

                u.DealSpellDamage(Spell, damage, SpellDamageType.SPELL_TYPE_NONMELEE, Index);
            }

            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_DUMMY(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_HEAL(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            int damage = 0;
            int current = 0;
            foreach(WorldObject obj in Targets)
            {
                if (!obj.IsTypeOf(ObjectTypes.TYPE_UNIT))
                    continue;

                damage = Spell.GetValue((Unit)obj, Index);
                if(current > 0)
                    damage *= 1 ^ current;

                ((Unit)obj).DealSpellDamage(Spell, damage, SpellDamageType.SPELL_TYPE_HEAL, Index);
            }

            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_QUEST_COMPLETE(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            foreach (WorldObject obj in Targets)
            {
                if (!obj.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                    continue;

                Player p = ((Player)obj);
                if (p.HasQuest(Spell.Spell.EffectMiscValue[Index]))
                {
                    p.Quests[Spell.Spell.EffectMiscValue[Index]].Status = QuestStatuses.QUEST_STATUS_COMPLETE;
                    p.UpdateSurroundingQuestStatus();
                }
            }

            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_LEAP(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            float radius = DBC.SpellRadius[(int)Spell.Spell.EffectRadiusIndex[Index]].m_radius;
            float newX = (float)(Spell.Caster.Location.X + Math.Cos(Spell.Caster.Orientation) * radius);
            float newY = (float)(Spell.Caster.Location.Y + Math.Sin(Spell.Caster.Orientation) * radius);
            float newZ = (float)(Spell.Caster.Location.Z);
            if(Math.Abs(Spell.Caster.Location.Z - newZ) > radius)
            {
                newX = Spell.Caster.Location.X;
                newY = Spell.Caster.Location.Y;
                newZ = Spell.Caster.Location.Z;
            }

            if (Spell.Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                ((Player)Spell.Caster).Teleport(Spell.Caster.Map, new Quaternion(newX, newY, newZ, Spell.Caster.Orientation));
            else
                Spell.Caster.Location = new Vector(newX, newY, newZ);

            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }

        public static SpellFailedReason SPELL_EFFECT_HEAL_MAX_HEALTH(SpellCast Spell, List<WorldObject> Targets, int Index, Item Item)
        {
            foreach(WorldObject obj in Targets)
            {
                if (!obj.IsTypeOf(ObjectTypes.TYPE_UNIT))
                    continue;
                ((Unit)obj).Health.ResetCurrent();
            }

            return SpellFailedReason.SPELL_FAILED_NO_REASON;
        }
    }
}