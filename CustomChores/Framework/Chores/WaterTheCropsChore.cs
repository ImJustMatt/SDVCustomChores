using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheCropsChore : BaseChore
    {
        private IList<HoeDirt> _hoeDirt;
        private readonly bool _enableFarm;
        private readonly bool _enableBuildings;
        private readonly bool _enableGreenhouse;
        private int _cropsWatered;

        public WaterTheCropsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableFarm", out var enableFarm);
            ChoreData.Config.TryGetValue("EnableBuildings", out var enableBuildings);
            ChoreData.Config.TryGetValue("EnableGreenhouse", out var enableGreenhouse);

            _enableFarm = !(enableFarm is bool b1) || b1;
            _enableBuildings = !(enableBuildings is bool b2) || b2;
            _enableGreenhouse = !(enableGreenhouse is bool b3) || b3;
        }

        public override bool CanDoIt()
        {
            if (Game1.isRaining || Game1.currentSeason.Equals("winter", StringComparison.CurrentCultureIgnoreCase))
                return false;

            var locations =
                from location in Game1.locations
                where (_enableFarm && location.IsFarm) ||
                      (_enableGreenhouse && location.IsGreenhouse)
                select location;
            
            if (_enableBuildings)
                locations = locations.Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value);
            
            _hoeDirt = locations
                .SelectMany(location => location.terrainFeatures.Values)
                .OfType<HoeDirt>()
                .Where(hoeDirt => hoeDirt.needsWatering())
                .ToList();

            return _hoeDirt.Any();
        }

        public override bool DoIt()
        {
            _cropsWatered = 0;
            foreach (var hoeDirt in _hoeDirt)
            {
                if (!hoeDirt.needsWatering())
                    continue;
                hoeDirt.state.Value = HoeDirt.watered;
                _cropsWatered++;
            }
            
            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("CropsWatered", GetCropsWatered);
            tokens.Add("WorkDone", GetCropsWatered);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        public string GetCropsWatered() =>
            _cropsWatered.ToString(CultureInfo.InvariantCulture);

        public string GetWorkNeeded() =>
            _hoeDirt.Count.ToString(CultureInfo.InvariantCulture);
    }
}
