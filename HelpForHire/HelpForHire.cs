﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores;
using LeFauxMatt.HelpForHire.Menus;
using LeFauxMatt.HelpForHire.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LeFauxMatt.HelpForHire
{
    
    public class HelpForHire : Mod
    {
        /*********
        ** Fields
        *********/
        private ICustomChoresApi _customChoresApi;

        /// <summary>The mod configuration.</summary>
        private ModConfig _config;

        /// <summary>A list of chores with assets and config related to the shop menu.</summary>
        private readonly IDictionary<string, ChoreHandler> _chores = new Dictionary<string, ChoreHandler>();

        internal static string PurchasedLabel;
        internal static string NotPurchasedLabel;
        internal static string InsufficientFundsLabel;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // init 
            _config = helper.ReadConfig<ModConfig>();
            PurchasedLabel = helper.Translation.Get("label.purchased");
            NotPurchasedLabel = helper.Translation.Get("label.notPurchased");
            InsufficientFundsLabel = helper.Translation.Get("label.insufficientFunds");

            // add console commands
            helper.ConsoleCommands.Add("chores_OpenShop", "Opens the chores shop.\n\nUsage: chores_OpenShop", OpenShop);

            // hook events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Displays a shop that sells chore services when the 'chores_openShop' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void OpenShop(string command, string[] args)
        {
            if (_chores.Any() && Game1.activeClickableMenu is null)
                Game1.activeClickableMenu = new ChoreMenu(_customChoresApi, _chores);
        }

        /****
        ** Event handlers
        ****/
        /// <summary>The method invoked on the first update tick, once all mods are initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // init
            _customChoresApi = Helper.ModRegistry.GetApi<ICustomChoresApi>("furyx639.CustomChores");
        }
        
        /// <summary>The event called after a save slot is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _customChoresApi = Helper.ModRegistry.GetApi<ICustomChoresApi>("furyx639.CustomChores");

            // get chores
            var choreKeys =
                from choreKey in _customChoresApi.GetChores()
                where _config.Chores.ContainsKey(choreKey)
                select choreKey;

            foreach (var choreKey in choreKeys)
            {
                var chore = _customChoresApi.GetChore(choreKey);
                _config.Chores.TryGetValue(choreKey, out var price);
                _chores.Add(choreKey, new ChoreHandler(chore, price));
            }
        }

        /// <summary>The method invoked when a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            bool insufficientFunds = false;
            foreach (var choreHandler in _chores)
            {
                var chore = choreHandler.Value;

                if (!chore.IsPurchased || !_customChoresApi.CheckChore(chore.ChoreName))
                    continue;

                if (Game1.player.Money < chore.Price)
                {
                    insufficientFunds = true;
                    break;
                }

                try
                {
                    if (!_customChoresApi.DoChore(chore.ChoreName))
                        continue;

                    Game1.playSound("purchaseClick");
                    Game1.player.Money -= chore.Price;
                    Monitor.Log($"Successfully performed chore {choreHandler.Key}", LogLevel.Trace);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to perform chore {choreHandler.Key}:\n{ex}", LogLevel.Error);
                }
            }

            if (!insufficientFunds)
                return;

            // Insufficient Funds
            foreach (var choreHandler in _chores)
            {
                choreHandler.Value.IsPurchased = false;
            }

            Game1.addHUDMessage(new HUDMessage(InsufficientFundsLabel, 3));
            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree || !_chores.Any() || !(Game1.activeClickableMenu is null))
                return;

            var key = e.Button;
            if (key == _config.ShopMenuButton)
                Game1.activeClickableMenu = new ChoreMenu(_customChoresApi, _chores);
        }
    }
}