using Common.Constants;
using Common.Database.DBC;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game;
using WorldServer.Game.Objects.UnitExtensions;
using WorldServer.Game.Structs;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class SpellHandler
    {
        public static void HandleCastSpellOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            uint spellid = packet.ReadUInt32();

            if (!manager.Character.Spells.ContainsKey(spellid))
            {
                manager.Character.SendCastResult(SpellFailedReason.SPELL_FAILED_NOT_KNOWN, spellid);
                return;
            }
                
            if (!DBC.Spell.ContainsKey(spellid))
            {
                manager.Character.SendCastResult(SpellFailedReason.SPELL_FAILED_NOT_KNOWN, spellid);
                return;
            }

            if(manager.Character.Spells[spellid].Cooldown > Globals.Time)
            {
                manager.Character.SendCastResult(SpellFailedReason.SPELL_FAILED_NOT_READY, spellid);
                return;
            }

            SpellTargets targets = new SpellTargets();
            targets.ReadTargets(ref packet, manager.Character);

            SpellCast cast = new SpellCast(manager.Character);
            cast.Targets = targets;
            cast.Spell = DBC.Spell[spellid];
            cast.Triggered = false;
            manager.Character.PrepareSpell(cast);
        }

        public static void HandleCancelCastOpcode(ref PacketReader packet, ref WorldManager manager)
        {
            uint spellid = packet.ReadUInt32();
            foreach(SpellCast spell in manager.Character.SpellCast.Values)
            {
                if(spell.Spell.Id == spellid && 
                    spell.State != SpellState.SPELL_STATE_FINISHED && 
                    spell.SchoolMask != SpellSchoolMask.SPELL_SCHOOL_MASK_NORMAL)
                {
                    spell.Cancel();
                }
            }
        }
    }
}
