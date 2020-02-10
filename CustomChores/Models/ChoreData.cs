using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace LeFauxMatt.CustomChores.Models
{
    public class ChoreData
    {
        public readonly string ChoreName;
        public readonly IDictionary<string, object> Config;
        public readonly IEnumerable<TranslationData> Translations;
        public readonly Texture2D Image;

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
