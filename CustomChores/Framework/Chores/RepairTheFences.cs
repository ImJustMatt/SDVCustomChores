using System.Linq;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class RepairTheFences : BaseCustomChore
    {
        public override string ChoreName { get; } = "RepairTheFences";
        public RepairTheFences(CustomChores instance)
            : base(instance) { }

        public override bool CanDoIt()
        {
            return Game1.locations.SelectMany(location => location.objects.Values).OfType<Fence>().Any();
        }

        public override bool DoIt()
        {
            var success = false;

            foreach (var location in Game1.locations)
            {
                foreach (var obj in location.objects.Values)
                {
                    if (!(obj is Fence fence))
                        continue;
                    fence.repair();
                    success = true;
                }
            }

            return success;
        }
    }
}
