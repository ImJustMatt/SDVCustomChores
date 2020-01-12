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
        public IDictionary<string, IList<CustomChoreConfig>> Spouses { get; set; } = new Dictionary<string, IList<CustomChoreConfig>>();

        public ModConfig()
        {
            // Default Marriage Candidates (and Krobus)
            IList<CustomChoreConfig> spouseAlex = new List<CustomChoreConfig>();
            spouseAlex.Add(new CustomChoreConfig("FeedThePet", 1));
            spouseAlex.Add(new CustomChoreConfig("PetTheAnimals", 0.6));
            spouseAlex.Add(new CustomChoreConfig("RepairTheFences", 0.6));
            spouseAlex.Add(new CustomChoreConfig("FeedTheAnimals", 0.6));
            Spouses.Add("Alex", spouseAlex);

            IList<CustomChoreConfig> spouseElliott = new List<CustomChoreConfig>();
            spouseElliott.Add(new CustomChoreConfig("FeedThePet", 0.9)) ;
            spouseElliott.Add(new CustomChoreConfig("RepairTheFences", 0.7));
            spouseElliott.Add(new CustomChoreConfig("WaterTheCrops", 0.6));
            spouseElliott.Add(new CustomChoreConfig("PetTheAnimals", 0.6));
            Spouses.Add("Elliott", spouseElliott);

            IList<CustomChoreConfig> spouseHarvey = new List<CustomChoreConfig>();
            spouseHarvey.Add(new CustomChoreConfig("FeedThePet", 0.8));
            spouseHarvey.Add(new CustomChoreConfig("WaterTheCrops", 0.6));
            spouseHarvey.Add(new CustomChoreConfig("WaterTheSlimes", 0.6));
            spouseHarvey.Add(new CustomChoreConfig("PetTheAnimals", 0.6));
            Spouses.Add("Harvey", spouseHarvey);

            IList<CustomChoreConfig> spouseSam = new List<CustomChoreConfig>();
            spouseSam.Add(new CustomChoreConfig("FeedThePet", 0.6));
            spouseSam.Add(new CustomChoreConfig("PetTheAnimals", 0.5));
            spouseSam.Add(new CustomChoreConfig("WaterTheCrops", 0.4));
            spouseSam.Add(new CustomChoreConfig("FeedTheAnimals", 0.4));
            Spouses.Add("Sam", spouseSam);

            IList<CustomChoreConfig> spouseSebastian = new List<CustomChoreConfig>();
            spouseSebastian.Add(new CustomChoreConfig("FeedThePet", 0.6));
            spouseSebastian.Add(new CustomChoreConfig("WaterTheSlimes", 0.6));
            spouseSebastian.Add(new CustomChoreConfig("PetTheAnimals", 0.5));
            spouseSebastian.Add(new CustomChoreConfig("RepairTheFences", 0.4));
            Spouses.Add("Sebastian", spouseSebastian);

            IList<CustomChoreConfig> spouseShane = new List<CustomChoreConfig>();
            spouseShane.Add(new CustomChoreConfig("FeedThePet", 1));
            spouseShane.Add(new CustomChoreConfig("PetTheAnimals", 0.8));
            spouseShane.Add(new CustomChoreConfig("FeedTheAnimals", 0.8));
            spouseShane.Add(new CustomChoreConfig("WaterTheSlimes", 0.5));
            Spouses.Add("Shane", spouseShane);

            IList<CustomChoreConfig> spouseAbigail = new List<CustomChoreConfig>();
            spouseAbigail.Add(new CustomChoreConfig("FeedThePet", 0.8));
            spouseAbigail.Add(new CustomChoreConfig("PetTheAnimals", 0.7));
            spouseAbigail.Add(new CustomChoreConfig("WaterTheCrops", 0.6));
            spouseAbigail.Add(new CustomChoreConfig("WaterTheSlimes", 0.6));
            Spouses.Add("Abigail", spouseAbigail);

            IList<CustomChoreConfig> spouseEmily = new List<CustomChoreConfig>();
            spouseEmily.Add(new CustomChoreConfig("FeedThePet", 0.8));
            spouseEmily.Add(new CustomChoreConfig("PetTheAnimals", 0.8));
            spouseEmily.Add(new CustomChoreConfig("FeedTheAnimals", 0.6));
            spouseEmily.Add(new CustomChoreConfig("WaterTheSlimes", 0.6));
            Spouses.Add("Emily", spouseEmily);

            IList<CustomChoreConfig> spouseHaley = new List<CustomChoreConfig>();
            spouseHaley.Add(new CustomChoreConfig("FeedThePet", 0.7));
            spouseHaley.Add(new CustomChoreConfig("PetTheAnimals", 0.5));
            spouseHaley.Add(new CustomChoreConfig("FeedTheAnimals", 0.4));
            spouseHaley.Add(new CustomChoreConfig("WaterTheSlimes", 0.4));
            Spouses.Add("Haley", spouseHaley);

            IList<CustomChoreConfig> spouseLeah = new List<CustomChoreConfig>();
            spouseLeah.Add(new CustomChoreConfig("FeedThePet", 0.9));
            spouseLeah.Add(new CustomChoreConfig("PetTheAnimals", 0.8));
            spouseLeah.Add(new CustomChoreConfig("WaterTheCrops", 0.6));
            spouseLeah.Add(new CustomChoreConfig("RepairTheFences", 0.5));
            Spouses.Add("Leah", spouseLeah);

            IList<CustomChoreConfig> spouseMaru = new List<CustomChoreConfig>();
            spouseMaru.Add(new CustomChoreConfig("FeedThePet", 0.9));
            spouseMaru.Add(new CustomChoreConfig("RepairTheFences", 0.8));
            spouseMaru.Add(new CustomChoreConfig("PetTheAnimals", 0.7));
            spouseMaru.Add(new CustomChoreConfig("WaterTheCrops", 0.6));
            Spouses.Add("Maru", spouseMaru);

            IList<CustomChoreConfig> spousePenny = new List<CustomChoreConfig>();
            spousePenny.Add(new CustomChoreConfig("FeedThePet", 0.9));
            spousePenny.Add(new CustomChoreConfig("PetTheAnimals", 0.8));
            spousePenny.Add(new CustomChoreConfig("WaterTheCrops", 0.6));
            spousePenny.Add(new CustomChoreConfig("FeedTheAnimals", 0.6));
            Spouses.Add("Penny", spousePenny);

            IList<CustomChoreConfig> spouseKrobus = new List<CustomChoreConfig>();
            spouseKrobus.Add(new CustomChoreConfig("WaterTheSlimes", 1));
            Spouses.Add("Krobus", spouseKrobus);
        }
    }
}
