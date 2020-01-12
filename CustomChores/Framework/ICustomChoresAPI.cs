using System;
using System.Collections.Generic;

namespace LeFauxMatt.CustomChores.Framework
{
    public interface ICustomChoresAPI
    {
        /// <summary>Add a chore.</summary>
        void AddCustomChore(ICustomChore chore);
    }
}
