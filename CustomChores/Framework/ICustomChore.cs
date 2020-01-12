using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        bool CanDoIt();

        /// <summary>Performs the chore.</summary>
        bool DoIt();

        /// <summary>Performs the chore.</summary>
        /// <param name="spouseName">The name of the spouse.</param>
        string GetDialogue(string spouseName);
    }
}
