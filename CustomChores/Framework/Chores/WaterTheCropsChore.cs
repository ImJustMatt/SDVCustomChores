using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheCropsChore : BaseChore
    {
        private IEnumerable<HoeDirt> _hoeDirt;
        private readonly bool _enableFarm;
        private readonly bool _enableBuildings;
        private readonly bool _enableGreenhouse;

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
            if (Game1.isRaining || Game1.currentSeason.Equals("winter"))
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
                .OfType<HoeDirt>();

            return _hoeDirt.Any();
        }

        public override bool DoIt()
        {
            foreach (var hoeDirt in _hoeDirt)
            {
                hoeDirt.state.Value = HoeDirt.watered;
            }
            
            return true;
        }
    }
}
