using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework
{
    internal abstract class BaseCustomChore : ICustomChore
    {
        internal readonly string ChoreName;
        protected IDictionary<string, string> Config;
        protected IEnumerable<Translation> Dialogues;

        protected BaseCustomChore(string choreName, IDictionary<string, string> config, IEnumerable<Translation> dialogue)
        {
            ChoreName = choreName;
            Config = config;
            Dialogues = dialogue;
        }

        public abstract bool CanDoIt(string name = null);
        public abstract bool DoIt(string name = null);

        public virtual string GetDialogue(NPC spouse)
        {
            var spouseGender = spouse.Gender == 1 ? "Female" : "Male";

            // Try to get individual dialogue
            var dialogues =
                from dialogue in Dialogues
                where dialogue.Key.StartsWith($"{spouse.getName()}.{ChoreName}",
                    StringComparison.CurrentCultureIgnoreCase)
                select dialogue;

            // Try to get gender dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"{spouseGender}.{ChoreName}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get global dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"default.{ChoreName}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Return null string
            if (!dialogues.Any())
                return (Translation)null;

            // Return random dialogue of all that meet criteria
            var rnd = new Random();
            var index = rnd.Next(0, dialogues.Count());

            return dialogues.ElementAt(index).Tokens(new
            {
                playerName = Game1.player.Name,
                nickName = Game1.player.getSpouse().getTermOfSpousalEndearment(),
                petName = Game1.player.getPetName()
            });
        }
    }
}