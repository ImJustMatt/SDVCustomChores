using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IDictionary<string, IList<CustomChoreConfig>> _spouses = new Dictionary<string, IList<CustomChoreConfig>>();

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

            if (!helper.Translation.GetTranslations().Any())
                _config.EnableDialogue = false;

            foreach (var spouse in _config.Spouses)
            {
                try
                {
                    var chores = spouse.Value
                        .Split('\\')
                        .Select((t) => t.Split(' '))
                        .Select((t) => new CustomChoreConfig(t[0], Convert.ToDouble(t[1])))
                        .ToList();
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

        internal Translation GetDialogue(string name, string chore)
        {
            // Try to get individual dialogue
            var dialogues =
                _dialogues.Where(dialogue => dialogue.Key.StartsWith($"{name}.{chore}", StringComparison.CurrentCultureIgnoreCase)).ToList();

            // Try to get global dialogue
            if (dialogues.Count == 0)
                dialogues = _dialogues.Where(dialogue => dialogue.Key.StartsWith($"default.{chore}", StringComparison.CurrentCultureIgnoreCase)).ToList();

            // Return null string
            if (dialogues.Count == 0)
                return (Translation) null;

            // Return random dialogue of all that meet criteria
            var rnd = new Random();
            var index = rnd.Next(dialogues.Count);

            return dialogues[index].Tokens(new
            {
                playerName = Game1.player.Name,
                nickName = Game1.player.getSpouse().getTermOfSpousalEndearment(),
                petName = Game1.player.getPetName()
            });
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
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
            TryAddChore<FeedTheAnimals>("FeedTheAnimals");
            TryAddChore<FeedThePet>("FeedThePet");
            TryAddChore<PetTheAnimals>("PetTheAnimals");
            TryAddChore<RepairTheFences>("RepairTheFences");
            TryAddChore<WaterTheCrops>("WaterTheCrops");
            TryAddChore<WaterTheSlimes>("WaterTheSlimes");
        }

        private void TryAddChore<T>(string name) where T: class, ICustomChore
        {
            _config.Chores.TryGetValue(name, out var config);
            _chores.Add(name, Activator.CreateInstance(typeof(T), new object[] { this, config }) as T);
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
            _spouses.TryGetValue(spouse.Name, out var chores);
            if (chores == null)
                return;

            // Get spouse hearts
            Game1.player.friendshipData.TryGetValue(spouse.Name, out var friendship);
            var spouseHearts = friendship.Points / NPC.friendshipPointsPerHeartLevel;
            if (spouseHearts < _config.HeartsNeeded && _config.HeartsNeeded > 0)
                return;

            var choresDone = _config.DailyLimit;
            var npcDialogue = new List<string>();

            foreach (var choreConfig in chores)
            {
                if (r.NextDouble() >= choreConfig.Chance)
                    continue;

                _chores.TryGetValue(choreConfig.ChoreName, out var chore);
                if (chore == null)
                    continue;
                if (!chore.CanDoIt())
                    continue;

                Monitor.Log($"Attempting to perform chore {choreConfig.ChoreName}:\n", LogLevel.Trace);

                bool didIt;
                try
                {
                    didIt = chore.DoIt();
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to perform chore {choreConfig.ChoreName}:\n{ex}", LogLevel.Error);
                    didIt = false;
                }

                if (!didIt)
                    continue;

                if (_config.EnableDialogue)
                {
                    var dialogueText = chore.GetDialogue(spouse.Name);
                    if (!string.IsNullOrWhiteSpace(dialogueText))
                        spouse.setNewDialogue(dialogueText, true);
                }

                if (--choresDone <= 0)
                    break;
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
            this._chores.TryGetValue(args[0], out var chore);

            if (chore != null)
            {
                this.Monitor.Log($"Attempting to perform chore {args[0]}.", LogLevel.Info);
                if (chore.CanDoIt())
                    chore.DoIt();
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
            this._chores.TryGetValue(args[0], out var chore);

            if (chore != null)
                this.Monitor.Log((chore.CanDoIt() ? "Can" : "Cannot") + $" do custom chore {args[0]}.",
                    LogLevel.Info);
            else
                this.Monitor.Log($"No chore found with name {args[0]}", LogLevel.Info);
        }
    }
}