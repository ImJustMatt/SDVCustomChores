namespace LeFauxMatt.CustomChores.Framework
{
    public interface ICustomChoresApi
    {
        /// <summary>Add a new custom chore to the game.</summary>
        /// <param name="name">The name of the chore for config/commands.</param>
        /// <param name="chore">A chore which performs one or more in-game tasks.</param>
        void AddCustomChore(string name, ICustomChore chore);
    }
}
