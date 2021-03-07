using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WT.UI;

namespace Mahjong
{
    public partial class TableUI
    {
        private List<Seat> seats = new List<Seat>(4);

        /// <summary>
        /// 发牌协程
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        private IEnumerator SendMajiangCoroutine(List<Player> players)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    yield return new WaitForSeconds(0.1f);

                    seats[j].FreshCard(players[j].myCards, i);
                }
            }
            yield return new WaitForSeconds(0.5f);

            //告诉控制器发牌动画完成了.可以出牌了
            if (finshSendAnitmaEvent != null)
            {
                finshSendAnitmaEvent();
            }
        }

        /// <summary>
        /// 摸牌动画
        /// </summary>
        /// <param name="mCardInfo"></param>
        /// <param name="index"></param>
        public void DrawMajiangAnimation(Card mCardInfo, int index)
        {
            seats[index].DrawCard(mCardInfo);
        }

        /// <summary>
        /// 玩家出麻将动画
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>
        /// <param name="index"></param>
        public void PlayMajiangAnimation(Card cardInfo, List<Card> list, int index)
        {
            seats[index].DropCard(cardInfo);
        }

        /// <summary>
        /// 显示玩家手里的麻将
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="isSend"></param>
        /// <returns></returns>
        public void ShowMajiang(List<Card> list, int index)
        {
            if (list.Count == 0)
            {
                Debug.Log("MyCards is Empty!");
                return;
            }

            seats[index].FreshCard(list, -1);
        }

        /// <summary>
        /// 显示玩家碰、杠的牌
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        public void ShowPengMajiang(List<Card> list, int index)
        {
            seats[index].Peng(list);
        }

        public void ShowHuMajiang(List<Card> list)
        {
            // 胡牌暂时都是一样的，随意随便找个seat显示，因为它有table的引用
            seats[0].ShowHuMajiang(list);
        }
    }
}