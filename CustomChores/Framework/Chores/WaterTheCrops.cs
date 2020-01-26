using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheCrops : BaseCustomChore
    {
        private IEnumerable<HoeDirt> _hoeDirt;
        private readonly bool _enableFarm;
        private readonly bool _enableBuildings;
        private readonly bool _enableGreenhouse;

        public WaterTheCrops(string choreName, IDictionary<string, string> config, IList<Translation> dialogue) : base(choreName, config, dialogue)
        {
            Config.TryGetValue("EnableFarm", out var enableFarm);
            Config.TryGetValue("EnableBuildings", out var enableBuildings);
            Config.TryGetValue("EnableGreenhouse", out var enableGreenhouse);

            _enableFarm = string.IsNullOrWhiteSpace(enableFarm) || Convert.ToBoolean(enableFarm);
            _enableBuildings = string.IsNullOrWhiteSpace(enableBuildings) || Convert.ToBoolean(enableBuildings);
            _enableGreenhouse = string.IsNullOrWhiteSpace(enableGreenhouse) || Convert.ToBoolean(enableGreenhouse);
        }

        public override bool CanDoIt(string name = null)
        {
            if (Game1.isRaining || Game1.currentSeason.Equals("winter"))
                return false;
            
            var locations = Game1.locations
                .Where(location => (_enableFarm && location.IsFarm) || (_enableGreenhouse && location.IsGreenhouse));
            
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

        public override bool DoIt(string name = null)
        {
            foreach (var hoeDirt in _hoeDirt)
            {
                hoeDirt.state.Value = HoeDirt.watered;
            }

            return true;
        }
    }
}
