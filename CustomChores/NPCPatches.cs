using System;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores
{
    internal class NPCPatches
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void marriageDuties_Prefix()
        {
            try
            {
                // Prevent default chores from occurring
                StardewValley.NPC.hasSomeoneFedTheAnimals = true;
                StardewValley.NPC.hasSomeoneFedThePet = true;
                StardewValley.NPC.hasSomeoneRepairedTheFences = true;
                StardewValley.NPC.hasSomeoneWateredCrops = true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(marriageDuties_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
