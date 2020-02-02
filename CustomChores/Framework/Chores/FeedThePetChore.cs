using System.Collections.Generic;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedThePetChore: BaseChore
    {
        public FeedThePetChore(ChoreData choreData) : base(choreData) { }

        public override bool CanDoIt()
        {
            return !Game1.isRaining && !Game1.getFarm().petBowlWatered.Value;
        }

        public override bool DoIt()
        {
            Game1.getFarm().petBowlWatered.Set(true);
            return true;
        }
    }
}
