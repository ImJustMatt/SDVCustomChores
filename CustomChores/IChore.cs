using System;
using System.Collections.Generic;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace LeFauxMatt.CustomChores
{
    public interface IChore
    {
        ChoreData ChoreData { get; }

        /*********
        ** Public methods
        *********/
        /// <summary>Returns true if chore can be performed based on the current days conditions.</summary>
        bool CanDoIt();

        /// <summary>Performs the chore and returns true/false on success or failure.</summary>
        bool DoIt();

        /// <summary>Returns tokens for substitution.</summary>
        IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper);
    }
}
