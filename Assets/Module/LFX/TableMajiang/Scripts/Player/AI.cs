using System.Collections.Generic;

namespace Mahjong
{
    /// <summary>
    /// 电脑AI出牌
    /// </summary>
    public class AI
    {
        public static CardInfo PlayCard(List<CardInfo> list)
        {
            var x = FindDropCard(list);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Card == x)
                {
                    return list[i];
                }
            }

            return list[list.Count - 1];
        }

        /// <summary>
        /// 找到不是顺子，不是对子的牌，出掉
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static CardType FindDropCard(List<CardInfo> list)
        {
            CardType index = CardType.None;
            List<CardType> cards = new List<CardType>();

            for (int i = 0; i < list.Count; i++)
            {
                cards.Add(list[i].Card);
            }

            if (cards[0] != cards[1] && (cards[0] + 1) != cards[1])
                return cards[0];

            for (int i = 1; i < cards.Count - 1; i++)
            {
                if (cards[i] != cards[i - 1] && cards[i] != cards[i + 1] && (cards[i] - 1) != cards[i - 1] && (cards[i] + 1) != cards[i + 1])
                    return cards[i];
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[cards.Count - 1] != cards[cards.Count - 2])
                {
                    return cards[cards.Count - 1];
                }
            }

            return index;
        }
    }
}