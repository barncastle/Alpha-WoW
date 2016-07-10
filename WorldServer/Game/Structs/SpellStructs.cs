using Common.Constants;
using Common.Database.DBC;
using Common.Database.DBC.Structures;
using Common.Helpers;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game.Managers;
using WorldServer.Game.Objects;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Storage;

namespace WorldServer.Game.Structs
{
    public class SpellCast
    {
        public Unit Caster;
        public SpellTargets Targets;
        public SpellSchoolMask SchoolMask;
        public Spell Spell;
        public CurrentSpellType SpellType = CurrentSpellType.CURRENT_GENERIC_SPELL;
        public volatile SpellState State = SpellState.SPELL_STATE_DELAYED;
        public bool Triggered = false;
        public bool CanReflect = false;
        public long Timer = 0;
        public int Duration = 0;
        public bool IsFinished { get { return State == SpellState.SPELL_STATE_FINISHED; } }
        public uint PowerCost = 0;
        public SpellCastTimes CastTime;

        private Vector m_location;
        private float m_orientation;
        private uint m_health;
        private int[] m_basepoints = new int[3];

        public SpellCast(Unit caster)
        {
            this.Caster = caster;
        }

        public void Initialize()
        {
            this.m_location = Caster.Location;
            this.m_orientation = Caster.Orientation;
            this.m_health = Caster.Health.Current;

            for (int i = 0; i < 3; i++)
                m_basepoints[i] = (int)(Spell.EffectBaseDice[i] + Spell.EffectBasePoints[i]);

            if (SchoolMask == SpellSchoolMask.SPELL_SCHOOL_MASK_MAGIC)
                this.SchoolMask = (SpellSchoolMask)(1 << (int)Spell.School);

            if (SchoolMask == SpellSchoolMask.SPELL_SCHOOL_MASK_NORMAL)
                SpellType = CurrentSpellType.CURRENT_MELEE_SPELL;
            else if (Spell.ChannelInterruptFlags != 0)
                SpellType = CurrentSpellType.CURRENT_CHANNELED_SPELL;

            this.PowerCost = SetPowerCost();
        }

        public void Update()
        {
            if (Flag.HasFlag(this.Spell.ChannelInterruptFlags, (uint)SpellChannelInterruptFlags.CHANNEL_FLAG_MOVEMENT) && this.m_location != Caster.Location)
            {
                Cancel();
                Caster.SendCastResult(SpellFailedReason.SPELL_FAILED_MOVING, Spell.Id);
                return;
            }

            if (Flag.HasFlag(Spell.InterruptFlags, (uint)SpellInterruptFlags.SPELL_INTERRUPT_FLAG_MOVEMENT) && (this.m_location != Caster.Location))
            {
                Cancel();
                Caster.SendCastResult(SpellFailedReason.SPELL_FAILED_MOVING, Spell.Id);
                return;
            }

            if (Flag.HasFlag(Spell.ChannelInterruptFlags, (uint)SpellChannelInterruptFlags.CHANNEL_FLAG_TURNING) && this.m_orientation != Caster.Orientation)
            {
                Cancel();
                Caster.SendCastResult(SpellFailedReason.SPELL_FAILED_MOVING, Spell.Id);
                return;
            }

            if (Flag.HasFlag(Spell.ChannelInterruptFlags, (uint)SpellChannelInterruptFlags.CHANNEL_FLAG_DAMAGE) && Caster.Health.Current < this.m_health)
            {
                Cancel();
                Caster.SendCastResult(SpellFailedReason.SPELL_FAILED_INTERRUPTED_COMBAT, Spell.Id);
                return;
            }

            if (Flag.HasFlag(Spell.InterruptFlags, (uint)SpellInterruptFlags.SPELL_INTERRUPT_FLAG_ABORT_ON_DMG) && Caster.Health.Current < this.m_health)
            {
                Cancel();
                Caster.SendCastResult(SpellFailedReason.SPELL_FAILED_INTERRUPTED_COMBAT, Spell.Id);
                return;
            }

            //SendChannelUpdate((uint)(this.Duration - (Globals.Time - this.Timer)));
        }

        public void Delay(float sec)
        {
            this.Timer += (long)(sec * TimeSpan.TicksPerSecond);
        }

        public void Cancel()
        {
            SendChannelUpdate(0);
            SendInterrupted(0);
            State = SpellState.SPELL_STATE_FINISHED;
        }


        public Dictionary<WorldObject, SpellMissInfo> GetTargets(byte index)
        {
            List<WorldObject> targets = new List<WorldObject>();
            if (Spell.Effect[index] <= 0)
                return new Dictionary<WorldObject, SpellMissInfo>();

            for (byte j = 0; j <= 1; j++)
            {
                SpellImplicitTargets ImplicitTarget = (SpellImplicitTargets)Spell.EffectImplicitTargetA[index];
                if (j == 1)
                    ImplicitTarget = (SpellImplicitTargets)Spell.EffectImplicitTargetB[index];

                if (ImplicitTarget == SpellImplicitTargets.TARGET_NOTHING)
                    continue;

                List<WorldObject> EnemyTargets = new List<WorldObject>();
                List<WorldObject> PartyTargets = new List<WorldObject>();
                Unit Ref = Caster;

                switch (ImplicitTarget)
                {
                    case SpellImplicitTargets.TARGET_ALL_ENEMY_IN_AREA:
                    case SpellImplicitTargets.TARGET_ALL_ENEMY_IN_AREA_INSTANT:
                        if (Flag.HasFlag(Targets.TargetMask, (uint)SpellTargetType.TARGET_TYPE_LOCATION))
                            EnemyTargets = GetEnemyAtPoint(Caster, Targets.TargetLocation, Spell.EffectRadiusIndex[index]);
                        else
                        {
                            //TODO
                            //if (Caster.IsTypeOf(ObjectTypes.TYPE_DYNAMICOBJECT))
                            //    EnemyTargets = GetEnemyAtPoint(((DynamicObjectObject)Caster).Caster, Caster.Location, Spell.EffectRadiusIndex[index]);
                            //else
                            EnemyTargets = GetEnemyAtPoint(Caster, Caster.Location, Spell.EffectRadiusIndex[index]);
                        }

                        foreach (WorldObject EnemyTarget in EnemyTargets)
                            if (!targets.Contains(EnemyTarget))
                                targets.Add(EnemyTarget);

                        break;
                    case SpellImplicitTargets.TARGET_ALL_FRIENDLY_UNITS_AROUND_CASTER:
                        EnemyTargets = GetEnemyAtPoint(Caster, Ref.Location, Spell.EffectRadiusIndex[index]);
                        foreach (WorldObject EnemyTarget in EnemyTargets)
                        {
                            if (!targets.Contains(EnemyTarget))
                                targets.Add(EnemyTarget);
                        }

                        break;
                    case SpellImplicitTargets.TARGET_ALL_PARTY:
                        if (Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER) && ((Player)Caster).Group != null)
                        {
                            foreach (WorldObject PartyTarget in ((Player)Caster).Group.Members.Values)
                            {
                                if (!targets.Contains(PartyTarget))
                                    targets.Add(PartyTarget);
                            }
                        }

                        break;
                    case SpellImplicitTargets.TARGET_ALL_PARTY_AROUND_CASTER_2:
                    case SpellImplicitTargets.TARGET_AROUND_CASTER_PARTY:
                    case SpellImplicitTargets.TARGET_AREAEFFECT_PARTY:
                        if (Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER) && ((Player)Caster).Group != null)
                        {
                            PartyTargets = new List<WorldObject>();
                            //TODO Totem
                            //if (Caster is TotemObject)
                            //{
                            //    PartyTargets = GetPartyMembersAtPoint(((TotemObject)Caster).Caster, Spell.EffectRadiusIndex[index], Caster.positionX, Caster.positionY, Caster.positionZ);
                            //}
                            //else
                            PartyTargets = ((Player)Caster).Group.GetGroupInRange(Caster, Spell.EffectRadiusIndex[index]).ToList<WorldObject>();

                            foreach (WorldObject PartyTarget in PartyTargets)
                            {
                                if (!targets.Contains(PartyTarget))
                                    targets.Add(PartyTarget);
                            }
                        }
                        break;
                    case SpellImplicitTargets.TARGET_CHAIN_DAMAGE:
                    case SpellImplicitTargets.TARGET_CHAIN_HEAL:
                        List<WorldObject> UsedTargets = new List<WorldObject>();
                        WorldObject TargetUnit = null;

                        if (!targets.Contains(Targets.Target))
                            targets.Add(Targets.Target);

                        UsedTargets.Add(Targets.Target);
                        TargetUnit = Targets.Target;

                        if (Spell.EffectChainTarget[index] > 1)
                        {
                            for (byte k = 2; k <= Spell.EffectChainTarget[index]; k++)
                            {
                                EnemyTargets = GetEnemyAtPoint((Unit)TargetUnit, Caster.Location, 10f);
                                TargetUnit = null;
                                float LowHealth = 1.01f;
                                float TmpLife = 0;
                                foreach (Unit tmpUnit in EnemyTargets)
                                {
                                    if (!UsedTargets.Contains(tmpUnit))
                                    {
                                        TmpLife = (tmpUnit.Health.Current / tmpUnit.Health.Maximum);
                                        if (TmpLife < LowHealth)
                                        {
                                            LowHealth = TmpLife;
                                            TargetUnit = tmpUnit;
                                        }
                                    }
                                }

                                if (TargetUnit != null)
                                {
                                    if (!targets.Contains(TargetUnit))
                                        targets.Add(TargetUnit);
                                    UsedTargets.Add(TargetUnit);
                                }
                                else
                                    break;
                            }
                        }
                        break;
                    case SpellImplicitTargets.TARGET_AROUND_CASTER_ENEMY:
                        EnemyTargets = GetEnemyAtPoint(Caster, Ref.Location, Spell.EffectRadiusIndex[index]);
                        foreach (WorldObject EnemyTarget in EnemyTargets)
                            if (!targets.Contains(EnemyTarget))
                                targets.Add(EnemyTarget);
                        break;
                    case SpellImplicitTargets.TARGET_DYNAMIC_OBJECT:
                        if (Targets.Target != null && Targets.Target.IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
                            targets.Add(Targets.Target);
                        break;
                    case SpellImplicitTargets.TARGET_INFRONT:
                        EnemyTargets = GetEnemyInFrontOfMe(Caster, Spell.EffectRadiusIndex[index]);
                        foreach (WorldObject EnemyTarget in EnemyTargets)
                            if (!targets.Contains(EnemyTarget))
                                targets.Add(EnemyTarget);
                        break;
                    case SpellImplicitTargets.TARGET_BEHIND_VICTIM:
                        break;
                    //TODO: Behind victim? What spells has this really?
                    case SpellImplicitTargets.TARGET_GAMEOBJECT_AND_ITEM:
                    case SpellImplicitTargets.TARGET_SELECTED_GAMEOBJECT:
                        if (Targets.Target != null && Targets.Target.IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
                            targets.Add(Targets.Target);
                        break;
                    case SpellImplicitTargets.TARGET_SELF:
                    case SpellImplicitTargets.TARGET_SELF2:
                    case SpellImplicitTargets.TARGET_SELF_FISHING:
                    case SpellImplicitTargets.TARGET_MASTER:
                    case SpellImplicitTargets.TARGET_DUEL_VS_PLAYER:
                        if (!targets.Contains(Caster))
                            targets.Add(Caster);
                        break;
                    case SpellImplicitTargets.TARGET_PET:
                    case SpellImplicitTargets.TARGET_MINION:
                        break;
                    //TODO
                    case SpellImplicitTargets.TARGET_NONCOMBAT_PET:
                        if (Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER) && ((Player)Caster).NonCombatPet != null)
                            targets.Add(((Player)Caster).NonCombatPet);
                        break;
                    case SpellImplicitTargets.TARGET_SINGLE_ENEMY:
                    case SpellImplicitTargets.TARGET_SINGLE_FRIEND_2:
                    case SpellImplicitTargets.TARGET_SELECTED_FRIEND:
                    case SpellImplicitTargets.TARGET_SINGLE_PARTY:
                        if (!targets.Contains(Targets.Target))
                            targets.Add(Targets.Target);
                        break;
                    case SpellImplicitTargets.TARGET_EFFECT_SELECT:
                        break;
                    default:
                        if (Targets.Target != null)
                        {
                            if (!targets.Contains(Targets.Target))
                                targets.Add(Targets.Target);
                        }
                        else
                        {
                            if (!targets.Contains(Caster))
                                targets.Add(Caster);
                        }
                        break;
                }

                if (Spell.EffectImplicitTargetA[index] == (uint)SpellImplicitTargets.TARGET_NOTHING && Spell.EffectImplicitTargetB[index] == (uint)SpellImplicitTargets.TARGET_NOTHING)
                {
                    if (targets.Count == 0)
                    {
                        if (Targets.Target != null)
                        {
                            if (!targets.Contains(Targets.Target))
                                targets.Add(Targets.Target);
                        }
                        else
                        {
                            if (!targets.Contains(Caster))
                                targets.Add(Caster);
                        }
                    }
                }
            }

            return CalculateMisses(Caster, targets, (SpellEffects)Spell.Effect[index]);
        }

        public Dictionary<WorldObject, SpellMissInfo> CalculateMisses(WorldObject Caster, List<WorldObject> Targets, SpellEffects SpellEffect)
        {
            Dictionary<WorldObject, SpellMissInfo> newTargets = new Dictionary<WorldObject, SpellMissInfo>();
            foreach (WorldObject Target in Targets)
            {
                if (Target == Caster && Caster.IsTypeOf(ObjectTypes.TYPE_UNIT) && Target.IsTypeOf(ObjectTypes.TYPE_UNIT))
                {
                    var _with1 = Target;
                    switch (SchoolMask)
                    {
                        case SpellSchoolMask.SPELL_SCHOOL_MASK_NONE:
                            newTargets.Add(Target, SpellMissInfo.MISS_NONE);
                            break;
                        case SpellSchoolMask.SPELL_SCHOOL_MASK_MAGIC:
                            newTargets.Add(Target, SpellMissInfo.MISS_NONE); // _with1.GetMagicSpellHitResult(Caster, this));
                            break;
                        case SpellSchoolMask.SPELL_SCHOOL_MASK_NORMAL:
                            newTargets.Add(Target, SpellMissInfo.MISS_NONE); //_with1.GetMeleeSpellHitResult(Caster, this));
                            break;
                    }
                }
                else
                    newTargets.Add(Target, SpellMissInfo.MISS_NONE);
            }

            return newTargets;
        }

        public int GetValue(Unit target, int index)
        {
            Player p = (Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER) ? (Player)Caster : null);
            uint combopoints = p?.ComboPoints ?? 0;

            byte level = Caster.Level;
            if (level > Spell.maxLevel && Spell.maxLevel > 0)
                level = (byte)Spell.maxLevel;
            else if (level < Spell.baseLevel)
                level = (byte)Spell.baseLevel;
            level -= (byte)Spell.spellLevel;

            int baseDice = (int)Spell.EffectBaseDice[index];
            float basePointsPerLevel = Spell.EffectRealPointsPerLevel[index];
            float randomPointsPerLevel = Spell.EffectDicePerLevel[index];
            int basePoints = (m_basepoints[index] > 0 ? m_basepoints[index] - baseDice : (int)Spell.EffectBasePoints[index]) + (int)(level * basePointsPerLevel);
            int randomPoints = (int)(Spell.EffectDieSides[index] + level * randomPointsPerLevel);

            switch (randomPoints)
            {
                case 0:
                case 1:
                    basePoints += baseDice;
                    break;
                default:
                    int randvalue = baseDice >= randomPoints ? new Random().Next(randomPoints, baseDice) : new Random().Next(baseDice, randomPoints);
                    basePoints += randvalue;
                    break;
            }

            int value = basePoints;

            //if (Player * modOwner = GetSpellModOwner())
            //    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_ALL_EFFECTS, value);

            if (Flag.HasFlag(Spell.Attributes, (uint)SpellAttributes.SPELL_ATTR_LEVEL_DAMAGE_CALCULATION) && Spell.spellLevel > 0 &&
                !new[] { SpellEffects.SPELL_EFFECT_WEAPON_PERC_DMG, SpellEffects.SPELL_EFFECT_APPLY_AURA }.Contains((SpellEffects)Spell.Effect[index]))
                value = (int)(value * 0.25f * Math.Exp(Caster.Level * (70 - Spell.spellLevel) / 1000.0f));

            return value;
        }



        public List<WorldObject> GetEnemyAtPoint(Unit caster, Vector location, float radius)
        {
            List<WorldObject> enemies = new List<WorldObject>();

            if (caster.IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                foreach (WorldObject unit in GridManager.Instance.GetSurroundingObjects(caster))
                {
                    if (!unit.IsTypeOf(ObjectTypes.TYPE_UNIT) || unit.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                        continue;
                    if (((Unit)unit).IsEnemyTo(caster))
                        continue;
                    if (((Unit)unit).IsDead)
                        continue;

                    if (unit.Location.Distance(location) <= radius)
                        enemies.Add(unit);
                }
            }
            else if (caster.IsTypeOf(ObjectTypes.TYPE_UNIT))
            {
                foreach (WorldObject unit in GridManager.Instance.GetSurroundingObjects(caster))
                {
                    if (!unit.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                        continue;
                    if (((Player)unit).IsDead)
                        continue;

                    if (unit.Location.Distance(location) <= radius)
                        enemies.Add(unit);
                }
            }

            return enemies;
        }

        public List<WorldObject> GetEnemyInFrontOfMe(Unit caster, float Distance)
        {
            List<WorldObject> result = new List<WorldObject>();
            List<WorldObject> tmp = GetEnemyAtPoint(caster, caster.Location, Distance);
            foreach (Unit unit in tmp)
                if (caster.IsInFrontOf(unit))
                    result.Add(unit);

            return result;
        }

        public List<WorldObject> GetEnemyInBehindMe(Unit caster, float Distance)
        {
            List<WorldObject> result = new List<WorldObject>();
            List<WorldObject> tmp = GetEnemyAtPoint(caster, caster.Location, Distance);
            foreach (Unit unit in tmp)
                if (caster.IsInBackOf(unit))
                    result.Add(unit);

            return result;
        }


        public void SendSpellStart()
        {
            this.State = SpellState.SPELL_STATE_PREPARING;
            this.SetCastTime();

            if (!Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                return;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_SPELL_START);

            if (Targets.Target.IsTypeOf(ObjectTypes.TYPE_ITEM))
                pkt.WriteUInt64(Targets.Target.Guid);
            else
                pkt.WriteUInt64(Caster.Guid);

            pkt.WriteUInt64(Caster.Guid);
            pkt.WriteUInt32(Spell.Id);
            pkt.WriteUInt16(2);
            pkt.WriteUInt32(0);
            Targets.WriteTargets(ref pkt);
            ((Player)Caster).Client.Send(pkt);
        }

        public void SendChannelUpdate(uint time)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.MSG_CHANNEL_UPDATE);
            pkt.WriteUInt32(time);
            ((Player)Caster).Client.Send(pkt);

            if (time == 0)
            {
                Caster.ChannelObject = 0;
                Caster.ChannelSpell = 0;
                GridManager.Instance.SendSurrounding(Caster.BuildUpdate(), Caster);
            }

        }

        public void SendSpellGo()
        {

            if (!Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                return;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_SPELL_GO);

            if (Targets.Target.IsTypeOf(ObjectTypes.TYPE_ITEM))
                pkt.WriteUInt64(Targets.Target.Guid);
            else
                pkt.WriteUInt64(Caster.Guid);

            pkt.WriteUInt64(Caster.Guid);
            pkt.WriteUInt32(Spell.Id);
            pkt.WriteUInt16(256);
            pkt.WriteUInt8(1); //Hits
            pkt.WriteUInt64(Caster.Guid);
            pkt.WriteUInt8(0); //Misses
            Targets.WriteTargets(ref pkt);
            ((Player)Caster).Client.Send(pkt);
        }

        public void SendInterrupted(byte result)
        {
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_SPELL_FAILURE);
            pkt.WriteUInt64(Caster.Guid);
            pkt.WriteUInt32(Spell.Id);
            pkt.WriteUInt8(result);
            GridManager.Instance.SendSurrounding(pkt, Caster);
        }

        public void SendCooldown()
        {
            if (!Caster.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                return;

            Player p = ((Player)Caster);
            if (!p.Spells.ContainsKey(Spell.Id))
                return;

            uint Recovery = Spell.RecoveryTime;
            uint CatergoryRecovery = Spell.CategoryRecoveryTime;

            if (Spell.Id == 2764) //Throw spell uses the equipped ranged item's attackspeed
                Recovery = p.Inventory.Backpack.GetItem((byte)InventorySlots.SLOT_RANGED)?.BaseAttackTime ?? Recovery;

            if (Spell.Id == 5019) //Shoot spell uses the equipped wand's attackspeed
                Recovery = p.Inventory.Backpack.GetItem((byte)InventorySlots.SLOT_RANGED)?.Template.WeaponSpeed ?? Recovery;

            if (CatergoryRecovery == 0 && Recovery == 0) //No cooldown
                return;

            p.Spells[Spell.Id].Cooldown = Globals.GetFutureTime(Recovery / 1000f);

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_SPELL_COOLDOWN);
            pkt.WriteUInt64(p.Guid);

            if (CatergoryRecovery > 0)
            {
                foreach (var spells in p.Spells)
                {
                    if (spells.Value.Spell.Category == Spell.Category)
                    {
                        pkt.WriteUInt32(spells.Key);
                        if (spells.Key != Spell.Id || Recovery > 0)
                            pkt.WriteUInt32(CatergoryRecovery);
                        else
                            pkt.WriteUInt32(Recovery);
                    }
                }
            }
            else if (Recovery > 0)
            {
                pkt.WriteUInt32(Spell.Id);
                pkt.WriteUInt32(Recovery);
            }

            p.Client.Send(pkt);
        }

        public uint GetManaCost(uint mana)
        {
            return PowerCost + Spell.manaCostPerLevel * Caster.Level + mana * (Spell.manaCostPct / 100);
        }


        private void SetCastTime()
        {
            if (DBC.SpellCastTimes.TryGetValue((int)Spell.CastingTimeIndex, out CastTime))
            {
                this.Duration = CastTime.m_base / 1000;
                this.Timer = Globals.GetFutureTime((float)this.Duration);
            }

        }

        private uint SetPowerCost()
        {
            if (Flag.HasFlag(Spell.Attributes, (uint)SpellAttributesEx.SPELL_ATTR_EX_DRAIN_ALL_POWER))
            {
                // If power type - health drain all
                if (Spell.powerType == (uint)PowerTypes.POWER_HEALTH)
                    return Caster.Health.Current;

                if (Spell.powerType <= (uint)PowerTypes.TYPE_HAPPINESS)
                    return Caster.GetPowerValue(false);

                return 0;
            }

            // Base powerCost
            int powerCost = (int)Spell.manaCost;
            // PCT cost from total amount
            if (Spell.manaPerSecond > 0)
            {
                switch ((PowerTypes)Spell.powerType)
                {
                    // health as power used
                    case PowerTypes.POWER_HEALTH:
                        powerCost += (int)(Spell.manaCostPct * Caster.Health.BaseAmount / 100);
                        break;
                    case PowerTypes.TYPE_MANA:
                        powerCost += (int)(Spell.manaCostPct * Caster.Mana.BaseAmount / 100);
                        break;
                    case PowerTypes.TYPE_RAGE:
                    case PowerTypes.TYPE_FOCUS:
                    case PowerTypes.TYPE_ENERGY:
                        powerCost += (int)(Spell.manaCostPct * Caster.GetPowerValue((PowerTypes)Spell.powerType, true) / 100);
                        break;
                    default:
                        return 0;
                }
            }

            if (Flag.HasFlag(Spell.Attributes, (uint)SpellAttributes.SPELL_ATTR_LEVEL_DAMAGE_CALCULATION))
                powerCost = (int)(powerCost / (1.117f * Spell.spellLevel / Caster.Level - 0.1327f));

            return (uint)(powerCost < 0 ? 0 : powerCost);
        }

        private SpellSchoolMask GetFirstSchoolInMask()
        {
            for (int i = 0; i < 8; ++i)
                if (Flag.HasFlag((byte)this.SchoolMask, (byte)(1 << i)))
                    return (SpellSchoolMask)i;

            return SpellSchoolMask.SPELL_SCHOOL_MASK_NORMAL;
        }
    }

    public class SpellTargets
    {
        public WorldObject Target = null;
        public Vector TargetLocation = null;
        public uint TargetMask = 0;

        public void ReadTargets(ref PacketReader packet, Player p)
        {
            TargetMask = packet.ReadUInt16();

            if (TargetMask == (uint)SpellTargetType.TARGET_TYPE_CASTER)
            {
                Target = p;
                return;
            }

            if (Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_UNIT) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_OBJECT) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_ITEM) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_ENEMY) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_FRIENDLY))
            {
                ulong guid = packet.ReadUInt64();
                Target = Database.Creatures.TryGet<WorldObject>(guid) ??
                         Database.Players.TryGet<WorldObject>(guid) ??
                         Database.GameObjects.TryGet<WorldObject>(guid) ??
                         Database.Items.TryGet<WorldObject>(guid);
            }

            if (Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_LOCATION))
            {
                TargetLocation = packet.ReadVector();
            }
        }

        public void WriteTargets(ref PacketWriter packet)
        {
            packet.WriteUInt16((ushort)TargetMask);
            if (Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_UNIT) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_OBJECT) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_ITEM) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_ENEMY) ||
                Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_FRIENDLY))
            {
                packet.WriteUInt64(Target.Guid);
            }

            if (Flag.HasFlag(TargetMask, (uint)SpellTargetType.TARGET_TYPE_LOCATION))
            {
                packet.WriteVector(TargetLocation);
            }
        }
    }
}
