using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheSlimes : BaseCustomChore
    {
        private IEnumerable<SlimeHutch> _slimeHutches;
        public WaterTheSlimes(string choreName, IDictionary<string, string> config, IEnumerable<Translation> dialogue)
            : base(choreName, config, dialogue) { }

        public override bool CanDoIt(string name = null)
        {
            _slimeHutches =  (
                from building in Game1.getFarm().buildings
                where building.daysOfConstructionLeft.Value <= 0
                      && building.indoors.Value is SlimeHutch
                select building.indoors.Value as SlimeHutch);

            return _slimeHutches.Any();
        }

        public override bool DoIt(string name = null)
        {
            foreach (var slimeHutch in _slimeHutches)
            {
                for (var index = 0; index < slimeHutch.waterSpots.Count; ++index)
                    slimeHutch.waterSpots[index] = true;
            }

            return true;
        }
    }
}
