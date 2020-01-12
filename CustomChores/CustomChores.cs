using System;
using System.Collections.Generic;
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
        public static IModHelper _helper;

        /// <summary>Custom Chores API.</summary>
        private CustomChoresAPI API;

        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>The custom chores.</summary>
        private readonly IDictionary<string, ICustomChore> Chores = new Dictionary<string, ICustomChore>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _helper = helper;

            this.Config = Helper.ReadConfig<ModConfig>();
            this.API = new CustomChoresAPI(this.Monitor, this.Chores);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;

            helper.ConsoleCommands.Add("chore_listall", "List all available chores by name.", this.ListChores);
            helper.ConsoleCommands.Add("chore_doit", "Performs the custom chore on demand.\n\nUsage: chore_doit <value>\n- value: the chore name.", this.DoChore);
            helper.ConsoleCommands.Add("chore_candoit", "Checks if the current conditions allows a custom chore to be done.\n\nUsage: chore_candoit <value>\n- value: the chore name.", this.CanDoChore);
        }

        public override object GetApi()
        {
            return new CustomChoresAPI(this.Monitor, this.Chores);
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
                prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.marriageDuties_Prefix))
            );

            // Load default chores
            this.API.AddCustomChore(new FeedTheAnimals());
            this.API.AddCustomChore(new FeedThePet());
            this.API.AddCustomChore(new PetTheAnimals());
            this.API.AddCustomChore(new RepairTheFences());
            this.API.AddCustomChore(new WaterTheCrops());
            this.API.AddCustomChore(new WaterTheSlimes());
        }

        /// <summary>
        /// Raised after the game begins a new day (including when the player loads a save).
        /// Here it updates the mail box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Random r = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.player.UniqueMultiplayerID);

            if (Game1.timeOfDay != 600 || !Game1.player.isMarried() || r.NextDouble() >= Config.GlobalChance)
                return;

            // Get spouse
            NPC spouse = Game1.player.getSpouse();
            if (spouse == null)
                return;

            // Get spouse chores
            Config.Spouses.TryGetValue(spouse.Name, out IList<CustomChoreConfig> chores);
            if (chores == null)
                return;

            // Get spouse hearts
            Game1.player.friendshipData.TryGetValue(spouse.Name, out Friendship friendship);
            int spouseHearts = friendship.Points / NPC.friendshipPointsPerHeartLevel;
            if (spouseHearts < Config.HeartsNeeded && Config.HeartsNeeded > 0)
                return;

            int choresDone = Config.DailyLimit;
            bool didIt;
            List<string> npcDialogue = new List<string>();
            string dialogue;

            foreach (CustomChoreConfig choreConfig in chores)
            {
                if (r.NextDouble() < choreConfig.Chance)
                {
                    Chores.TryGetValue(choreConfig.ChoreName, out ICustomChore chore);
                    if (chore == null)
                        continue;
                    if (!chore.CanDoIt())
                        continue;

                    Monitor.Log($"Attempting to perform chore {choreConfig.ChoreName}:\n", LogLevel.Trace);

                    try
                    {
                        didIt = chore.DoIt();
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Failed to perform chore {choreConfig.ChoreName}:\n{ex}", LogLevel.Error);
                        didIt = false;
                    }

                    if (didIt)
                    {
                        --choresDone;
                        if (Config.EnableDialogue)
                        {
                            dialogue = chore.GetDialogue(spouse.Name);
                            if (dialogue != null && dialogue != "")
                                npcDialogue.Add(dialogue);
                        }
                    }

                    if (choresDone <= 0)
                        break;
                }
            }

            if (npcDialogue.Count > 0)
            {
                int index = r.Next(npcDialogue.Count);
                Game1.drawDialogue(spouse, npcDialogue[index]);
            }
        }

        /// <summary>Lists all chores by name when 'chore_listall' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ListChores(string command, string[] args)
        {
            foreach (string choreName in this.Chores.Keys)
            {
                this.Monitor.Log($"- {choreName}.", LogLevel.Info);
            }
        }

        /// <summary>Performs a custom chore on demand when 'chore_doit' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void DoChore(string command, string[] args)
        {
            ICustomChore chore = null;
            string choreName = args[0];
            this.Chores.TryGetValue(choreName, out chore);

            if (chore != null)
            {
                this.Monitor.Log($"Attempting to perform chore {choreName}.", LogLevel.Info);
                if (chore.CanDoIt())
                    chore.DoIt();
            }
            else
            {
                this.Monitor.Log($"No chore found with name {choreName}", LogLevel.Info);
            }
        }

        /// <summary>Returns current condition for custom chore when 'chore_candoit' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void CanDoChore(string command, string[] args)
        {
            ICustomChore chore = null;
            string choreName = args[0];
            this.Chores.TryGetValue(choreName, out chore);

            if (chore != null)
            {
                if (chore.CanDoIt())
                    this.Monitor.Log($"Can do custom chore {choreName}.", LogLevel.Info);
                else
                    this.Monitor.Log($"Cannot do custom chore {choreName}.", LogLevel.Info);
            }
            else
            {
                this.Monitor.Log($"No chore found with name {choreName}", LogLevel.Info);
            }
        }
    }
}