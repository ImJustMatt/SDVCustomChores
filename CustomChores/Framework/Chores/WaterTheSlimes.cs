using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class WaterTheSlimes : ICustomChore
    {
        public string ChoreName { get; } = "WaterTheSlimes";

        private readonly CustomChores ModInstance;

        public WaterTheSlimes(CustomChores instance)
        {
            this.ModInstance = instance;
        }

        public bool CanDoIt()
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.daysOfConstructionLeft.Value > 0)
                    continue;

                if (building.indoors.Value is SlimeHutch)
                    return true;
            }
            return false;
        }

        public bool DoIt()
        {
            bool success = false;

            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.daysOfConstructionLeft.Value > 0)
                    continue;

                if (building.indoors.Value is SlimeHutch slimeHutch)
                {
                    for (int index = 0; index < slimeHutch.waterSpots.Count; ++index)
                        slimeHutch.waterSpots[index] = true;

                    success = true;
                }
            }

            return success;
        }

        public string GetDialogue(string spouseName)
        {
            return ModInstance.GetDialogue(spouseName, ChoreName);
        }
    }
}
