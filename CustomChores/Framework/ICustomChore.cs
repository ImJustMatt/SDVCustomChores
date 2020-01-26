using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework
{
    public interface ICustomChore
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Returns true if chore can be performed in current days conditions.</summary>
        /// <param name="spouse">A reference to the farmer's spouse.</param>
        bool CanDoIt(NPC spouse);

        /// <summary>Performs the chore.</summary>
        /// <param name="spouse">A reference to the farmer's spouse.</param>
        bool DoIt(NPC spouse);

        /// <summary>Performs the chore.</summary>
        /// <param name="spouse">A reference to the farmer's spouse.</param>
        Translation GetDialogue(NPC spouse);
    }
}
