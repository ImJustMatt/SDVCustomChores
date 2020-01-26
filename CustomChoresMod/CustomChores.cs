using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using LeFauxMatt.CustomChores.Framework;
using LeFauxMatt.CustomChores.Framework.Chores;
using LeFauxMatt.CustomChores.Models;

namespace LeFauxMatt.CustomChores
{
    /// <summary>The mod entry point.</summary>
    public class CustomChores : Mod
    {
        /*********
        ** Fields
        *********/

        /// <summary>The mod configuration.</summary>
        private ModConfig _config;

        /// <summary>The custom chores.</summary>
        private readonly IDictionary<string, ICustomChore> _chores = new Dictionary<string, ICustomChore>();

        /// <summary>The dialogue that the spouses will say in the morning depending on what chore(s) they were able to.</summary>
        private IEnumerable<Translation> _dialogues;

        /// <summary>The spouses that will be able to perform chores.</summary>
        private readonly IDictionary<string, IEnumerable<CustomChoreConfig>> _spouses = new Dictionary<string, IEnumerable<CustomChoreConfig>>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;

            helper.ConsoleCommands.Add("chore_ListAll", "List all available chores by name.", this.ListChores);
            helper.ConsoleCommands.Add("chore_DoIt",
                "Performs the custom chore on demand.\n\nUsage: chore_DoIt <value>\n- value: the chore name.",
                this.DoChore);
            helper.ConsoleCommands.Add("chore_CanDoIt",
                "Checks if the current conditions allows a custom chore to be done.\n\nUsage: chore_CanDoIt <value>\n- value: the chore name.",
                this.CanDoChore);

            _dialogues = helper.Translation.GetTranslations();

            foreach (var spouse in _config.Spouses)
            {
                try
                {
                    var chores = spouse.Value
                        .Split('\\')
                        .Select((t) => t.Split(' '))
                        .Select((p) => new CustomChoreConfig(p[0], Convert.ToDouble(p[1])));
                    _spouses.Add(spouse.Key, chores);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"An error occured while parsing the log entry for {spouse.Key}", LogLevel.Error);
                }
            }
        }

        public override object GetApi()
        {
            return new CustomChoresApi(Monitor, _chores);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Raised after the game is launched, right before the first update tick. This happens once per game session
        /// (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to
        /// set up mod integrations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create("furyx639.MarriageDutiesMod");

            // Patch to prevent default chores
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.NPC), nameof(StardewValley.NPC.marriageDuties)),
                prefix: new HarmonyMethod(typeof(NpcPatches), nameof(NpcPatches.MarriageDuties_Prefix))
            );

            // Load default chores
            foreach (var choreConfig in _config.Chores)
            {
                if (choreConfig.Key.StartsWith("FeedTheAnimals", StringComparison.CurrentCultureIgnoreCase))
                    TryAddChore<FeedTheAnimals>("FeedTheAnimals", choreConfig);
                else if (choreConfig.Key.StartsWith("FeedThePet", StringComparison.CurrentCultureIgnoreCase))
                    TryAddChore<FeedThePet>("FeedThePet", choreConfig);
                else if (choreConfig.Key.StartsWith("GiveAGift", StringComparison.CurrentCultureIgnoreCase))
                    TryAddChore<GiveAGift>("GiveAGift", choreConfig);
                else if (choreConfig.Key.StartsWith("PetTheAnimals", StringComparison.CurrentCultureIgnoreCase))
                    TryAddChore<PetTheAnimals>("PetTheAnimals", choreConfig);
                else if (choreConfig.Key.StartsWith("RepairTheFences", StringComparison.CurrentCultureIgnoreCase))
                    TryAddChore<RepairTheFences>("RepairTheFences", choreConfig);
                else if (choreConfig.Key.StartsWith("WaterTheCrops", StringComparison.CurrentCultureIgnoreCase))
                    TryAddChore<WaterTheCrops>("WaterTheCrops", choreConfig);
                else if (choreConfig.Key.StartsWith("WaterTheSlimes", StringComparison.CurrentCultureIgnoreCase))
                    TryAddChore<WaterTheSlimes>("WaterTheSlimes", choreConfig);
            }
        }

        private void TryAddChore<T>(string choreName, KeyValuePair<string, IDictionary<string, string>> choreConfig)
            where T : class, ICustomChore
        {
            var dialogues =
                from dialogue in _dialogues
                where dialogue.Key.IndexOf(choreName,
                          StringComparison.CurrentCultureIgnoreCase) >= 0
                select dialogue;
            _chores.Add(choreConfig.Key,
                Activator.CreateInstance(typeof(T), new object[] { choreConfig.Key, choreConfig.Value, dialogues }) as T);
        }

        /// <summary>
        /// Raised after the game begins a new day (including when the player loads a save).
        /// Here it updates the mail box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 +
                                  (int) Game1.player.UniqueMultiplayerID);

            if (Game1.timeOfDay != 600 || !Game1.player.isMarried() || r.NextDouble() >= _config.GlobalChance)
                return;

            // Get spouse
            var spouse = Game1.player.getSpouse();
            if (spouse == null)
                return;

            // Get spouse chores
            _spouses.TryGetValue(spouse.Name, out var spouseConfig);
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
                where r.NextDouble() < choreConfig.Chance
                select choreConfig.ChoreName;

            // Generate list of chore options for today
            var choreList = (
                from chore in _chores
                where spouseChores.Contains(chore.Key) &&
                      chore.Value.CanDoIt(spouse)
                select chore.Key).Shuffle();

            // Attempt to perform chores from options
            var choresDone = _config.DailyLimit;
            foreach (var choreName in choreList)
            {
                _chores.TryGetValue(choreName, out var chore);
                if (chore == null)
                    continue;

                Monitor.Log($"Attempting to perform chore {choreName}", LogLevel.Trace);
                try
                {
                    if (chore.DoIt(spouse))
                    {
                        var dialogueText = chore.GetDialogue(spouse).ToString();
                        if (!string.IsNullOrWhiteSpace(dialogueText))
                            spouse.setNewDialogue(dialogueText, true);
                        if (--choresDone <= 0)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to perform chore {choreName}:\n{ex}", LogLevel.Error);
                }
            }
        }

        /// <summary>Lists all chores by name when 'chore_ListAll' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ListChores(string command, string[] args)
        {
            foreach (var choreName in this._chores.Keys)
            {
                this.Monitor.Log($"- {choreName}.", LogLevel.Info);
            }
        }

        /// <summary>Performs a custom chore on demand when 'chore_DoIt' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void DoChore(string command, string[] args)
        {
            var spouse = Game1.player.getSpouse();
            this._chores.TryGetValue(args[0], out var chore);

            if (chore != null)
            {
                this.Monitor.Log($"Attempting to perform chore {args[0]}.", LogLevel.Info);
                if (chore.CanDoIt(spouse))
                    chore.DoIt(spouse);
            }
            else
            {
                this.Monitor.Log($"No chore found with name {args[0]}", LogLevel.Info);
            }
        }

        /// <summary>Returns current condition for custom chore when 'chore_CanDoIt' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void CanDoChore(string command, string[] args)
        {
            var spouse = Game1.player.getSpouse();
            this._chores.TryGetValue(args[0], out var chore);

            if (chore != null)
                this.Monitor.Log((chore.CanDoIt(spouse) ? "Can" : "Cannot") + $" do custom chore {args[0]}.",
                    LogLevel.Info);
            else
                this.Monitor.Log($"No chore found with name {args[0]}", LogLevel.Info);
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