using Common.Constants;
using Common.Database.DBC;
using Common.Database.DBC.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Game.Objects.UnitExtensions
{
    public static class FactionExtension
    {
        public static bool IsFriendlyTo(this Unit u, Unit target)
        {
            if (!DBC.FactionTemplate.ContainsKey(u.Faction) || !DBC.FactionTemplate.ContainsKey(target.Faction))
                return false;

            FactionTemplate thisFaction = DBC.FactionTemplate[u.Faction];
            FactionTemplate targetFaction = DBC.FactionTemplate[target.Faction];
            return thisFaction.IsFriendlyTo(targetFaction);
        }

        public static bool IsEnemyTo(this Unit u, Unit target)
        {
            if (!DBC.FactionTemplate.ContainsKey(u.Faction) || !DBC.FactionTemplate.ContainsKey(target.Faction))
                return false;

            FactionTemplate thisFaction = DBC.FactionTemplate[u.Faction];
            FactionTemplate targetFaction = DBC.FactionTemplate[target.Faction];
            return thisFaction.IsEnemyTo(targetFaction);
        }

        public static bool IsNeutralTo(this Unit u)
        {
            if (!DBC.FactionTemplate.ContainsKey(u.Faction))
                return false;

            FactionTemplate thisFaction = DBC.FactionTemplate[u.Faction];
            return thisFaction.NeutralToAll();
        }
    }
}
