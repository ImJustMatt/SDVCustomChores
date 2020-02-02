using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework
{
    internal abstract class BaseChore : IChore
    {
        public ChoreData ChoreData { get; }

        protected BaseChore(ChoreData choreData)
        {
            ChoreData = choreData;
        }

        public abstract bool CanDoIt();
        public abstract bool DoIt();

        public virtual IDictionary<string, string> GetTokens()
        {
            var spouse = Game1.player.getSpouse();

            return new Dictionary<string, string>()
            {
                {"playerName", Game1.player.Name},
                {"nickName", spouse?.getTermOfSpousalEndearment()},
                {"spouseName", spouse?.getName() },
                {"spouseGender", spouse?.Gender == 1 ? "Female" : "Male" },
                {"petName", Game1.player.getPetName()},
                {"hasChild", Game1.player.getChildrenCount() > 0 ? "Yes" : "No" },
                {
                    "childName", Game1.player.getChildrenCount() > 0
                        ? Game1.player.getChildren().Shuffle().First().getName()
                        : ""
                }
            };
        }
    }
}