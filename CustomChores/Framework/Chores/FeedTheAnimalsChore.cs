using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework.Graphics;
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

        public override IDictionary<string, string> GetTokens()
        {
            var tokens = base.GetTokens();
            var farmAnimals =
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where (_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn")) ||
                      (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop"))
                select farmAnimal;
            if (farmAnimals.Any())
                tokens.Add("animalName", farmAnimals.Shuffle().First().Name);
            return tokens;
        }
    }
}
