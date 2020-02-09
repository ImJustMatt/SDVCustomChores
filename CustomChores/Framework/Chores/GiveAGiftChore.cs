using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class GiveAGiftChore : BaseChore
    {
        private NPC _todayBirthdayNpc;
        private readonly string _giftType;
        private readonly int _maxGifts;
        private readonly bool _enableUniversal;
        private readonly double _chanceForLove;
        private readonly IEnumerable<int> _universalLoves;
        private readonly IEnumerable<int> _universalLikes;
        private IDictionary<int, string> _items;
        private int _giftsGiven;

        public GiveAGiftChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("GiftType", out var giftType);
            ChoreData.Config.TryGetValue("MaxGifts", out var maxGifts);
            ChoreData.Config.TryGetValue("EnableUniversal", out var enableUniversal);
            ChoreData.Config.TryGetValue("ChanceForLove", out var chanceForLove);

            _giftType = giftType is string s && !string.IsNullOrWhiteSpace(s) ? s : "Birthday";
            _maxGifts = maxGifts is int n ? n : 1;

            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
                return;

            _enableUniversal = enableUniversal is bool b && b;
            _chanceForLove = chanceForLove is double d ? d : 0.1;

            Game1.NPCGiftTastes.TryGetValue("Universal_Love", out var universalLoves);
            if (!string.IsNullOrWhiteSpace(universalLoves))
                _universalLoves =
                    from a in universalLoves.Split(' ')
                    select Convert.ToInt32(a, CultureInfo.InvariantCulture);

            Game1.NPCGiftTastes.TryGetValue("Universal_Like", out var universalLikes);
            if (universalLikes != null)
                _universalLikes =
                    from a in universalLikes.Split(' ')
                    select Convert.ToInt32(a, CultureInfo.InvariantCulture);
        }

        public override bool CanDoIt(bool today = true)
        {
            _items = null;
            _giftsGiven = _maxGifts > 1 ? Game1.random.Next(1, _maxGifts) : 1;

            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (today)
                _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            else if (Game1.dayOfMonth < 28)
                _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth + 1);
            else
            {
                string nextSeason;
                switch (Game1.currentSeason)
                {
                    case "spring":
                        nextSeason = "summer";
                        break;
                    case "summer":
                        nextSeason = "fall";
                        break;
                    case "fall":
                        nextSeason = "winter";
                        break;
                    case "winter":
                        nextSeason = "spring";
                        break;
                    default:
                        nextSeason = Game1.currentSeason;
                        break;
                }
                _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(nextSeason, 1);
            }

            return _todayBirthdayNpc != null && !_todayBirthdayNpc.getName().Equals(Game1.player.getSpouse().getName(), StringComparison.CurrentCultureIgnoreCase);
        }

        public override bool DoIt()
        {
            var r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 +
                               (int) Game1.player.UniqueMultiplayerID);
            
            var itemIds = new List<int>();

            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
            { 
                itemIds.AddRange(
                    from itemId in _giftType.Split(' ')
                    select Convert.ToInt32(itemId, CultureInfo.InvariantCulture));
            }
            else
            {
                Game1.NPCGiftTastes.TryGetValue(_todayBirthdayNpc.getName(), out var personalGifts);

                var personalData = personalGifts?.Split('/');

                if (personalData == null)
                    return false;

                if (r.NextDouble() < _chanceForLove)
                {
                    itemIds.AddRange(
                        from s in personalData[1].Split(' ')
                        select Convert.ToInt32(s, CultureInfo.InvariantCulture));
                    if (_enableUniversal)
                        itemIds.AddRange(_universalLoves);
                }
                else
                {
                    itemIds.AddRange(
                        from s in personalData[3].Split(' ')
                        select Convert.ToInt32(s, CultureInfo.InvariantCulture));
                    if (_enableUniversal)
                        itemIds.AddRange(_universalLikes);
                }
            }

            var itemCats = itemIds.Where(itemId => itemId < 0);

            // Get objects by category
            var objectsFromCats =
                from objectInfo in Game1.objectInformation.Select(objectInfo =>
                    new KeyValuePair<int, string[]>(objectInfo.Key, objectInfo.Value.Split('/')[3].Split(' ')))
                where objectInfo.Value.Length == 2 &&
                      itemCats.Contains(Convert.ToInt32(objectInfo.Value[1], CultureInfo.InvariantCulture))
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

            // Store items to give to player
            _items = objects.Shuffle().Take(_giftsGiven)
                .ToDictionary(
                    item => item.Key,
                    item => item.Value.Split('/')[0]);

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("ItemName", GetItemName);
            tokens.Add("ItemId", GetItemId);
            tokens.Add("Birthday", GetBirthdayName);
            tokens.Add("BirthdayGender", GetBirthdayGender);
            tokens.Add("GiftsGiven", GetGiftsGiven);
            tokens.Add("WorkDone", GetGiftsGiven);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        public string GetItemName() =>
            string.Join(", ", _items.Values);

        public string GetItemId() =>
            "[" + string.Join("][", _items.Keys) + "]";

        public static string GetBirthdayName() =>
            Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth)?.getName();

        public static string GetBirthdayGender() =>
            Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth)?.Gender == 1 ? "Female" : "Male";

        public string GetGiftsGiven() =>
            _items.Count.ToString(CultureInfo.InvariantCulture);

        public string GetWorkNeeded() =>
            _giftsGiven.ToString(CultureInfo.InvariantCulture);
    }
}