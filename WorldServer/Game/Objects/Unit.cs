using Common.Constants;
using Common.Helpers.Extensions;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using WorldServer.Game.Structs;
using WorldServer.Game.Managers;
using WorldServer.Storage;
using System.Collections.Concurrent;
using Common.Helpers;
using Newtonsoft.Json;
using WorldServer.Game.Objects.UnitExtensions;
using Common.Database.DBC;

namespace WorldServer.Game.Objects
{
    public class Unit : WorldObject
    {
        public uint ChannelSpell = 0;
        public ulong ChannelObject = 0;

        public bool Dirty = true;
        public volatile bool InCombat = false;
        public bool IsArmed = false;
        public volatile bool IsDead = false;
        public byte StandState = 0;
        public uint Money = 0;

        public float BlockPercentage = 0f;
        public float ParryPercentage = 0f;
        public float DodgePercentage = 0f;
        public byte Level = 1;
        public readonly byte MaxLevel = Globals.MAX_LEVEL;

        public TStat Health = new TStat();
        public TStat Mana = new TStat();
        public TStat Rage = new TStat(10); //Rage has x10 multiplier
        public TStat Focus = new TStat();
        public TStat Energy = new TStat();
        public TStat Strength = new TStat();
        public TStat Agility = new TStat();
        public TStat Stamina = new TStat();
        public TStat Intellect = new TStat();
        public TStat Spirit = new TStat();
        public TResistance Armor = new TResistance();
        public TResistance Holy = new TResistance();
        public TResistance Fire = new TResistance();
        public TResistance Nature = new TResistance();
        public TResistance Frost = new TResistance();
        public TResistance Shadow = new TResistance();
        public TStat Damage = new TStat();
        public byte PowerType = 0;
        public byte Class;
        public uint Faction;

        [JsonIgnore]
        public ConcurrentDictionary<ulong, Unit> Attackers = new ConcurrentDictionary<ulong, Unit>();
        [JsonIgnore]
        public Dictionary<CurrentSpellType, SpellCast> SpellCast = new Dictionary<CurrentSpellType, SpellCast>();
        [JsonIgnore]
        protected Dictionary<AttackTypes, long> AttackTimers = new Dictionary<AttackTypes, long>();

        public uint ExtraAttacks = 0;
        public bool IsAttacking = false;
        public ulong CombatTarget; //Target set 
        public bool IsDisarmedMainHand = false;
        public bool IsDisarmedOffHand = false;
        public bool IsRooted = false;
        public bool IsSilenced = false;
        public bool IsSitting
        {
            get
            {
                StandState s = (StandState)this.StandState;
                return
                    s == Common.Constants.StandState.UNIT_SITTINGCHAIR || s == Common.Constants.StandState.UNIT_SITTINGCHAIRLOW ||
                    s == Common.Constants.StandState.UNIT_SITTINGCHAIRMEDIUM || s == Common.Constants.StandState.UNIT_SITTINGCHAIRHIGH ||
                    s == Common.Constants.StandState.UNIT_SITTING;
            }
        }
        public bool IsStanding
        {
            get
            {
                StandState s = (StandState)this.StandState;
                return !IsSitting && s != Common.Constants.StandState.UNIT_SLEEPING && s != Common.Constants.StandState.UNIT_KNEEL;
            }
        }

        public Unit()
        {
            this.ObjectType |= ObjectTypes.TYPE_UNIT;
            AttackTimers.Add(AttackTypes.BASE_ATTACK, 0);
            AttackTimers.Add(AttackTypes.OFFHAND_ATTACK, 0);
        }

        public void SetRoot(bool enable)
        {
            PacketWriter pw = new PacketWriter((enable ? Opcodes.SMSG_FORCE_MOVE_ROOT : Opcodes.SMSG_FORCE_MOVE_UNROOT));
            pw.WriteUInt64(this.Guid);
            pw.WriteUInt32(0);
            GridManager.Instance.SendSurrounding(pw, this);
            this.IsRooted = enable;
        }

        public void PlayEmote(uint emote)
        {
            if (emote == 0)
                return;

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_EMOTE);
            pkt.WriteUInt32(emote);
            pkt.WriteUInt64(this.Guid);
            GridManager.Instance.SendSurroundingInRange(pkt, this, Globals.EMOTE_RANGE);
        }

        public void SetStandState(byte state)
        {
            this.StandState = state;

            if (this.IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                PacketWriter pkt = new PacketWriter(Opcodes.CMSG_STANDSTATECHANGE);
                pkt.WriteUInt8(state);
                ((Player)this).Client.Send(pkt);

                GridManager.Instance.SendSurrounding(this.BuildUpdate(), this);
            }

            this.Dirty = true;
        }

        #region Movement Functions
        public float GetOrientation(float x1, float x2, float y1, float y2)
        {
            float angle = (float)Math.Atan2(y2 - y1, x2 - x1);

            if (angle < 0)
                angle = (float)(angle + 2 * Math.PI);

            return angle;
        }

        public bool IsInFrontOf(WorldObject obj)
        {
            float angle2 = GetOrientation(this.Location.X, obj.Location.X, this.Location.Y, obj.Location.Y);
            float lowAngle = this.Orientation - 1.047198f;
            float hiAngle = this.Orientation + 1.047198f;

            if (lowAngle < 0)
                return ((angle2 >= 2 * Math.PI + lowAngle & angle2 <= 2 * Math.PI) | (angle2 >= 0 & angle2 <= hiAngle));

            return (angle2 >= lowAngle) & (angle2 <= hiAngle);
        }

        public bool IsInBackOf(WorldObject obj)
        {
            float angle2 = GetOrientation(obj.Location.X, this.Location.X, obj.Location.Y, this.Location.Y);
            float lowAngle = this.Orientation - 1.047198f;
            float hiAngle = this.Orientation + 1.047198f;

            if (lowAngle < 0)
                return ((angle2 >= 2 * Math.PI + lowAngle & angle2 <= 2 * Math.PI) | (angle2 >= 0 & angle2 <= hiAngle));

            return (angle2 >= lowAngle) & (angle2 <= hiAngle);
        }
        #endregion


        #region Combat Functions

        public void DealSpellDamage(SpellCast spell, int damage, SpellDamageType type, int index)
        {
            bool isHeal = false;
            bool isDot = false;

            Unit caster = spell.Caster;
            switch (type)
            {
                case SpellDamageType.SPELL_TYPE_HEAL:
                    isHeal = true;
                    break;
                case SpellDamageType.SPELL_TYPE_HEALDOT:
                    isHeal = true;
                    isDot = true;
                    break;
                case SpellDamageType.SPELL_TYPE_DOT:
                    isDot = true;
                    break;
            }

            int benefit = 0;
            if (caster.IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                int penalty = 0;
                int effectcount = 0;
                for (uint i = 0; i < 3; i++)
                    if (spell.Spell.Effect[i] != 0)
                        effectcount++;

                if (effectcount > 1)
                    penalty = 5;

                int spelldamage = 0;
                //TODO
                //        If IsHeal Then
                //            SpellDamage = CType(Caster, CharacterObject).healing.Value
                //        Else
                //            SpellDamage = CType(Caster, CharacterObject).spellDamage(DamageType).Value
                //        End If

                if (isDot)
                {
                    int tickamt = (int)(spell.Duration / spell.Spell.EffectAmplitude[index]);
                    if (tickamt < 5)
                        tickamt = 5;

                    benefit = spelldamage / tickamt;
                }
                else
                {
                    int casttime = spell.CastTime?.m_base ?? 0;
                    if (casttime < 1500)
                        casttime = 1500;
                    else if (casttime > 3500)
                        casttime = 3500;

                    benefit = (int)Math.Truncate(spelldamage * (casttime / 1000f) * ((100 - penalty) / 100) / 3.5f);
                }

            }

            damage += benefit;



        }

        public uint GetPowerValue(bool maximum)
        {
            return GetPowerValue((PowerTypes)this.PowerType, maximum);
        }

        public uint GetPowerValue(PowerTypes type, bool maximum)
        {
            switch (type)
            {
                case PowerTypes.TYPE_ENERGY:
                    return (maximum ? Energy.Maximum : Energy.Current);
                case PowerTypes.TYPE_FOCUS:
                    return (maximum ? Focus.Maximum : Focus.Current);
                case PowerTypes.TYPE_MANA:
                    return (maximum ? Mana.Maximum : Mana.Current);
                case PowerTypes.TYPE_RAGE:
                    return (maximum ? Rage.Maximum : Rage.Current);
            }

            return 0;
        }

        public void SetPowerValue(PowerTypes type, uint value, bool maximum)
        {
            switch (type)
            {
                case PowerTypes.TYPE_ENERGY:
                    if (maximum)
                        Energy.Maximum = value;
                    else
                        Energy.Current = value;
                    break;
                case PowerTypes.TYPE_FOCUS:
                    if (maximum)
                        Focus.Maximum = value;
                    else
                        Focus.Current = value;
                    break;
                case PowerTypes.TYPE_MANA:
                    if (maximum)
                        Mana.Maximum = value;
                    else
                        Mana.Current = value;
                    break;
                case PowerTypes.TYPE_RAGE:
                    if (maximum)
                        Rage.Maximum = value;
                    else
                        Rage.Current = value;
                    break;
            }
        }

        public void SetPowerValue(uint value, bool maximum)
        {
            SetPowerValue((PowerTypes)this.PowerType, value, maximum);
        }

        public bool Attack(Unit victim, bool melee)
        {
            if (victim == this)
                return false;
            if (victim.IsDead)
                return false;

            //Fighting already
            if (IsAttacking)
            {
                //Fighting same target already
                if (CombatTarget == victim.Guid)
                    return true;
                else
                    return false;
            }
            else
            {
                CombatTarget = victim.Guid;
                IsAttacking = true;
                InCombat = true;

                if (victim.IsTypeOf(ObjectTypes.TYPE_UNIT) && !victim.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                {
                    Creature c = ((Creature)victim);
                    c.SetCombatStartLocation();
                }
            }

            victim.Attackers.TryAdd(this.Guid, this);
            return true;
        }

        public bool UpdateMeleeAttackingState()
        {
            byte swingerror = 0;
            float combatAngle = (float)Math.PI;
            Player player = (this.IsTypeOf(ObjectTypes.TYPE_PLAYER) ? ((Player)this) : null);
            Unit victim = (Database.Creatures.TryGet<Unit>(CombatTarget) ?? Database.Players.TryGet<Unit>(CombatTarget));

            if (victim == null)
            {
                if (player != null)
                    player.LeaveCombat();
                return false;
            }

            if (victim.IsDead)
            {
                if (this.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                    ((Player)this).LeaveCombat();
                else
                {
                    Unit dump;
                    this.Attackers.TryRemove(victim.Guid, out dump);
                    this.IsAttacking = false;
                }

                return false;
            }

            if (!IsAttackReady(AttackTypes.BASE_ATTACK) && !IsAttackReady(AttackTypes.OFFHAND_ATTACK)) //Implement two hand timer
                return false;

            //Out of reach
            if (this.Location.Distance(victim.Location) > (player != null ? 3f : ((Creature)this).Template.CombatReach * 3f))
            {
                SetAttackTimer(AttackTypes.BASE_ATTACK, (int)this.BaseAttackTime);
                if (player != null && player.Inventory.HasOffhandWeapon())
                    SetAttackTimer(AttackTypes.OFFHAND_ATTACK, (int)this.BaseAttackTime);
                swingerror = 1;
            }
            else if (this.Location.Angle(victim.Location) > combatAngle || this.Location.Angle(victim.Location) < -combatAngle) //Facing target?
            {
                SetAttackTimer(AttackTypes.BASE_ATTACK, (int)this.BaseAttackTime);
                if (player != null && player.Inventory.HasOffhandWeapon())
                    SetAttackTimer(AttackTypes.OFFHAND_ATTACK, (int)this.BaseAttackTime);
                swingerror = 2;
            }
            else
            {
                //Main Hand attack
                if (IsAttackReady(AttackTypes.BASE_ATTACK))
                {
                    if (player != null && player.Inventory.HasOffhandWeapon())
                        if (AttackTimers[AttackTypes.OFFHAND_ATTACK] < 200)
                            SetAttackTimer(AttackTypes.OFFHAND_ATTACK, 200); //Prevent both hand attacks at the same time

                    AttackerStateUpdate(victim, AttackTypes.BASE_ATTACK, false);

                    SetAttackTimer(AttackTypes.BASE_ATTACK, (int)this.BaseAttackTime);
                }

                //Off Hand attack
                if (player != null && player.Inventory.HasOffhandWeapon() && IsAttackReady(AttackTypes.OFFHAND_ATTACK))
                {
                    if (AttackTimers[AttackTypes.BASE_ATTACK] < 200)
                        SetAttackTimer(AttackTypes.BASE_ATTACK, 200); //Prevent both hand attacks at the same time

                    AttackerStateUpdate(victim, AttackTypes.OFFHAND_ATTACK, false);

                    SetAttackTimer(AttackTypes.OFFHAND_ATTACK, (int)this.BaseAttackTime);
                }
            }

            if (player != null && swingerror != player.SwingError)
            {
                switch (swingerror)
                {
                    case 1:
                        player.SendAttackSwingNotInRange();
                        break;
                    case 2:
                        player.SendAttackSwingFacingWrongWay();
                        break;
                    default:
                        break;
                }
                player.SwingError = swingerror;
            }

            return (swingerror == 0);
        }

        public void AttackerStateUpdate(Unit victim, AttackTypes at, bool extra)
        {
            if (victim.IsDead)
            {
                if (this.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                    ((Player)this).LeaveCombat();
                return;
            }


            if (at == AttackTypes.BASE_ATTACK)
            {
                //TODO Cast current melee spell

                //No recent extra attack only at any non extra attack
                if (!extra && ExtraAttacks > 0)
                {
                    while (ExtraAttacks > 0)
                    {
                        AttackerStateUpdate(victim, AttackTypes.BASE_ATTACK, true);
                        if (ExtraAttacks > 0)
                            --ExtraAttacks;
                    }
                    return;
                }
            }

            UnitStructs.CalcDamageInfo damageInfo;
            CalculateMeleeDamage(victim, 0, out damageInfo, at);
            SendAttackStateUpdate(damageInfo);

            //Extra attack only at any non extra attack
            if (!extra && ExtraAttacks > 0)
            {
                while (ExtraAttacks > 0)
                {
                    AttackerStateUpdate(victim, AttackTypes.BASE_ATTACK, true);
                    if (ExtraAttacks > 0)
                        --ExtraAttacks;
                }
            }
        }

        public void CalculateMeleeDamage(Unit victim, uint damage, out UnitStructs.CalcDamageInfo damageInfo, AttackTypes attackType)
        {
            damageInfo = new UnitStructs.CalcDamageInfo();

            damageInfo.attacker = this.Guid;
            damageInfo.target = victim.Guid;
            damageInfo.damageSchoolMask = 0;
            damageInfo.attackType = attackType;
            damageInfo.damage = 0;
            damageInfo.cleanDamage = 0;
            damageInfo.absorb = 0;
            damageInfo.resist = 0;
            damageInfo.blocked_amount = 0;

            damageInfo.TargetState = 0;
            damageInfo.HitInfo = 0;
            damageInfo.procAttacker = ProcFlags.NONE;
            damageInfo.procVictim = ProcFlags.NONE;
            damageInfo.procEx = ProcFlagsExLegacy.NONE;

            if (victim == null)
            {
                if (this.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                    ((Player)this).LeaveCombat();
                return;
            }


            if (this.IsDead || victim.IsDead)
            {
                if (this.IsTypeOf(ObjectTypes.TYPE_PLAYER))
                    ((Player)this).LeaveCombat();
                return;
            }

            // Select HitInfo/procAttacker/procVictim flag based on attack type
            switch (attackType)
            {
                case AttackTypes.BASE_ATTACK:
                    damageInfo.procAttacker = ProcFlags.DONE_MELEE_AUTO_ATTACK | ProcFlags.DONE_MAINHAND_ATTACK;
                    damageInfo.procVictim = ProcFlags.TAKEN_MELEE_AUTO_ATTACK;
                    break;
                case AttackTypes.OFFHAND_ATTACK:
                    damageInfo.procAttacker = ProcFlags.DONE_MELEE_AUTO_ATTACK | ProcFlags.DONE_OFFHAND_ATTACK;
                    damageInfo.procVictim = ProcFlags.TAKEN_MELEE_AUTO_ATTACK;
                    damageInfo.HitInfo = HitInfo.OFFHAND;
                    break;
                default:
                    return;
            }

            // Physical Immune check

            damageInfo.damage += CalculateDamage(damageInfo.attackType, false, true);
            // Add melee damage bonus
            //damage = MeleeDamageBonusDone(damageInfo.target, damage, damageInfo.attackType);
            //damage = damageInfo.target.MeleeDamageBonusTaken(this, damage, damageInfo.attackType);

            // Calculate armor reduction
            //if (IsDamageReducedByArmor((SpellSchoolMask)(damageInfo.damageSchoolMask)))
            //{
            //damageInfo.damage = CalcArmorReducedDamage(damageInfo.target, damage, NULL, damageInfo.attackType);
            //damageInfo.cleanDamage += damage - damageInfo.damage;
            //}

            float critChance = 5 + ((float)this.Agility.Current / (this.Level < 3 || this.Agility.Current < 3 ? 1 : this.Level / 3)); //Just a quick dummy formula for crit percentage; 5% base + 1% for each (1/3 level X agility)
            bool isCrit = (new Random().Next(0, 100) <= critChance);

            damageInfo.TargetState = RollMeleeOutcome(victim);

            switch (damageInfo.TargetState)
            {
                case VictimStates.VS_EVADE:
                    damageInfo.HitInfo |= HitInfo.MISS | HitInfo.SWINGNOHITSOUND;
                    damageInfo.procEx |= ProcFlagsExLegacy.EVADE;
                    damageInfo.damage = 0;
                    damageInfo.cleanDamage = 0;
                    return;
                case VictimStates.VS_NONE:
                    damageInfo.HitInfo |= HitInfo.MISS;
                    damageInfo.procEx |= ProcFlagsExLegacy.MISS;
                    damageInfo.damage = 0;
                    damageInfo.cleanDamage = 0;
                    break;
                case VictimStates.VS_WOUND:
                    if (isCrit)
                    {
                        damageInfo.HitInfo |= HitInfo.CRITICALHIT;

                        damageInfo.procEx |= ProcFlagsExLegacy.CRITICAL_HIT;
                        // Crit bonus calc
                        damageInfo.damage += damageInfo.damage;
                        //float mod = 0.0f;

                        // Apply SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_DAMAGE or SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_DAMAGE
                        //if (damageInfo.attackType == RANGED_ATTACK)
                        //mod += damageInfo.target.GetTotalAuraModifier(SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_DAMAGE);
                        //else
                        //mod += damageInfo.target.GetTotalAuraModifier(SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_DAMAGE);

                        // Increase crit damage from SPELL_AURA_MOD_CRIT_DAMAGE_BONUS
                        //mod += (GetTotalAuraMultiplierByMiscMask(SPELL_AURA_MOD_CRIT_DAMAGE_BONUS, damageInfo.damageSchoolMask) - 1.0f) * 100;

                        //if (mod != 0)
                        //AddPct(damageInfo.damage, mod);
                    }
                    else
                        damageInfo.procEx |= ProcFlagsExLegacy.NORMAL_HIT;
                    break;
                case VictimStates.VS_PARRY:
                    damageInfo.procEx |= ProcFlagsExLegacy.PARRY;
                    damageInfo.cleanDamage += damageInfo.damage;
                    damageInfo.damage = 0;
                    break;
                case VictimStates.VS_DODGE:
                    damageInfo.procEx |= ProcFlagsExLegacy.DODGE;
                    damageInfo.cleanDamage += damageInfo.damage;
                    damageInfo.damage = 0;
                    break;
                case VictimStates.VS_BLOCK:
                    //damageInfo.TargetState = VICTIMSTATE_HIT;
                    damageInfo.procEx |= ProcFlagsExLegacy.BLOCK | ProcFlagsExLegacy.NORMAL_HIT;
                    //30% damage blocked, double blocked amount if block is critical
                    //damageInfo.blocked_amount = CalculatePct(damageInfo.damage, damageInfo.target.isBlockCritical() ? damageInfo.target.GetBlockPercent() * 2 : damageInfo.target.GetBlockPercent());
                    damageInfo.damage -= damageInfo.blocked_amount;
                    damageInfo.cleanDamage += damageInfo.blocked_amount;
                    break;
                default:
                    break;
            }

            //Always apply HITINFO_AFFECTS_VICTIM in case its not a miss
            if (!Convert.ToBoolean(damageInfo.HitInfo & HitInfo.MISS))
                damageInfo.HitInfo |= HitInfo.AFFECTS_VICTIM;

            float resilienceReduction = (this.Armor.BaseAmount + 400 + 85 * (int)this.Level);
            if (resilienceReduction > 0.75f)
                resilienceReduction = 0.75f;
            else if (resilienceReduction < 0)
                resilienceReduction = 0;

            damageInfo.resist = (uint)resilienceReduction;

            //Calculate absorb resist
            if ((int)damageInfo.damage > 0)
            {
                damageInfo.procVictim |= ProcFlags.TAKEN_DAMAGE;
                // Calculate absorb & resists
                //CalcAbsorbResist(damageInfo.target, SpellSchoolMask(damageInfo.damageSchoolMask), DIRECT_DAMAGE, damageInfo.damage, &damageInfo.absorb, &damageInfo.resist);

                if (damageInfo.absorb != 0)
                {
                    damageInfo.HitInfo |= (damageInfo.damage - damageInfo.absorb == 0 ? HitInfo.FULL_ABSORB : HitInfo.PARTIAL_ABSORB);
                    damageInfo.procEx |= ProcFlagsExLegacy.ABSORB;
                }

                if (damageInfo.resist != 0)
                    damageInfo.HitInfo |= (damageInfo.damage - damageInfo.resist == 0 ? HitInfo.FULL_RESIST : HitInfo.PARTIAL_RESIST);

                damageInfo.damage -= damageInfo.absorb + damageInfo.resist;
            }
            else //Just incase
                damageInfo.damage = 0;

            if (damageInfo.damage < 0)
                damageInfo.damage = 0;

            if (damageInfo.cleanDamage < 0)
                damageInfo.cleanDamage = 0;
        }

        public uint CalculateDamage(AttackTypes attType, bool normalized, bool addTotalPct)
        {
            float min_damage = 0, max_damage = 0;

            if (IsTypeOf(ObjectTypes.TYPE_PLAYER))
            {
                ((Player)this).CalculateMinMaxDamage(attType, normalized, addTotalPct, out min_damage, out max_damage);
            }
            else if (IsTypeOf(ObjectTypes.TYPE_UNIT))
            {
                Creature c = (Creature)this;
                min_damage = c.Template.Damage.BaseAmount;
                max_damage = c.Template.Damage.Maximum;
            }

            if (min_damage > max_damage)
            {
                float min = min_damage;
                min_damage = max_damage;
                max_damage = min;
            }

            if (max_damage == 0.0f)
                max_damage = 5.0f;

            return (uint)new Random().Next((int)min_damage, (int)max_damage);
        }

        public VictimStates RollMeleeOutcome(Unit victim)
        {
            float parry_chance = 5f + ((this.Level * 5f) * 0.04f);
            float miss_chance = 5f + ((this.Level * 5f) * 0.04f);

            int sum = 0;
            int roll = new Random().Next(0, 10000);
            int tmp = (int)miss_chance;

            if (roll < (sum += tmp))
                return VictimStates.VS_NONE;

            bool isBehind = (victim.Orientation >= (this.Orientation - (Math.PI / 2)) && victim.Orientation <= this.Orientation + (Math.PI * 2));
            if (!isBehind)
            {
                if (tmp > 0 && roll < (sum + tmp))
                    return VictimStates.VS_DODGE;

                if (roll < (sum += (int)parry_chance))
                    return VictimStates.VS_PARRY;

                if (this.Level < victim.Level)
                {
                    tmp = 10 + 2 * 100;
                    tmp = (tmp > 4000 ? 4000 : tmp);
                    if (roll < (sum + tmp))
                        return VictimStates.VS_DEFLECT;
                }

                if ((float)(tmp > 100 ? tmp / 100 : tmp) > new Random().Next(0, 99))
                    return VictimStates.VS_BLOCK;
                else
                    return VictimStates.VS_WOUND;
            }


            return VictimStates.VS_WOUND;
        }

        public void SendAttackStateUpdate(UnitStructs.CalcDamageInfo damageInfo)
        {
            PacketWriter data = new PacketWriter(Opcodes.SMSG_ATTACKERSTATEUPDATE);
            data.WriteUInt32(0xE2); //TODO : Figure out what each of these types are
            data.WriteUInt64(this.Guid);
            data.WriteUInt64(damageInfo.target);
            data.WriteUInt32(damageInfo.damage);
            data.WriteUInt8(1);

            data.WriteUInt32(damageInfo.damageSchoolMask);
            data.WriteFloat(damageInfo.damage);
            data.WriteUInt32(damageInfo.damage);
            data.WriteUInt32(damageInfo.absorb);
            data.WriteUInt32((uint)damageInfo.TargetState);
            data.WriteUInt32(damageInfo.resist); //?
            data.WriteUInt32(0); //?
            data.WriteUInt32(0); //?
            data.WriteUInt32(damageInfo.blocked_amount);
            GridManager.Instance.SendSurrounding(data, this);

            //Damage Effects
            if (damageInfo.damage > 0)
            {
                if (!this.UnitFlags.HasFlag((uint)Common.Constants.UnitFlags.UNIT_FLAG_IN_COMBAT))
                    Flag.SetFlag(ref UnitFlags, (uint)Common.Constants.UnitFlags.UNIT_FLAG_IN_COMBAT);

                Unit target = Database.Creatures.TryGet<Unit>(damageInfo.target) ?? Database.Players.TryGet<Unit>(damageInfo.target);
                if (target == null)
                    return;

                if (!target.UnitFlags.HasFlag((uint)Common.Constants.UnitFlags.UNIT_FLAG_IN_COMBAT))
                    Flag.SetFlag(ref target.UnitFlags, (uint)Common.Constants.UnitFlags.UNIT_FLAG_IN_COMBAT);

                int health = (int)target.Health.Current - (int)damageInfo.damage;
                if (health < 0) health = 0; //Prevent uint overflow

                target.Health.Current = (uint)health; //Update health

                //Update units
                if (IsTypeOf(ObjectTypes.TYPE_PLAYER))
                {
                    Player p = ((Player)this);
                    Creature c = ((Creature)target);

                    if (!c.IsAttacking) //Set attacking state
                    {
                        c.IsAttacking = true;
                        c.InCombat = true;
                        c.CombatTarget = this.Guid;
                    }

                    if (c.AttackTimers[AttackTypes.BASE_ATTACK] != (int)c.BaseAttackTime)
                    {
                        if (c.AttackTimers[AttackTypes.BASE_ATTACK] + 200 > c.BaseAttackTime)
                            c.SetAttackTimer(AttackTypes.BASE_ATTACK, c.BaseAttackTime);
                        else
                            c.SetAttackTimer(AttackTypes.OFFHAND_ATTACK, c.AttackTimers[AttackTypes.BASE_ATTACK] + 200);
                    }

                    if (c.Health.Current <= 0)
                    {
                        Unit dump;
                        c.Die(p);

                        if (p.Attackers.ContainsKey(c.Guid))
                            p.Attackers.TryRemove(c.Guid, out dump);

                        p.LeaveCombat();
                    }
                }
                else
                {
                    Player p = (Player)target;
                    Creature c = (Creature)this;

                    if (p.Health.Current <= 0)
                    {
                        p.SetRoot(true);
                        p.IsDead = true;
                        p.UnitFlags = (uint)Common.Constants.UnitFlags.UNIT_FLAG_DEAD;
                        p.DynamicFlags = (uint)UnitDynamicTypes.UNIT_DYNAMIC_DEAD;
                        p.StandState = (byte)Common.Constants.StandState.UNIT_DEAD;

                        Flag.RemoveFlag(ref p.UnitFlags, (uint)Common.Constants.UnitFlags.UNIT_FLAG_IN_COMBAT); //Remove combat flags
                        Flag.RemoveFlag(ref c.UnitFlags, (uint)Common.Constants.UnitFlags.UNIT_FLAG_IN_COMBAT);

                        p.LeaveCombat();
                    }
                }

                GridManager.Instance.SendSurrounding(target.BuildUpdate(), target);
            }
        }

        public bool IsAttackReady(AttackTypes at)
        {
            return AttackTimers[at] <= 0;
        }

        public void UpdateAttackTimer(AttackTypes at)
        {
            AttackTimers[at] -= 50; //timer update time
        }

        public void SetAttackTimer(AttackTypes at, long value)
        {
            AttackTimers[at] = value;
        }

        public void UpdateSpell()
        {
            foreach (SpellCast spell in this.SpellCast.Values)
            {
                if (spell.IsFinished || spell.State != SpellState.SPELL_STATE_CASTING)
                    continue;

                if (spell.Timer <= Globals.Time)
                    this.Cast(spell);
                else
                    spell.Update();
            }
        }

        #endregion
    }
}
