using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedTheAnimals : BaseCustomChore
    {
        private IEnumerable<AnimalHouse> _animalHouses;
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;

        public FeedTheAnimals(string choreName, IDictionary<string, string> config, IEnumerable<Translation> dialogue) : base(choreName, config, dialogue)
        {
            Config.TryGetValue("EnableBarns", out var enableBarns);
            Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = string.IsNullOrWhiteSpace(enableBarns) || Convert.ToBoolean(enableBarns);
            _enableCoops = string.IsNullOrWhiteSpace(enableCoops) || Convert.ToBoolean(enableCoops);
        }

        public override bool CanDoIt(NPC spouse)
        {
            _animalHouses = (
                    from building in Game1.getFarm().buildings
                    where building.daysOfConstructionLeft <= 0 &&
                          ((_enableBarns && building is Barn) ||
                           (_enableCoops && building is Coop))
                    select building.indoors.Value)
                .OfType<AnimalHouse>();
            return _animalHouses.Any();
        }

        public override bool DoIt(NPC spouse)
        {
            foreach (var animalHouse in _animalHouses)
            {
                animalHouse.feedAllAnimals();
            }

            return true;
        }

        public override Translation GetDialogue(NPC spouse)
        {
            var farmAnimals =
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where ((_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn")) ||
                       (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop")))
                select farmAnimal;

            return base.GetDialogue(spouse).Tokens(new
            {
                animalName = farmAnimals.Shuffle().First().Name
            });
        }
    }
}
