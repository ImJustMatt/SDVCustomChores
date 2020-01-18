using System.Collections.Generic;
using StardewModdingAPI;

namespace LeFauxMatt.CustomChores.Framework
{
    public class CustomChoresApi: ICustomChoresApi
    {
        private readonly IMonitor _monitor;
        private readonly IDictionary<string, ICustomChore> _chores;

        /// <summary>Construct an instance</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="chores">The custom chores.</param>
        internal CustomChoresApi(IMonitor monitor, IDictionary<string, ICustomChore> chores)
        {
            this._monitor = monitor;
            this._chores = chores;
        }

        /// <summary>Add a new custom chore to the game.</summary>
        /// <param name="chore">A chore which performs one or more in-game tasks.</param>
        public void AddCustomChore(ICustomChore chore)
        {
            this._monitor.Log($"Adding custom chore: {chore.GetType().AssemblyQualifiedName}", LogLevel.Trace);
            this._chores.Add(chore.ChoreName, chore);
        }
    }
}
