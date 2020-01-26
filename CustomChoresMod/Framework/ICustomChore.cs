using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework
{
    public interface ICustomChore
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Returns true if chore can be performed based on the current days conditions.</summary>
        /// <param name="spouse">A reference to the farmer's spouse.</param>
        bool CanDoIt(NPC spouse);

        /// <summary>Performs the chore and returns true/false on success or failure.</summary>
        /// <param name="spouse">A reference to the farmer's spouse.</param>
        bool DoIt(NPC spouse);

        /// <summary>Returns the dialogue to be displayed on the day the chore is performed.</summary>
        /// <param name="spouse">A reference to the farmer's spouse.</param>
        Translation GetDialogue(NPC spouse);
    }
}
