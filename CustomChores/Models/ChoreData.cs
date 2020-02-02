using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace LeFauxMatt.CustomChores.Models
{
    public class ChoreData
    {
        public readonly string ChoreName;
        public readonly IDictionary<string, object> Config;
        public readonly IEnumerable<Translation> Translations;
        public readonly Texture2D Image;

        public ChoreData(string choreName, IDictionary<string, object> config, IEnumerable<Translation> translations, Texture2D image)
        {
            ChoreName = choreName;
            Config = config;
            Translations = translations;
            Image = image;
        }
    }
}
