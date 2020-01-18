using System.Linq;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class PetTheAnimals : BaseCustomChore
    {
        public override string ChoreName { get; } = "PetTheAnimals";

        public PetTheAnimals(CustomChores instance)
            : base(instance) { }

        public override bool CanDoIt()
        {
            return Game1.getFarm().getAllFarmAnimals().Count > 0;
        }

        public override bool DoIt()
        {
            var success = false;

            foreach (var farmAnimal in Game1.getFarm().getAllFarmAnimals().Where(farmAnimal => !farmAnimal.wasPet.Value))
            {
                farmAnimal.pet(Game1.player);
                success = true;
            }

            return success;
        }
    }
}
