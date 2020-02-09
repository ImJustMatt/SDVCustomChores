using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace LeFauxMatt.CustomChores.Models
{
    public class ChoreData
    {
        public string ChoreName { get; }
        public IDictionary<string, object> Config { get; }
        public IEnumerable<TranslationData> Translations { get; }
        public Texture2D Image { get; }

        public ChoreData(string choreName, IDictionary<string, object> config, IEnumerable<TranslationData> translations, Texture2D image)
        {
            ChoreName = choreName;
            Config = config;
            Translations = translations;
            Image = image;
        }

        public void ClearTranslationCache()
        {
            foreach (var translation in Translations)
            {
                translation.ClearCache();
            }
        }
    }
}
