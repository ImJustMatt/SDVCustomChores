﻿using System.Collections.Generic;
using StardewModdingAPI;

namespace LeFauxMatt.HelpForHire.Models
{
    public class ModConfig
    {
        /// <summary>The chores that will be available for purchase.</summary>
        public IDictionary<string, int> Chores { get; set; } = new Dictionary<string, int>();

        /// <summary>The button used to activate the shop menu.</summary>
        public SButton ShopMenuButton { get; set; } = SButton.P;

        public ModConfig()
        {
            Chores.Add("furyx639.FeedTheAnimals", 1000);
            Chores.Add("furyx639.FeedThePet", 250);
            Chores.Add("furyx639.PetTheAnimals", 2000);
            Chores.Add("furyx639.RepairTheFences", 2000);
            Chores.Add("furyx639.WaterTheCrops", 2000);
            Chores.Add("furyx639.WaterTheSlimes", 1000);
        }
    }
}
