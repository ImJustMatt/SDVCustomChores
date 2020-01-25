﻿using StardewValley;

namespace LeFauxMatt.CustomChores.Framework
{
    public interface ICustomChore
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A unique name for the custom chore.</summary>
        string ChoreName { get; }

        /*********
        ** Public methods
        *********/
        /// <summary>Returns true if chore can be performed in current days conditions.</summary>
        bool CanDoIt(string name = null);

        /// <summary>Performs the chore.</summary>
        bool DoIt(string name = null);

        /// <summary>Performs the chore.</summary>
        /// <param name="spouse">A reference to the spouse saying the Dialogue</param>
        string GetDialogue(NPC spouse);
    }
}
