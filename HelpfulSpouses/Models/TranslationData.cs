using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace LeFauxMatt.HelpfulSpouses.Models
{
    internal class TranslationData
    {
        protected internal string Key { get; }
        protected internal Translation Translation { get; }
        protected internal IDictionary<string, string> Selectors { get; } = new Dictionary<string, string>();

        public TranslationData(Translation translation)
        {
            Translation = translation;

            var parts = translation.Key.Split('[');

            // get key
            Key = parts.First().Trim();

            // get selector
            foreach (var part in parts.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;
                if (!part.EndsWith("]"))
                    continue;

                var subParts = part.TrimEnd(']').Split('=');
                if (subParts.Length != 2)
                    continue;

                Selectors.Add(subParts[0].Trim(), subParts[1].Trim());
            }
        }

        public bool Filter(IDictionary<string, string> tokens)
        {
            foreach (var selector in Selectors)
            {
                tokens.TryGetValue(selector.Key, out var tokenValue);
                if (tokenValue is null || !selector.Value.Equals(tokenValue, StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}
