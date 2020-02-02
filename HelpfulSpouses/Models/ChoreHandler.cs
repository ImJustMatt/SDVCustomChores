using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;

namespace LeFauxMatt.HelpfulSpouses.Models
{
    internal class ChoreHandler
    {
        private ChoreData Chore { get; }
        public string ChoreName => Chore.ChoreName;
        internal IList<TranslationData> Translations { get; } = new List<TranslationData>();

        internal ChoreHandler(ChoreData chore)
        {
            Chore = chore;

            // add dialogues
            var translations =
                from translation in chore.Translations
                where translation.Key.StartsWith("HelpfulSpouses",
                    StringComparison.CurrentCultureIgnoreCase)
                select translation;
            
            foreach (var translation in translations)
            {
                Translations.Add(new TranslationData(translation));
            }
        }
    }
}
