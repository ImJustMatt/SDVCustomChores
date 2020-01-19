using System.Collections.Generic;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedThePet: BaseCustomChore
    {
        public override string ChoreName { get; } = "FeedThePet";
        public FeedThePet(CustomChores instance, IDictionary<string, string> config)
            : base(instance, config) { }

        public override bool CanDoIt(string name = null)
        {
            return !Game1.isRaining && !Game1.getFarm().petBowlWatered.Value;
        }

        public override bool DoIt(string name = null)
        {
            Game1.getFarm().petBowlWatered.Set(true);
            return true;
        }
    }
}
