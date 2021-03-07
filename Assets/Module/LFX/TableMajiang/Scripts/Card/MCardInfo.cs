namespace Mahjong
{
    /// <summary>
    /// 卡牌信息
    /// </summary>
    public enum CardIndex
    {
        None = 0,

        //筒
        Tong1 = 1,
        Tong2,
        Tong3,
        Tong4,
        Tong5,
        Tong6,
        Tong7,
        Tong8,
        Tong9,

        //条
        Tiao1 = 11,
        Tiao2,
        Tiao3,
        Tiao4,
        Tiao5,
        Tiao6,
        Tiao7,
        Tiao8,
        Tiao9,

        //万
        Wan1 = 21,
        Wan2,
        Wan3,
        Wan4,
        Wan5,
        Wan6,
        Wan7,
        Wan8,
        Wan9,

        Dong = 31,
        Nan = 33,
        Xi = 35,
        Bei = 37,

        Zhong = 41,
        Fa = 43,
        Bai = 45,

        Max
    }

    public class Card
    {
        private CardIndex cardIndex;
        public CardIndex CardIndex
        {
            get
            {
                return cardIndex;
            }
        }

        public object UserData
        {
            get;set;
        }

        public Card(CardIndex cardType)
        {
            this.cardIndex = cardType;
        }
    }
}