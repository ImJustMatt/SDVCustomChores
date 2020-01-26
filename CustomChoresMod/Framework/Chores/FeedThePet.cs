using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedThePet: BaseCustomChore
    {
        public FeedThePet(string choreName, IDictionary<string, string> config, IEnumerable<Translation> dialogue)
            : base(choreName, config, dialogue) { }

        public override bool CanDoIt(NPC spouse)
        {
            return !Game1.isRaining && !Game1.getFarm().petBowlWatered.Value;
        }

        public override bool DoIt(NPC spouse)
        {
            Game1.getFarm().petBowlWatered.Set(true);
            return true;
        }
    }
}
