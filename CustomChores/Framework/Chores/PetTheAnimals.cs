using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class PetTheAnimals : ICustomChore
    {
        public string ChoreName { get; } = "PetTheAnimals";

        public bool CanDoIt()
        {
            return Game1.getFarm().getAllFarmAnimals().Count > 0;
        }

        public bool DoIt()
        {
            bool success = false;

            foreach (FarmAnimal farmAnimal in Game1.getFarm().getAllFarmAnimals())
            {
                if (!farmAnimal.wasPet.Value)
                {
                    farmAnimal.pet(Game1.player);
                    success = true;
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
