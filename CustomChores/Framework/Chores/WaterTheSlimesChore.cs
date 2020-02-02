using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheSlimesChore : BaseChore
    {
        private IEnumerable<SlimeHutch> _slimeHutches;
        public WaterTheSlimesChore(ChoreData choreData) : base(choreData) { }

        public override bool CanDoIt()
        {
            _slimeHutches =  (
                from building in Game1.getFarm().buildings
                where building.daysOfConstructionLeft.Value <= 0
                      && building.indoors.Value is SlimeHutch
                select building.indoors.Value as SlimeHutch);

            return _slimeHutches.Any();
        }

        public override bool DoIt()
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
