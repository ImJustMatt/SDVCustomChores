﻿using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores;
using LeFauxMatt.HelpForHire.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace LeFauxMatt.HelpForHire.Menus
{
    internal class ChoreMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        private ICustomChoresApi _customChoresApi;

        /// <summary>A list of chores with assets and config related to the shop menu.</summary>
        private readonly IDictionary<string, ChoreHandler> _chores;

        /// <summary>A list of chore keys sorted by their display name.</summary>
        private readonly IList<string> _choreKeys;
        private int _currentChoreIndex = 0;
        internal ChoreHandler CurrentChore => _chores[_choreKeys[_currentChoreIndex]];

        private static int MaxWidthOfImage { get; } = 384;
        private static int MaxHeightOfImage { get; } = 384;
        private static int MaxWidthOfDescription { get; } = 512;

        private ClickableTextureComponent _backButton;
        private ClickableTextureComponent _forwardButton;
        private ClickableTextureComponent _okButton;

        /*********
        ** Public methods
        *********/
        public ChoreMenu(ICustomChoresApi customChoresApi, IDictionary<string, ChoreHandler> chores)
        {
            _customChoresApi = customChoresApi;
            _chores = chores;

            // create ordered list
            _choreKeys = (
                from chore in chores
                orderby chore.Value.DisplayName.Tokens(_customChoresApi.GetChoreTokens(chore.Key)).ToString()
                select chore.Key).ToList();

            ResetBounds();
        }

        public override void performHoverAction(int x, int y)
        {
            _backButton.tryHover(x, y, 1f);
            _forwardButton.tryHover(x, y, 1f);
            _okButton.tryHover(x, y, 0.1f);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            ResetBounds();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (_backButton.containsPoint(x, y))
            {
                --_currentChoreIndex;
                if (_currentChoreIndex < 0)
                    _currentChoreIndex = _choreKeys.Count - 1;
                Game1.playSound("shwip");
                _backButton.scale = _backButton.baseScale;
            }

            if (_forwardButton.containsPoint(x, y))
            {
                _currentChoreIndex = (_currentChoreIndex + 1) % _choreKeys.Count;
                Game1.playSound("shwip");
                _forwardButton.scale = _forwardButton.baseScale;
            }

            if (_okButton.containsPoint(x, y))
            {
                CurrentChore.IsPurchased = !CurrentChore.IsPurchased;
                Game1.playSound(CurrentChore.IsPurchased ? "purchase" : "sell");
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            var tokens = _customChoresApi.GetChoreTokens(CurrentChore.ChoreName);
            
            // Chore Preview
            IClickableMenu.drawTextureBox(b,
                xPositionOnScreen,
                yPositionOnScreen,
                MaxWidthOfImage + spaceToClearSideBorder * 2,
                MaxHeightOfImage + spaceToClearSideBorder * 2,
                Color.White);

            CurrentChore.DrawInMenu(b,
                xPositionOnScreen + MaxWidthOfImage / 2 + spaceToClearSideBorder,
                yPositionOnScreen + MaxHeightOfImage / 2 + spaceToClearSideBorder);

            // Chore Display Name
            SpriteText.drawStringWithScrollCenteredAt(b,
                CurrentChore.DisplayName.Tokens(tokens),
                xPositionOnScreen + MaxWidthOfImage + MaxWidthOfDescription / 2 + 60,
                yPositionOnScreen,
                MaxWidthOfDescription - 64,
                1f, -1, 0, 0.88f, false);

            // Chore Description
            IClickableMenu.drawTextureBox(b,
                xPositionOnScreen + MaxWidthOfImage + spaceToClearSideBorder * 2 + 16,
                yPositionOnScreen + 80,
                MaxWidthOfDescription + spaceToClearSideBorder * 2,
                MaxHeightOfImage - 48,
                Color.White);

            Utility.drawTextWithShadow(b,
                Game1.parseText(CurrentChore.Description.Tokens(tokens), Game1.dialogueFont, MaxWidthOfDescription),
                Game1.dialogueFont,
                new Vector2(xPositionOnScreen + MaxWidthOfImage + spaceToClearSideBorder * 3 + 16, yPositionOnScreen + spaceToClearSideBorder + 80),
                Game1.textColor,
                1f, -1f, -1, -1, 0.75f, 3);

            // Chore Cost
            SpriteText.drawString(b,
                "$",
                xPositionOnScreen + MaxWidthOfImage + spaceToClearSideBorder * 3 + 32,
                yPositionOnScreen + MaxHeightOfImage - spaceToClearSideBorder - 16,
                999999, -1, 999999, 1f, 0.88f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);

            Utility.drawTextWithShadow(b,
                Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", CurrentChore.Price),
                Game1.dialogueFont,
                new Vector2(xPositionOnScreen + MaxWidthOfImage + spaceToClearSideBorder * 3 + 100,
                    yPositionOnScreen + MaxHeightOfImage - spaceToClearSideBorder - 16),
                Game1.player.Money >= CurrentChore.Price ? Game1.textColor : Color.Red,
                1f, -1f, -1, -1, 0.25f, 3);

            // Purchased Status
            Utility.drawTextWithShadow(b,
                CurrentChore.IsPurchased ? HelpForHire.PurchasedLabel : HelpForHire.NotPurchasedLabel,
                Game1.dialogueFont,
                new Vector2(xPositionOnScreen + MaxWidthOfImage + MaxWidthOfDescription / 2 + spaceToClearSideBorder * 3,
                    yPositionOnScreen + MaxHeightOfImage - spaceToClearSideBorder - 16),
                CurrentChore.IsPurchased ? Game1.textColor : Color.Red,
                1f, -1f, -1, -1, 0.25f, 3);

            _backButton.draw(b);
            _forwardButton.draw(b);
            _okButton.draw(b, !CurrentChore.IsPurchased ? Color.White : Color.Gray * 0.8f, 0.88f, 0);

            drawMouse(b);
        }

        /*********
        ** Private methods
        *********/
        private void ResetBounds()
        {
            xPositionOnScreen = Game1.viewport.Width / 2 - MaxWidthOfImage - spaceToClearSideBorder - 96;
            yPositionOnScreen = Game1.viewport.Height / 2 - MaxHeightOfImage / 2 - 64;
            width = MaxWidthOfImage + MaxWidthOfDescription + spaceToClearSideBorder * 2 + 96;
            height = MaxHeightOfImage + spaceToClearTopBorder + 64;

            initialize(
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                true);

            // Back Button
            _backButton = new ClickableTextureComponent(
                    new Rectangle(xPositionOnScreen + MaxWidthOfImage / 2 + spaceToClearSideBorder - 96, yPositionOnScreen + MaxHeightOfImage + spaceToClearSideBorder * 2 + 32, 48, 44),
                    Game1.mouseCursors,
                    new Rectangle(352, 495, 12, 11),
                    4f,
                    false)
                { myID = 101, rightNeighborID = 102 };

            // Forward Button
            _forwardButton = new ClickableTextureComponent(
                    new Rectangle(xPositionOnScreen + MaxWidthOfImage / 2 + spaceToClearSideBorder + 32, yPositionOnScreen + MaxHeightOfImage + spaceToClearSideBorder * 2 + 32, 48, 44),
                    Game1.mouseCursors,
                    new Rectangle(365, 495, 12, 11),
                    4f,
                    false)
                { myID = 102, leftNeighborID = 101 };

            // OK Button
            _okButton = new ClickableTextureComponent(
                    new Rectangle(xPositionOnScreen + MaxWidthOfImage + MaxWidthOfDescription + spaceToClearSideBorder * 4 - 48, yPositionOnScreen + MaxHeightOfImage + spaceToClearSideBorder * 2 + 32, 64, 64),
                    Game1.mouseCursors,
                    new Rectangle(280, 411, 16, 16),
                    4f,
                    false)
                { myID = 106 };
        }
    }
}
