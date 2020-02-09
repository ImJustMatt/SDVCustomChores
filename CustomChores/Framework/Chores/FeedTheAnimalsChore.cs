using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using SObject = StardewValley.Object;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedTheAnimalsChore : BaseChore
    {
        private IEnumerable<AnimalHouse> _animalHouses;
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;
        private int _animalsFed;

        public FeedTheAnimalsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableBarns", out var enableBarns);
            ChoreData.Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = !(enableBarns is bool b1) || b1;
            _enableCoops = !(enableCoops is bool b2) || b2;
        }

        public override bool CanDoIt()
        {
            _animalsFed = 0;
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
            _animalsFed = 0;
            foreach (var animalHouse in _animalHouses)
            {
                _animalsFed += FeedAllAnimals(animalHouse);
            }

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("AnimalName", GetFarmAnimalName);
            tokens.Add("AnimalsFed", GetAnimalsFed);
            tokens.Add("WorkDone", GetAnimalsFed);
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

        public string GetAnimalsFed() => _animalsFed.ToString(CultureInfo.InvariantCulture);

        private static int FeedAllAnimals(AnimalHouse animalHouse)
        {
            var animalsFed = 0;
            for (var xTile = 0; xTile < animalHouse.map.Layers[0].LayerWidth; ++xTile)
            {
                for (var yTile = 0; yTile < animalHouse.map.Layers[0].LayerHeight; ++yTile)
                {
                    if (animalHouse.doesTileHaveProperty(xTile, yTile, "Trough", "Back") == null)
                        continue;
                    var key = new Vector2(xTile, yTile);
                    if (!animalHouse.objects.ContainsKey(key) || Game1.getFarm().piecesOfHay <= 0)
                        continue;
                    animalHouse.objects.Add(key, new SObject(178, 1));
                    --Game1.getFarm().piecesOfHay.Value;
                    ++animalsFed;
                    if (animalsFed >= animalHouse.animalLimit)
                        return animalsFed;
                }
            }
            return animalsFed;
        }
    }
}
