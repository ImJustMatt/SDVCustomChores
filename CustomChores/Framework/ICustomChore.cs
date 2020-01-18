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
        /// <param name="name">The name of the spouse.</param>
        string GetDialogue(string name);
    }
}
