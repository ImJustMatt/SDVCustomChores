using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class WaterTheCrops: ICustomChore
    {
        public string ChoreName { get; } = "WaterTheCrops";

        public bool CanDoIt()
        {
            return !Game1.isRaining && !Game1.currentSeason.Equals("winter");
        }

        public bool DoIt()
        {
            bool success = false;

            foreach (GameLocation location in Game1.locations)
            {
                if (!location.IsFarm && !location.IsGreenhouse)
                    continue;

                foreach (TerrainFeature terrainFeature in location.terrainFeatures.Values)
                {
                    if (terrainFeature is HoeDirt dirt)
                    {
                        dirt.state.Value = HoeDirt.watered;
                        success = true;
                    }
                }
            }

            return success;
        }

        public string GetDialogue(string spouseName)
        {
            Translation translation = CustomChores._helper.Translation.Get($"{spouseName}.{ChoreName}");
            return translation.ToString();
        }
    }
}
