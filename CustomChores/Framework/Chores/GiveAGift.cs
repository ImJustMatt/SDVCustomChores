using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using SObject = StardewValley.Object;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class GiveAGift : BaseCustomChore
    {
        public override string ChoreName { get; } = "GiveAGift";

        private NPC _todayBirthdayNpc;
        private readonly bool _enableUniversal;
        private readonly double _chanceForLove;
        private string _itemName;
        private int _itemId;

        public GiveAGift(CustomChores instance, IDictionary<string, string> config) : base(instance, config)
        {
            Config.TryGetValue("EnableUniversal", out var enableUniversal);
            Config.TryGetValue("ChanceForLove", out var chanceForLove);

            _enableUniversal = (enableUniversal != null) && Convert.ToBoolean(enableUniversal);
            _chanceForLove = (chanceForLove != null) ? Convert.ToDouble(chanceForLove) : 0.1;
        }

        public override bool CanDoIt(string name = null)
        {
            _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            return _todayBirthdayNpc != null && !_todayBirthdayNpc.Name.Equals(name);
        }

        public override bool DoIt(string name = null)
        {
            var r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 +
                               (int) Game1.player.UniqueMultiplayerID);
            
            var itemIds = new List<int>();

            Game1.NPCGiftTastes.TryGetValue(_todayBirthdayNpc.Name, out var personalGifts);

            var personalData = personalGifts?.Split('/');

            if (personalData == null)
                return false;

            if (r.NextDouble() < _chanceForLove)
            {
                itemIds.AddRange(personalData[1].Split(' ').Select(s => Convert.ToInt32(s)));
                if (_enableUniversal)
                {
                    Game1.NPCGiftTastes.TryGetValue("Universal_Love", out var universalLoveGifts);
                    if (universalLoveGifts != null)
                        itemIds.AddRange(universalLoveGifts.Split(' ').Select(s => Convert.ToInt32(s)));
                }
            }
            else
            {
                itemIds.AddRange(personalData[3].Split(' ').Select(s => Convert.ToInt32(s)));
                if (_enableUniversal)
                {
                    Game1.NPCGiftTastes.TryGetValue("Universal_Like", out var universalLoveGifts);
                    if (universalLoveGifts != null)
                        itemIds.AddRange(universalLoveGifts.Split(' ').Select(s => Convert.ToInt32(s)));
                }
            }

            var itemCats = itemIds.Where(itemId => itemId < 0);

            // Get objects by category
            var objectsFromCats = Game1.objectInformation
                .Where(objectInfo =>
                    itemCats.Contains(Convert.ToInt32(objectInfo.Value.Split('/')[3].Split(' ')[1])));

            // Get objects by id
            var objectsFromIds = Game1.objectInformation
                .Where(objectInfo => itemIds.Contains(Convert.ToInt32(objectInfo.Key)));

            // Get unique objects from both lists
            var objects = objectsFromIds
                .Concat(objectsFromCats
                    .Where(objectInfo =>
                        !itemIds.Contains(Convert.ToInt32(objectInfo.Key))))
                .ToList();

            // Get random object from list
            var index = r.Next(objects.Count);

            // Store item to give to player
            _itemName = objects[index].Value.Split('/')[0];
            _itemId = objects[index].Key;

            return true;
        }

        public override string GetDialogue(string name)
        {
            // Try to get dialogue for character - unique, gender, then any
            var dialogues =
                ModInstance.GetDialogue(name, ChoreName + "." + _todayBirthdayNpc.Name)
                ?? ModInstance.GetDialogue(name, ChoreName + "." + (_todayBirthdayNpc.Gender == 1 ? "Him" : "Her"))
                ?? ModInstance.GetDialogue(name, ChoreName);

            return dialogues.Tokens(new 
            {
                itemName = _itemName,
                itemId = _itemId,
                npcName = _todayBirthdayNpc.Name
            });
        }
    }
}