namespace LeFauxMatt.CustomChores.Framework
{
    internal abstract class BaseCustomChore : ICustomChore
    {
        private readonly CustomChores _modInstance;
        public abstract string ChoreName { get; }
        protected BaseCustomChore(CustomChores instance)
        {
            this._modInstance = instance;
        }

        public abstract bool CanDoIt();
        public abstract bool DoIt();

        public string GetDialogue(string name)
        {
            return _modInstance.GetDialogue(name, ChoreName);
        }
    }
}