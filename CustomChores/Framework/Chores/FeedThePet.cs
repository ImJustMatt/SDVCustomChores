using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class FeedThePet: ICustomChore
    {
        public string ChoreName { get; } = "FeedThePet";

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
            string petName = Game1.player.getPetName();
            Translation translation = CustomChores._helper.Translation.Get($"{spouseName}.{ChoreName}", new { petName = petName });
            return translation.ToString();
        }
    }
}
