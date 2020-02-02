using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Harmony;
using LeFauxMatt.CustomChores;
using LeFauxMatt.HelpfulSpouses.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LeFauxMatt.HelpfulSpouses
{
    public class HelpfulSpouses : Mod
    {
        /*********
        ** Fields
        *********/
        private ICustomChoresApi _customChoresApi;

        /// <summary>The mod configuration.</summary>
        private ModConfig _config;

        /// <summary>A list of chores and enumerable dialogue.</summary>
        private readonly IDictionary<string, ChoreHandler> _chores = new Dictionary<string, ChoreHandler>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            _config = Helper.ReadConfig<ModConfig>();

            // add console commands
            helper.ConsoleCommands.Add("spouse_AddChore", "Makes spouse perform a chore.\n\nUsage: spouse_AddChore <value>\n- value: chore by name.", AddChore);

            // hook events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Does chore and adds dialogue to your spouse when the 'spouse_addChore' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void AddChore(string command, string[] args)
        {
            // Get Spouse
            var spouse = Game1.player.getSpouse();
            if (spouse == null)
                return;

            // Get Chore
            _chores.TryGetValue(args[0], out var chore);
            if (chore is null)
            {
                Monitor.Log($"A chore of name {args[0]} does not exist.", LogLevel.Alert);
                return;
            }

            // Check Chore
            if (!_customChoresApi.CheckChore(chore.ChoreName))
            {
                Monitor.Log($"Cannot perform chore {args[0]}", LogLevel.Alert);
                return;
            }

            // Try Chore
            if (!_customChoresApi.DoChore(chore.ChoreName))
            {
                Monitor.Log($"Failed to perform chore {args[0]}", LogLevel.Alert);
                return;
            }

            // Add Dialogue
            var tokens = _customChoresApi.GetChoreTokens(chore.ChoreName);
                    
            var dialogue =
                from translation in chore.Translations
                where translation.Filter(tokens)
                select translation;

            var dialogueText = dialogue.First().Translation.Tokens(tokens).ToString();

            if (!string.IsNullOrWhiteSpace(dialogueText))
                spouse.setNewDialogue(dialogueText, true);

            Monitor.Log($"Successfully performed chore {args[0]}", LogLevel.Info);
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

            // harmony patches to prevent default chores
            var harmony = HarmonyInstance.Create("furyx639.MarriageDutiesMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.marriageDuties)),
                prefix: new HarmonyMethod(typeof(NpcPatches), nameof(NpcPatches.MarriageDuties_Prefix))
            );
        }

        /// <summary>The event called after a save slot is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // get chores
            foreach (var choreKey in _customChoresApi.GetChores())
            {
                var chore = _customChoresApi.GetChore(choreKey);
                _chores.Add(choreKey, new ChoreHandler(chore));
            }
        }

        /// <summary>The method invoked when a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var r = new Random((int)Game1.stats.DaysPlayed +
                               (int)Game1.uniqueIDForThisGame / 2 +
                               (int)Game1.player.UniqueMultiplayerID);

            if (Game1.timeOfDay != 600 || !Game1.player.isMarried() || r.NextDouble() >= _config.GlobalChance)
                return;

            // Get spouse
            var spouse = Game1.player.getSpouse();
            if (spouse == null)
                return;

            // Get spouse chores
            _config.Spouses.TryGetValue(spouse.Name, out var spouseConfig);
            if (spouseConfig == null || !spouseConfig.Any())
                return;

            // Get spouse hearts
            Game1.player.friendshipData.TryGetValue(spouse.Name, out var friendship);
            var spouseHearts = friendship.Points / NPC.friendshipPointsPerHeartLevel;
            if (spouseHearts < _config.HeartsNeeded && _config.HeartsNeeded > 0)
                return;

            // List of spouse chores based on random chance
            var spouseChores =
                from choreConfig in spouseConfig
                where r.NextDouble() < choreConfig.Value
                select choreConfig.Key;

            // Generate list of chore options for today
            var choreList = (
                from chore in _chores
                where spouseChores.Contains(chore.Key)
                select chore).Shuffle();

            // Attempt to perform chores from options
            var choresDone = _config.DailyLimit;
            foreach (var choreHandler in choreList)
            {
                var chore = choreHandler.Value;
                if (!_customChoresApi.CheckChore(chore.ChoreName))
                    continue;
                
                try
                {
                    if (!_customChoresApi.DoChore(chore.ChoreName))
                        continue;

                    Monitor.Log($"Successfully performed chore {choreHandler.Key}", LogLevel.Trace);

                    var tokens = _customChoresApi.GetChoreTokens(chore.ChoreName);
                    
                    var dialogue =
                        from translation in chore.Translations
                        where translation.Filter(tokens)
                        select translation;

                    var dialogueText = dialogue.First().Translation.Tokens(tokens).ToString();

                    if (!string.IsNullOrWhiteSpace(dialogueText))
                        spouse.setNewDialogue(dialogueText, true);
                    if (--choresDone <= 0)
                        break;
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to perform chore {choreHandler.Key}:\n{ex}", LogLevel.Error);
                }
            }
        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (rng == null)
                throw new ArgumentNullException(nameof(rng));
            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                var j = rng.Next(i, buffer.Count);
                yield return buffer[j];
                buffer[j] = buffer[i];
            }
        }
    }
}
