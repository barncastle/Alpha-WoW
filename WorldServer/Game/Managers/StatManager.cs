using Common.Constants;
using Common.Helpers.Extensions;
using Common.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldServer.Game.Objects;
using WorldServer.Game.Structs;

namespace WorldServer.Game.Managers
{
    public class StatManager
    {
        private Player character;
        public StatManager(Player character)
        {
            this.character = character;
        }

        public void UpdateAll()
        {
            CalculateItemStats();
            UpdateHealth();
            UpdateMana();
            UpdateDefenseBonusesMod();
            character.Inventory.SetBaseAttackTime();
        }

        private void CalculateItemStats()
        {
            int[] attributes = new int[8];
            character.Damage.SetAll(0);
            character.Armor.ResetAll();
            character.Holy.ResetAll();
            character.Fire.ResetAll();
            character.Nature.ResetAll();
            character.Frost.ResetAll();
            character.Shadow.ResetAll();
            character.BaseAttackTime = (character.Agility.Current * 140); //Default DPS calulation

            List<Item> equipped = character.Inventory.Backpack.Items.Values.Where(x => x.CurrentSlot < (byte)InventorySlots.SLOT_BAG1).ToList();
            character.Holy.PositiveAmount += (uint)equipped.Where(x => x.Template.ResistHoly > 0).Sum(x => x.Template.ResistHoly);
            character.Holy.NegativeAmount += (uint)equipped.Where(x => x.Template.ResistHoly < 0).Sum(x => x.Template.ResistHoly);

            character.Nature.PositiveAmount += (uint)equipped.Where(x => x.Template.ResistNature > 0).Sum(x => x.Template.ResistNature);
            character.Nature.NegativeAmount += (uint)equipped.Where(x => x.Template.ResistNature < 0).Sum(x => x.Template.ResistNature);

            character.Fire.PositiveAmount += (uint)equipped.Where(x => x.Template.ResistFire > 0).Sum(x => x.Template.ResistFire);
            character.Fire.NegativeAmount += (uint)equipped.Where(x => x.Template.ResistFire < 0).Sum(x => x.Template.ResistFire);

            character.Frost.PositiveAmount += (uint)equipped.Where(x => x.Template.ResistFrost > 0).Sum(x => x.Template.ResistFrost);
            character.Frost.NegativeAmount += (uint)equipped.Where(x => x.Template.ResistFrost < 0).Sum(x => x.Template.ResistFrost);

            character.Shadow.PositiveAmount += (uint)equipped.Where(x => x.Template.ResistShadow > 0).Sum(x => x.Template.ResistShadow);
            character.Shadow.NegativeAmount += (uint)equipped.Where(x => x.Template.ResistShadow < 0).Sum(x => x.Template.ResistShadow);

            character.Armor.BaseAmount += (uint)equipped.Sum(x => x.Template.ResistPhysical);
            character.Damage.Maximum += (uint)equipped.Sum(x => x.Template.DamageStats[0].Max);
            character.Damage.Current += (uint)equipped.Sum(x => x.Template.DamageStats[0].Min);

            character.BaseAttackTime += (equipped.FirstOrDefault(x => x.CurrentSlot == (uint)InventorySlots.SLOT_MAINHAND)?.Template.WeaponSpeed ?? 0) / 4;

            //Stat update
            foreach (Item item in equipped)
                foreach (ItemAttribute ia in item.Template.Attributes)
                    attributes[ia.ID] += ia.Value;

            character.Agility.ResetCurrent(true);
            character.Agility.Current = ((uint)attributes[(int)InventoryStats.AGILITY] + character.Agility.BaseAmount);
            character.Strength.ResetCurrent(true);
            character.Strength.Current = ((uint)attributes[(int)InventoryStats.STRENGTH] + character.Strength.BaseAmount);
            character.Stamina.ResetCurrent(true);
            character.Stamina.Current = ((uint)attributes[(int)InventoryStats.STAMINA] + character.Stamina.BaseAmount);
            character.Spirit.ResetCurrent(true);
            character.Spirit.Current = ((uint)attributes[(int)InventoryStats.SPIRIT] + character.Spirit.BaseAmount);
            character.Intellect.ResetCurrent(true);
            character.Intellect.Current = ((uint)attributes[(int)InventoryStats.INTELLECT] + character.Intellect.BaseAmount);
        }

        private void UpdateHealth()
        {
            uint baseStam = (character.Stamina.Current < 20 ? character.Stamina.Current : 20);
            uint moreStam = character.Stamina.Current - baseStam;
            character.Health.Maximum = character.Health.BaseAmount + baseStam + (moreStam * 10);
        }

        private void UpdateMana()
        {
            uint baseMana = (character.Intellect.Current < 20 ? character.Intellect.Current : 20);
            uint moreMana = character.Intellect.Current - baseMana;
            character.Mana.Maximum = character.Mana.BaseAmount + baseMana + (moreMana * 15);
        }

        private void UpdateDefenseBonusesMod()
        {
            character.BlockPercentage = 5f + ((character.Level * 5f) * 0.04f);
            character.ParryPercentage = 5f + ((character.Level * 5f) * 0.04f);
            character.DodgePercentage = 5f + ((character.Level * 5f) * 0.04f);
        }

        public float GetSpiritRegenRate()
        {
            float addValue = 0f;
            float spirit = character.Spirit.Current;

            switch (character.Class)
            {
                case (byte)Classes.CLASS_DRUID:
                case (byte)Classes.CLASS_PALADIN:
                case (byte)Classes.CLASS_WARLOCK:
                    addValue = (character.Spirit.Current / 5 + 15);
                    break;
                case (byte)Classes.CLASS_MAGE:
                case (byte)Classes.CLASS_PRIEST:
                    addValue = (character.Spirit.Current / 4 + 15);
                    break;
                case (byte)Classes.CLASS_SHAMAN:
                    addValue = (character.Spirit.Current / 5 + 17);
                    break;
            }
            return addValue /= 2.0f; //This returns the per second update amount
        }

    }
}
