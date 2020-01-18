using System.Collections.Generic;

namespace LeFauxMatt.CustomChores.Models
{
    public class ModConfig
    {
        /// <summary>The maximum number of chores any spouse will perform in one day.</summary>
        public int DailyLimit { get; set; } = 3;

        /// <summary>The minimum number of hearts required for spouse to perform any chore.</summary>
        public int HeartsNeeded { get; set; } = 10;

        /// <summary>Overall chance that any chore gets performed.</summary>
        public double GlobalChance { get; set; } = 1;

        /// <summary>Determines whether spouse will say a random dialogue option in the morning about the chore they performed.</summary>
        public bool EnableDialogue { get; set; } = false;

        /// <summary>The spouses that will be able to perform chores.</summary>
        public IDictionary<string, string> Spouses { get; set; } = new Dictionary<string, string>();

        public ModConfig()
        {
            // Default Marriage Candidates (and Krobus)
            Spouses.Add("Alex", "FeedThePet 1\\PetTheAnimals 0.6\\RepairTheFences 0.6\\FeedTheAnimals 0.6");
            Spouses.Add("Elliott","FeedThePet 0.9\\RepairTheFences 0.7\\WaterTheCrops 0.6\\PetTheAnimals 0.6");
            Spouses.Add("Harvey","FeedThePet 0.8\\WaterTheCrops 0.6\\WaterTheSlimes 0.6\\PetTheAnimals 0.6");
            Spouses.Add("Sam","FeedThePet 0.6\\PetTheAnimals 0.5\\WaterTheCrops 0.4\\FeedTheAnimals 0.4");
            Spouses.Add("Sebastian","FeedThePet 0.6\\WaterTheSlimes 0.6\\PetTheAnimals 0.5\\RepairTheFences 0.4");
            Spouses.Add("Shane","FeedThePet 1\\PetTheAnimals 0.8\\FeedTheAnimals 0.8\\WaterTheSlimes 0.5");
            Spouses.Add("Abigail","FeedThePet 0.8\\PetTheAnimals 0.7\\WaterTheCrops 0.6\\WaterTheSlimes 0.6");
            Spouses.Add("Emily","FeedThePet 0.8\\PetTheAnimals 0.8\\FeedTheAnimals 0.6\\WaterTheSlimes 0.6");
            Spouses.Add("Haley","FeedThePet 0.7\\PetTheAnimals 0.5\\FeedTheAnimals 0.4\\WaterTheSlimes 0.4");
            Spouses.Add("Leah","FeedThePet 0.9\\PetTheAnimals 0.8\\WaterTheCrops 0.6\\RepairTheFences 0.5");
            Spouses.Add("Maru","FeedThePet 0.9\\RepairTheFences 0.8\\PetTheAnimals 0.7\\WaterTheCrops 0.6");
            Spouses.Add("Penny","FeedThePet 0.9\\PetTheAnimals 0.8\\WaterTheCrops 0.6\\FeedTheAnimals 0.6");
            Spouses.Add("Krobus","WaterTheSlimes 1");
        }
    }
}
