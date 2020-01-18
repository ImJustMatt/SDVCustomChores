namespace LeFauxMatt.CustomChores.Framework
{
    public interface ICustomChoresApi
    {
        /// <summary>Add a chore.</summary>
        void AddCustomChore(ICustomChore chore);
    }
}
