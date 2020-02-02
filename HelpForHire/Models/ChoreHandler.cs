using System;
using System.Linq;
using LeFauxMatt.CustomChores;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace LeFauxMatt.HelpForHire.Models
{
    internal class ChoreHandler
    {
        private ChoreData Chore { get; }
        internal Translation DisplayName { get; }
        internal Translation Description { get; }
        internal bool IsPurchased { get; set; }
        internal int Price { get; }
        public string ChoreName => Chore.ChoreName;
        public int ImageWidth => Chore.Image.Width;
        public int ImageHeight => Chore.Image.Height;

        internal ChoreHandler(ChoreData chore, int price)
        {
            Chore = chore;
            Price = price;

            // get display name
            DisplayName = (
                from translation in chore.Translations
                where translation.Key.Equals("HelpForHire.DisplayName",
                    StringComparison.CurrentCultureIgnoreCase)
                select translation).First();

            // get description
            Description = (
                from translation in chore.Translations
                where translation.Key.Equals("HelpForHire.Description",
                    StringComparison.CurrentCultureIgnoreCase)
                select translation).First();
        }

        public virtual void DrawInMenu(SpriteBatch b, int x, int y)
        {
            b.Draw(Chore.Image,
                new Vector2(x - Chore.Image.Width / 2, y - Chore.Image.Height / 2),
                new Rectangle(0, 0, Chore.Image.Width, Chore.Image.Height),
                Color.White);
        }
    }
}