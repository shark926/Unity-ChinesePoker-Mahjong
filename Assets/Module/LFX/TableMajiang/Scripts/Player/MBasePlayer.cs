﻿using System.Collections.Generic;

namespace Mahjong
{
    /// <summary>
    /// 桌上玩家类
    /// </summary>
    public class Player
    {
        //玩家手牌
        private List<CardInfo> _myCards = new List<CardInfo>();

        public List<CardInfo> myCards
        {
            get
            {
                return _myCards;
            }
            set
            {
                _myCards = value;
            }
        }

        /// <summary>
        /// 添加麻将到玩家的myCards列表中
        /// </summary>
        /// <param name="mCardInfo"></param>
        public void AddCard(CardInfo mCardInfo)
        {
            _myCards.Add(mCardInfo);
        }
    }
}