using System.Linq;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheSlimes : ICustomChore
    {
        public string ChoreName { get; } = "WaterTheSlimes";

        private readonly CustomChores _modInstance;

        public WaterTheSlimes(CustomChores instance)
        {
            this._modInstance = instance;
        }

        public bool CanDoIt()
        {
            return Game1.getFarm().buildings
                .Where(building => building.daysOfConstructionLeft.Value <= 0)
                .Any(building => building.indoors.Value is SlimeHutch);
        }

        public bool DoIt()
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

        public string GetDialogue(string spouseName)
        {
            return _modInstance.GetDialogue(spouseName, ChoreName);
        }
    }
}
