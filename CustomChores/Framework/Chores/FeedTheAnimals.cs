using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class FeedTheAnimals: ICustomChore
    {
        public string ChoreName { get; } = "FeedTheAnimals";

        private readonly CustomChores ModInstance;

        public FeedTheAnimals(CustomChores instance)
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

            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.daysOfConstructionLeft.Value > 0)
                    continue;

                if (building is Barn || building is Coop)
                {
                    (building.indoors.Value as AnimalHouse).feedAllAnimals();
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
