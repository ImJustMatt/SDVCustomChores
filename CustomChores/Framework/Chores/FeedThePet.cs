using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class FeedThePet: ICustomChore
    {
        public string ChoreName { get; } = "FeedThePet";

        private readonly CustomChores ModInstance;

        public FeedThePet(CustomChores instance)
        {
            this.ModInstance = instance;
        }

        public bool CanDoIt()
        {
            return !Game1.isRaining && !Game1.getFarm().petBowlWatered.Value;
        }

        public bool DoIt()
        {
            Game1.getFarm().petBowlWatered.Set(true);
            return true;
        }

        public string GetDialogue(string spouseName)
        {
            return ModInstance.GetDialogue(spouseName, ChoreName);
        }
    }
}
