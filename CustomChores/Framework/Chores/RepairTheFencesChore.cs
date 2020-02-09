using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class RepairTheFencesChore : BaseChore
    {
        private IEnumerable<Fence> _fences;
        private readonly bool _enableFarm;
        private readonly bool _enableBuildings;
        private readonly bool _enableOutdoors;
        private int _fencesRepaired;

        public RepairTheFencesChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableFarm", out var enableFarm);
            ChoreData.Config.TryGetValue("EnableBuildings", out var enableBuildings);
            ChoreData.Config.TryGetValue("EnableOutdoors", out var enableOutdoors);

            _enableFarm = !(enableFarm is bool b1) || b1;
            _enableBuildings = !(enableBuildings is bool b2) || b2;
            _enableOutdoors = !(enableOutdoors is bool b3) || b3;
        }

        public override bool CanDoIt()
        {
            var locations =
                from location in Game1.locations
                where (_enableFarm && location.IsFarm) ||
                      (_enableOutdoors && location.IsOutdoors)
                select location;

            if (_enableBuildings)
                locations = locations.Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value);
            
            _fences = locations
                .SelectMany(location => location.objects.Values)
                .OfType<Fence>()
                .Where(fence => fence.health.Value < fence.maxHealth.Value);
            
            return _fences.Any();
        }

        public override bool DoIt()
        {
            _fencesRepaired = 0;
            foreach (var fence in _fences)
            {
                fence.repair();
                ++_fencesRepaired;
            }

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("FencesRepaired", GetFencesRepaired);
            tokens.Add("WorkDone", GetFencesRepaired);
            return tokens;
        }

        public string GetFencesRepaired() => _fencesRepaired.ToString(CultureInfo.InvariantCulture);
    }
}
