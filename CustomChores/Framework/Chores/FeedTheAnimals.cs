using System.Linq;
using StardewValley;
using StardewValley.Buildings;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedTheAnimals : BaseCustomChore
    {
        public override string ChoreName { get; } = "FeedTheAnimals";
        public FeedTheAnimals(CustomChores instance)
            : base(instance) { }

        public override bool CanDoIt()
        {
            return Game1.getFarm().getAllFarmAnimals().Count > 0;
        }

        public override bool DoIt()
        {
            var success = false;

            foreach (var building in Game1.getFarm().buildings.Where(building => building.daysOfConstructionLeft.Value <= 0).Where(building => building is Barn || building is Coop))
            {
                (building.indoors.Value as AnimalHouse)?.feedAllAnimals();
                success = true;
            }

            return success;
        }
    }
}
