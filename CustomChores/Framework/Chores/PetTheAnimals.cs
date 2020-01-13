using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class PetTheAnimals : ICustomChore
    {
        public string ChoreName { get; } = "PetTheAnimals";

        private readonly CustomChores ModInstance;

        public PetTheAnimals(CustomChores instance)
        {
            this.ModInstance = instance;
        }

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
            return ModInstance.GetDialogue(spouseName, ChoreName);
        }
    }
}
