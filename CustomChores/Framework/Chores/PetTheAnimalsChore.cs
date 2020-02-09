using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class PetTheAnimalsChore : BaseChore
    {
        private IEnumerable<FarmAnimal> _farmAnimals;
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;
        private int _animalsPetted;

        public PetTheAnimalsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableBarns", out var enableBarns);
            ChoreData.Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = !(enableBarns is bool b1) || b1;
            _enableCoops = !(enableCoops is bool b2) || b2;
        }

        public override bool CanDoIt()
        {
            _farmAnimals =
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where !farmAnimal.wasPet.Value &&
                      ((_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn")) || 
                       (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop")))
                select farmAnimal;
            return _farmAnimals.Any();
        }

        public override bool DoIt()
        {
            _animalsPetted = 0;
            foreach (var farmAnimal in _farmAnimals)
            {

                if (farmAnimal.wasPet)
                    continue;
                farmAnimal.pet(Game1.player);
                ++_animalsPetted;
            }

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("AnimalName", GetFarmAnimalName);
            tokens.Add("AnimalsPetted", GetAnimalsPetted);
            tokens.Add("WorkDone", GetAnimalsPetted);
            return tokens;
        }

        public string GetFarmAnimalName()
        {
            var farmAnimals = (
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where (_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn", StringComparison.CurrentCultureIgnoreCase)) ||
                      (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop", StringComparison.CurrentCultureIgnoreCase))
                select farmAnimal).ToList();
            return farmAnimals.Any() ? farmAnimals.Shuffle().First().Name : null;
        }

        public string GetAnimalsPetted() => _animalsPetted.ToString(CultureInfo.InvariantCulture);
    }
}
