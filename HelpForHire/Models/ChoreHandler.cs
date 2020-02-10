using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LeFauxMatt.HelpForHire.Models
{
    internal class ChoreHandler
    {
        internal bool IsPurchased { get; set; }
        internal int Price { get; }
        public string ChoreName => Chore.ChoreName;
        public string DisplayName => _displayName.Tokens(_customChoresApi.GetChoreTokens(ChoreName));
        public string Description => _description.Tokens(_customChoresApi.GetChoreTokens(ChoreName));
        public int EstimatedCost => ModConfig.Instance.PayPerUnit ? Price * WorkNeeded : Price;
        public int ActualCost => ModConfig.Instance.PayPerUnit ? Price * WorkDone : Price;
        public int WorkNeeded => Convert.ToInt32(_workNeeded.Invoke());
        public int WorkDone => Convert.ToInt32(_workDone.Invoke());
        public int ImageWidth => Chore.Image.Width;
        public int ImageHeight => Chore.Image.Height;
        private ChoreData Chore { get; }
        private readonly ICustomChoresApi _customChoresApi;
        private readonly TranslationData _displayName;
        private readonly TranslationData _description;
        private readonly IDictionary<string, Func<string>> _choreTokens;
        private readonly Func<string> _workNeeded;
        private readonly Func<string> _workDone;

        internal ChoreHandler(ChoreData chore, int price, ICustomChoresApi customChoresApi)
        {
            Chore = chore;
            Price = price;
            _customChoresApi = customChoresApi;

            // get display name
            _choreTokens = customChoresApi.GetChoreTokens(chore.ChoreName);
            _choreTokens.Add("Mod", () => "HelpForHire");

            _displayName = (
                from translation in chore.Translations
                where translation.Key.Equals("DisplayName", StringComparison.CurrentCultureIgnoreCase)
                      && translation.Filter(_choreTokens)
                select translation).First();

            // get description
            _description = (
                from translation in chore.Translations
                where translation.Key.Equals("Description", StringComparison.CurrentCultureIgnoreCase)
                      && translation.Filter(_choreTokens)
                select translation).First();

            // get work needed token
            if (_choreTokens.TryGetValue("WorkNeeded", out var workNeededFn))
                _workNeeded = () => workNeededFn?.Invoke() ?? "0";

            // get work done token
            if (_choreTokens.TryGetValue("WorkDone", out var workDoneFn))
                _workDone = () => workDoneFn?.Invoke() ?? "0";
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