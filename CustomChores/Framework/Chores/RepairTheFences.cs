using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    class RepairTheFences: ICustomChore
    {
        public string ChoreName { get; } = "RepairTheFences";

        public bool CanDoIt()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (SObject obj in location.objects.Values)
                {
                    if (obj is Fence)
                        return true;
                }
            }

            return false;
        }

        public bool DoIt()
        {
            bool success = false;

            foreach (GameLocation location in Game1.locations)
            {
                foreach (SObject obj in location.objects.Values)
                {
                    if (obj is Fence fence)
                    {
                        fence.repair();
                        success = true;
                    }
                }
            }

            return success;
        }

        public string GetDialogue(string spouseName)
        {
            Translation translation = CustomChores._helper.Translation.Get($"{spouseName}.{ChoreName}");
            return translation.ToString();
        }
    }
}
