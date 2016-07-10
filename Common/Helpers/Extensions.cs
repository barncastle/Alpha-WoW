using Common.Singleton;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Helpers.Extensions
{
    public class TStat
    {
        private uint m_current = 0;
        private uint m_maximum = 0;
        private uint m_base = 0;
        private uint m_multiplier = 1; //Rage has a multiplier of 10

        public TStat(uint multiplier = 1)
        {
            m_multiplier = multiplier;
        }

        public TStat(uint baseamt, uint maxamt)
        {
            this.m_multiplier = 1;
            this.BaseAmount = baseamt;
            this.Maximum = maxamt;
        }

        /// <summary>
        /// This is the current value and adds the new value you set to it
        /// The rage modifier is applied when returned
        /// </summary>
        public uint Current
        {
            get { return (m_current * m_multiplier); }
            set { m_current = value; }
        }

        /// <summary>
        /// This only gets affected by character base stats not items or buffs
        /// </summary>
        public uint BaseAmount
        {
            get { return (m_base * m_multiplier); }
            set { m_base = value; }
        }

        /// <summary>
        /// This is the Maximum or Base value of the stat
        /// The rage modifier is applied when returned
        /// </summary>
        public uint Maximum
        {
            get { return (m_maximum * m_multiplier); }
            set { m_maximum = value; }
        }

        /// <summary>
        /// Sets both fields to the same value
        /// </summary>
        /// <param name="value"></param>
        public void SetAll(UInt32 value)
        {
            m_maximum = value;
            m_current = value;
            m_base = value;
        }

        /// <summary>
        /// Resets the Current value to the Max/Base value
        /// </summary>
        public void ResetCurrent(bool useBase = false)
        {
            if (useBase)
                m_current = m_base;
            else
                m_current = m_maximum;
        }

    }

    public class TResistance
    {
        private uint negative;
        private uint positive;
        private uint baseval;

        public TResistance() { }

        public TResistance(uint baseamt)
        {
            this.BaseAmount = baseamt;
        }

        public TResistance(uint baseamt, uint posamt)
        {
            this.BaseAmount = baseamt;
            this.PositiveAmount = posamt;
        }

        public uint NegativeAmount
        {
            get { return negative; }
            set { negative = value; }
        }

        public uint PositiveAmount
        {
            get { return positive; }
            set { positive = value; }
        }

        public uint BaseAmount
        {
            get { return baseval; }
            set { baseval = value; }
        }

        public void SetAll(UInt32 val)
        {
            baseval = val;
            positive = val;
            negative = val;
        }

        /// <summary>
        /// Resets everything
        /// </summary>
        public void ResetAll()
        {
            baseval = 0;
            positive = 0;
            negative = 0;
        }

        /// <summary>
        /// Resets Positive and Negative to Base
        /// </summary>
        public void ResetModifiers()
        {
            positive = baseval;
            negative = baseval;
        }
    }

    public class TRandom
    {
        private uint min;
        private uint max;

        public TRandom() { }

        public TRandom(uint min, uint max)
        {
            this.Minimum = min;
            this.Maximum = max;
        }

        public uint Minimum
        {
            get { return min; }
            set { min = value; }
        }

        public uint Maximum
        {
            get { return max; }
            set { max = value; }
        }

        public byte GetRandom()
        {
            return (byte)new Random().Next((int)min, (int)max);
        }

        public byte GetRandom(int minincrease = 0, int maxincrease = 0)
        {
            return (byte)new Random().Next((int)min + minincrease, (int)max + maxincrease);
        }
    }

    public class TReadOnly<T>
    {
        public readonly T Value;

        public TReadOnly(T value)
        {
            this.Value = value;
        }
    }

    public class ItemAttribute
    {
        public uint ID = 0;
        public int Value = 0;

        public ItemAttribute() { }

        public ItemAttribute(ref MySqlDataReader dr, int i)
        {
            i++;
            this.ID = Convert.ToUInt32(dr["stat_type" + i]);
            this.Value = Convert.ToInt32(dr["stat_value" + i]);
        }
    }

    public class DamageStat
    {
        public int Min = 0;
        public int Max = 0;
        public int Type = 0;

        public DamageStat() { }

        public DamageStat(ref MySqlDataReader dr, int i)
        {
            i++;
            this.Min = Convert.ToInt32(dr["dmg_min" + i]);
            this.Max = Convert.ToInt32(dr["dmg_max" + i]);
            this.Type = Convert.ToInt32(dr["dmg_type" + i]);
        }
    }

    public class SpellStat
    {
        public int ID = 0;
        public int Trigger = 0;
        public int Charges = 0;
        public int Cooldown = 0;
        public int Category = 0;
        public int CategoryCoolDown = 0;

        public SpellStat() { }

        public SpellStat(ref MySqlDataReader dr, int i)
        {
            i++;
            this.ID = Convert.ToInt32(dr["spellid_" + i]);
            this.Trigger = Convert.ToInt32(dr["spelltrigger_" + i]);
            this.Charges = Convert.ToInt32(dr["spellcharges_" + i]);
            this.Cooldown = Convert.ToInt32(dr["spellcooldown_" + i]);
            this.Category = Convert.ToInt32(dr["spellcategory_" + i]);
            this.CategoryCoolDown = Convert.ToInt32(dr["spellcategorycooldown_" + i]);
        }
    }


}
