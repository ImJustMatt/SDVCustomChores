using System.Collections.Generic;
using System.Dynamic;

namespace LeFauxMatt.CustomChores.Framework
{
    internal abstract class BaseCustomChore : ICustomChore
    {
        protected readonly CustomChores ModInstance;
        protected IDictionary<string, string> Config;
        public abstract string ChoreName { get; }
        protected BaseCustomChore(CustomChores instance, IDictionary<string, string> config)
        {
            ModInstance = instance;
            Config = config;
        }

        public abstract bool CanDoIt(string name = null);
        public abstract bool DoIt(string name = null);

        public virtual string GetDialogue(string name)
        {
            return ModInstance.GetDialogue(name, ChoreName);
        }
    }
}