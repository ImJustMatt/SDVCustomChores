﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;

namespace LeFauxMatt.CustomChores.Framework
{
    public class CustomChoresApi : ICustomChoresApi
    {
        /*********
        ** Fields
        *********/
        /// <summary>Encapsulates monitoring and logging.</summary>
        private readonly IMonitor _monitor;

        /// <summary>Adds new types of custom chore.</summary>
        private readonly ChoreBuilder _choreBuilders;

        /// <summary>Instances of custom chores created by content packs.</summary>
        private readonly IDictionary<string, IChore> _chores;

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="choreBuilders">The custom chores.</param>
        /// /// <param name="chores"></param>
        internal CustomChoresApi(IMonitor monitor, ChoreBuilder choreBuilders, IDictionary<string, IChore> chores)
        {
            _monitor = monitor;
            _choreBuilders = choreBuilders;
            _chores = chores;
        }

        /// <summary>Adds new types of custom chore.</summary>
        /// <param name="factory">A factory which creates an instance of a chore type.</param>
        public void AddChoreFactory(IChoreFactory factory)
        {
            _monitor.Log($"Adding chore factory: {factory.GetType().AssemblyQualifiedName}", LogLevel.Trace);
            _choreBuilders.AddChoreFactory(factory);
        }

        /// <summary>Get a list of chores.r</summary>
        /// <returns>List of chores available by name.</returns>
        public IList<string> GetChores() => _chores.Keys.ToList();

        /// <summary>Get a single chore.</summary>
        /// <returns>Single instance of chore object by name.</returns>
        public ChoreData GetChore(string choreName)
        {
            _chores.TryGetValue(choreName, out var chore);
            return chore?.ChoreData;
        }

        /// <summary>Performs a chore.</summary>
        /// <returns>True if chore is successfully performed.</returns>
        public bool DoChore(string choreName)
        {
            _chores.TryGetValue(choreName, out var chore);
            return !(chore is null) && chore.DoIt();
        }

        /// <summary>Checks if a chore can be done.</summary>
        /// <returns>True if current conditions allows chore to be done.</returns>
        public bool CheckChore(string choreName)
        {
            _chores.TryGetValue(choreName, out var chore);
            return !(chore is null) && chore.CanDoIt();
        }

        /// <summary>Gets chore tokens.</summary>
        /// <returns>Dictionary of chore tokens.</returns>
        public IDictionary<string, string> GetChoreTokens(string choreName)
        {
            _chores.TryGetValue(choreName, out var chore);
            return chore?.GetTokens();
        }
    }
}