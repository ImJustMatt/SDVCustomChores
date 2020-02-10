using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedTheAnimalsChore : BaseChore
    {
        private IEnumerable<AnimalHouse> _animalHouses;
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;

        public FeedTheAnimalsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableBarns", out var enableBarns);
            ChoreData.Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = !(enableBarns is bool b1) || b1;
            _enableCoops = !(enableCoops is bool b2) || b2;
        }

        public override bool CanDoIt()
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

        public override bool DoIt()
        {
            foreach (var animalHouse in _animalHouses)
            {
                animalHouse.feedAllAnimals();
            }

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("AnimalName", GetFarmAnimalName);
            return tokens;
        }

        public string GetFarmAnimalName()
        {
            var farmAnimals =
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where (_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn")) ||
                      (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop"))
                select farmAnimal;
            return farmAnimals.Any() ? farmAnimals.Shuffle().First().Name : null;
        }
    }
}
