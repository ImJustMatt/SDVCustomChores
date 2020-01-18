using StardewValley;
using StardewValley.TerrainFeatures;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheCrops : BaseCustomChore
    {
        public override string ChoreName { get; } = "WaterTheCrops";
        public WaterTheCrops(CustomChores instance)
            : base(instance) { }

        public override bool CanDoIt()
        {
            return !Game1.isRaining && !Game1.currentSeason.Equals("winter");
        }

        public override bool DoIt()
        {
            var success = false;

            foreach (var location in Game1.locations)
            {
                if (!location.IsFarm && !location.IsGreenhouse)
                    continue;

                foreach (var terrainFeature in location.terrainFeatures.Values)
                {
                    if (!(terrainFeature is HoeDirt dirt))
                        continue;
                    dirt.state.Value = HoeDirt.watered;
                    success = true;
                }
            }

            return success;
        }
    }
}
