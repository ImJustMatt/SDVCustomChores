using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class GiveAGift : BaseCustomChore
    {
        private NPC _todayBirthdayNpc;
        private readonly string _giftType;
        private readonly bool _enableUniversal;
        private readonly double _chanceForLove;
        private string _itemName;
        private int _itemId;

        public GiveAGift(string choreName, IDictionary<string, string> config, IEnumerable<Translation> dialogue) : base(choreName, config, dialogue)
        {
            Config.TryGetValue("Type", out var giftType);
            Config.TryGetValue("EnableUniversal", out var enableUniversal);
            Config.TryGetValue("ChanceForLove", out var chanceForLove);

            _giftType = !string.IsNullOrWhiteSpace(giftType) ? giftType : "Birthday";
            _enableUniversal = !string.IsNullOrWhiteSpace(enableUniversal) && Convert.ToBoolean(enableUniversal);
            _chanceForLove = !string.IsNullOrWhiteSpace(chanceForLove) ? Convert.ToDouble(chanceForLove) : 0.1;
        }

        public override bool CanDoIt(NPC spouse)
        {
            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
                return true;
            _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            return _todayBirthdayNpc != null && !_todayBirthdayNpc.getName().Equals(spouse.getName());
        }

        public override bool DoIt(NPC spouse)
        {
            var r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 +
                               (int) Game1.player.UniqueMultiplayerID);
            
            var itemIds = new List<int>();

            Game1.NPCGiftTastes.TryGetValue(_todayBirthdayNpc.getName(), out var personalGifts);

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
            var objectsFromCats =
                from objectInfo in Game1.objectInformation.Select(objectInfo =>
                    new KeyValuePair<int, string[]>(objectInfo.Key, objectInfo.Value.Split('/')[3].Split(' ')))
                where objectInfo.Value.Length == 2 &&
                      itemCats.Contains(Convert.ToInt32(objectInfo.Value[1]))
                select objectInfo.Key;

            // Get objects by id
            var objectsFromIds =
                from objectInfo in Game1.objectInformation
                where itemIds.Contains(objectInfo.Key)
                select objectInfo.Key;

            // Get unique objects from both lists
            var objects =
                from objectInfo in Game1.objectInformation
                where objectsFromCats.Contains(objectInfo.Key) ||
                      objectsFromIds.Contains(objectInfo.Key)
                select objectInfo;

            // Get random object from list
            var index = r.Next(0, objects.Count());

            // Store item to give to player
            _itemId = objects.ElementAt(index).Key;
            _itemName = objects.ElementAt(index).Value.Split('/')[0];

            return true;
        }

        public override string GetDialogue(NPC spouse)
        {
            var spouseName = spouse.getName();
            var spouseGender = spouse.Gender == 1 ? "Female" : "Male";
            var otherName = _todayBirthdayNpc.getName();
            var otherGender = _todayBirthdayNpc.Gender == 1 ? "Her" : "Him";

            // Try to get individual spouse, individual birthday dialogue
            var dialogues =
                from dialogue in Dialogues
                where dialogue.Key.StartsWith($"{spouseName}.{ChoreName}.{otherName}",
                          StringComparison.CurrentCultureIgnoreCase)
                select dialogue;

            // Try to get gender spouse, individual birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"{spouseGender}.{ChoreName}.{otherName}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get default, individual birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"default.{ChoreName}.{otherName}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get individual spouse, gender birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"{spouseName}.{ChoreName}.{otherGender}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get gender spouse, gender birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"{spouseGender}.{ChoreName}.{otherGender}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get default, gender birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"default.{ChoreName}.{otherGender}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get individual spouse, non-specific birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"{spouseName}.{ChoreName}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get gender spouse, non-specific birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"{spouseGender}.{ChoreName}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Try to get default, non-specific birthday dialogue
            if (!dialogues.Any())
                dialogues =
                    from dialogue in Dialogues
                    where dialogue.Key.StartsWith($"default.{ChoreName}",
                        StringComparison.CurrentCultureIgnoreCase)
                    select dialogue;

            // Return default empty string
            if (!dialogues.Any())
                return $"[{_itemName}]";

            // Return random dialogue of all that meet criteria
            var rnd = new Random();
            var index = rnd.Next(0, dialogues.Count());

            return dialogues.ElementAt(index).Tokens(new
            {
                playerName = Game1.player.Name,
                nickName = Game1.player.getSpouse().getTermOfSpousalEndearment(),
                petName = Game1.player.getPetName(),
                itemName = _itemName,
                itemId = _itemId,
                npcName = _todayBirthdayNpc.Name
            });
        }
    }
}