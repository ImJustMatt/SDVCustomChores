using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework
{
    public class CustomChoresAPI: ICustomChoresAPI
    {
        private readonly IMonitor Monitor;
        private readonly IDictionary<string, ICustomChore> Chores;

        /// <summary>Construct an instance</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="chores">The custom chores.</param>
        internal CustomChoresAPI(IMonitor monitor, IDictionary<string, ICustomChore> chores)
        {
            this.Monitor = monitor;
            this.Chores = chores;
        }

        /// <summary>Add a new custom chore to the game.</summary>
        /// <param name="chore">A chore which performs one or more in-game tasks.</param>
        public void AddCustomChore(ICustomChore chore)
        {
            this.Monitor.Log($"Adding custom chore: {chore.GetType().AssemblyQualifiedName}", LogLevel.Trace);
            this.Chores.Add(chore.ChoreName, chore);
        }
    }
}
