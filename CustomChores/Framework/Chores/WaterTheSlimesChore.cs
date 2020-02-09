using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheSlimesChore : BaseChore
    {
        private IList<SlimeHutch> _slimeHutches;
        private int _slimesWatered;

        public WaterTheSlimesChore(ChoreData choreData) : base(choreData) { }

        public override bool CanDoIt()
        {
            _slimeHutches =  (
                from building in Game1.getFarm().buildings
                where building.daysOfConstructionLeft.Value <= 0
                      && building.indoors.Value is SlimeHutch
                select building.indoors.Value as SlimeHutch).ToList();

            return _slimeHutches.Any();
        }

        public override bool DoIt()
        {
            _slimesWatered = 0;
            foreach (var slimeHutch in _slimeHutches)
            {
                for (var index = 0; index < slimeHutch.waterSpots.Count; ++index)
                {
                    if (slimeHutch.waterSpots[index])
                        continue;

                    slimeHutch.waterSpots[index] = true;
                    ++_slimesWatered;
                }
            }

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("SlimesWatered", GetSlimesWatered);
            tokens.Add("WorkDone", GetSlimesWatered);
            return tokens;
        }

        public string GetSlimesWatered() => _slimesWatered.ToString(CultureInfo.InvariantCulture);
    }
}
