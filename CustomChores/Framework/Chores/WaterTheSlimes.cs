using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheSlimes : BaseCustomChore
    {
        public override string ChoreName { get; } = "WaterTheSlimes";
        public WaterTheSlimes(CustomChores instance, IDictionary<string, string> config)
            : base(instance, config) { }

        public override bool CanDoIt(string name = null)
        {
            return Game1.getFarm().buildings
                .Where(building => building.daysOfConstructionLeft.Value <= 0)
                .Any(building => building.indoors.Value is SlimeHutch);
        }

        public override bool DoIt(string name = null)
        {
            var success = false;

            foreach (var building in Game1.getFarm().buildings.Where(building => building.daysOfConstructionLeft.Value <= 0))
            {
                if (!(building.indoors.Value is SlimeHutch slimeHutch))
                    continue;
                for (var index = 0; index < slimeHutch.waterSpots.Count; ++index)
                    slimeHutch.waterSpots[index] = true;
                success = true;
            }

            return success;
        }
    }
}
