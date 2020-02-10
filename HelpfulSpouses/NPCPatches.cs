using System;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.HelpfulSpouses
{
    internal class NpcPatches
    {
        private static IMonitor _monitor;

        public static void Initialize(IMonitor monitor)
        {
            _monitor = monitor;
        }

        public static void MarriageDuties_Prefix()
        {
            try
            {
                // Prevent default chores from occurring
                NPC.hasSomeoneFedTheAnimals = true;
                NPC.hasSomeoneFedThePet = true;
                NPC.hasSomeoneRepairedTheFences = true;
                NPC.hasSomeoneWateredCrops = true;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MarriageDuties_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
