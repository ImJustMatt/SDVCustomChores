using LeFauxMatt.CustomChores.Models;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedThePetChore: BaseChore
    {
        public FeedThePetChore(ChoreData choreData) : base(choreData) { }

        public override bool CanDoIt(bool today = true)
        {
            return (today && !Game1.isRaining && Game1.player.getPet() != null && !Game1.getFarm().petBowlWatered.Value)
                   || (!today && Game1.weatherForTomorrow != 1 && Game1.weatherForTomorrow != 3 && Game1.player.getPet() != null);
        }

        public override bool DoIt()
        {
            Game1.getFarm().petBowlWatered.Set(true);
            return true;
        }
    }
}
