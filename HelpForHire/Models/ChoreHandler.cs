using System;
using System.Linq;
using LeFauxMatt.CustomChores;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LeFauxMatt.HelpForHire.Models
{
    internal class ChoreHandler
    {
        private ChoreData Chore { get; }
        private readonly ICustomChoresApi _customChoresApi;
        private readonly TranslationData _displayName;
        private readonly TranslationData _description;
        internal bool IsPurchased { get; set; }
        internal int Price { get; }
        public string ChoreName => Chore.ChoreName;
        public string DisplayName => _displayName.Tokens(_customChoresApi.GetChoreTokens(ChoreName));
        public string Description => _description.Tokens(_customChoresApi.GetChoreTokens(ChoreName));
        public int ImageWidth => Chore.Image.Width;
        public int ImageHeight => Chore.Image.Height;

        internal ChoreHandler(ChoreData chore, int price, ICustomChoresApi customChoresApi)
        {
            Chore = chore;
            Price = price;
            _customChoresApi = customChoresApi;

            // get display name
            var tokens = customChoresApi.GetChoreTokens(chore.ChoreName);
            tokens.Add("Mod", () => "HelpForHire");

            _displayName = (
                from translation in chore.Translations
                where translation.Key.Equals("DisplayName",
                          StringComparison.CurrentCultureIgnoreCase)
                      && translation.Filter(tokens)
                select translation).First();

            // get description
            _description = (
                from translation in chore.Translations
                where translation.Key.Equals("Description",
                    StringComparison.CurrentCultureIgnoreCase)
                select translation).First();
        }

        public virtual void DrawInMenu(SpriteBatch b, int x, int y)
        {
            b.Draw(Chore.Image,
                new Vector2(x - ImageWidth / 2, y - ImageHeight / 2),
                new Rectangle(0, 0, ImageWidth, ImageHeight),
                Color.White);
        }

        public void ClearTranslationCache()
        {
            Chore.ClearTranslationCache();
        }
    }
}