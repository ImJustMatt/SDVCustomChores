using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class PetTheAnimals : BaseCustomChore
    {
        private IEnumerable<FarmAnimal> _farmAnimals;
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;

        public PetTheAnimals(string choreName, IDictionary<string, string> config, IList<Translation> dialogue) : base(choreName, config, dialogue)
        {
            Config.TryGetValue("EnableBarns", out var enableBarns);
            Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = string.IsNullOrWhiteSpace(enableBarns) || Convert.ToBoolean(enableBarns);
            _enableCoops = string.IsNullOrWhiteSpace(enableCoops) || Convert.ToBoolean(enableCoops);
        }

        public override bool CanDoIt(string name = null)
        {
            _farmAnimals =
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where !farmAnimal.wasPet.Value &&
                      ((_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn")) || 
                       (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop")))
                select farmAnimal;
            return _farmAnimals.Any();
        }

        public override bool DoIt(string name = null)
        {
            foreach (var farmAnimal in _farmAnimals)
            {
                farmAnimal.pet(Game1.player);
            }

            return true;
        }
    }
}
