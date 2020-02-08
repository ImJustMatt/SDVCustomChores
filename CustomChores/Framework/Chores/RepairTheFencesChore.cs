using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
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
            
            _fences = locations.SelectMany(location => location.objects.Values).OfType<Fence>();
            
            return _fences.Any();
        }

        public override bool DoIt()
        {
            foreach (var fence in _fences)
            {
                fence.repair();
            }

            return true;
        }
    }
}
