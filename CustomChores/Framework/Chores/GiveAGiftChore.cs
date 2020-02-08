using System;
using System.Collections.Generic;
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
        private readonly bool _enableUniversal;
        private readonly double _chanceForLove;
        private readonly IEnumerable<int> _universalLoves;
        private readonly IEnumerable<int> _universalLikes;
        private string _itemName;
        private int _itemId;

        public GiveAGiftChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("GiftType", out var giftType);
            ChoreData.Config.TryGetValue("EnableUniversal", out var enableUniversal);
            ChoreData.Config.TryGetValue("ChanceForLove", out var chanceForLove);

            _giftType = giftType is string s && !string.IsNullOrWhiteSpace(s) ? s : "Birthday";

            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
                return;

            _enableUniversal = enableUniversal is bool b && b;
            _chanceForLove = chanceForLove is double d ? d : 0.1;

            Game1.NPCGiftTastes.TryGetValue("Universal_Love", out var universalLoves);
            if (!string.IsNullOrWhiteSpace(universalLoves))
                _universalLoves =
                    from a in universalLoves.Split(' ')
                    select Convert.ToInt32(a);

            Game1.NPCGiftTastes.TryGetValue("Universal_Like", out var universalLikes);
            if (universalLikes != null)
                _universalLikes =
                    from a in universalLikes.Split(' ')
                    select Convert.ToInt32(a);
        }

        public override bool CanDoIt()
        {
            if (!_giftType.Equals("Birthday", StringComparison.CurrentCultureIgnoreCase))
                return true;
            _todayBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            return _todayBirthdayNpc != null && !_todayBirthdayNpc.getName().Equals(Game1.player.getSpouse().getName());
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
                    select Convert.ToInt32(itemId));
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
                        select Convert.ToInt32(s));
                    if (_enableUniversal)
                        itemIds.AddRange(_universalLoves);
                }
                else
                {
                    itemIds.AddRange(
                        from s in personalData[3].Split(' ')
                        select Convert.ToInt32(s));
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
            var item = objects.Shuffle().First();

            // Store item to give to player
            _itemId = item.Key;
            _itemName = item.Value.Split('/')[0];

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens(IContentHelper contentHelper)
        {
            var tokens = base.GetTokens(contentHelper);
            tokens.Add("ItemName", GetItemName);
            tokens.Add("ItemId", GetItemId);
            tokens.Add("Birthday", GetBirthdayName);
            tokens.Add("BirthdayGender", GetBirthdayGender);
            return tokens;
        }

        public string GetItemName()
        {
            return _itemName;
        }

        public string GetItemId()
        {
            return _itemId.ToString();
        }

        public static string GetBirthdayName()
        {
            return Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth)?.getName();
        }

        public static string GetBirthdayGender()
        {
            return Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth)?.Gender == 1
                ? "Female" : "Male";
        }
    }
}