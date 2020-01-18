using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedThePet: BaseCustomChore
    {
        public override string ChoreName { get; } = "FeedThePet";
        public FeedThePet(CustomChores instance)
            : base(instance) { }

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
