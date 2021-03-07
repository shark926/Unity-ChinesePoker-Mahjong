using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public abstract class Seat
    {
        protected Transform table;
        protected Transform trans = null;
        protected Vector3 pengPos;

        public Seat()
        {
            table = GameObject.Find("Content/Table").transform;
        }

        //摸牌
        public abstract void DrawCard(Card card);

        //出牌
        public void DropCard(Card card)
        {
            MCard cardObj = GetCardObj(card);
            cardObj.SetState(CardState.B);

            cardObj.transform.position = GetDropCard();
            cardObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            cardObj.transform.SetParent(table);
        }

        protected abstract Vector3 GetDropCard();

        public abstract void FreshCard(List<Card> list, int index);

        public void Peng(List<Card> list)
        {
            pengPos += GetPengPosOffset();

            Vector3 pos = pengPos;

            for (int i = 0; i < list.Count; i++)
            {
                pos.x += 0.5f;

                MCard cardObj = GetCardObj(list[i]);
                cardObj.SetState(CardState.B);
                cardObj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                cardObj.transform.position = pos;
                cardObj.transform.SetParent(table);
            }
        }

        protected abstract Vector3 GetPengPosOffset();

        public void Gang(List<Card> list)
        {
        }

        public void ShowHuMajiang(List<Card> list)
        {
            //牌的显示位置
            List<Vector3> pos = new List<Vector3>();
            //牌所在的位置
            Vector3 basePos = Vector3.zero;

            basePos = table.position + new Vector3(list.Count * -0.45f, 0, 0);
            for (int i = 0; i < list.Count; i++)
            {
                basePos.x += 0.8f;
                pos.Add(basePos);
            }

            for (int i = 0; i < pos.Count; i++)
            {
                var card = GetCardObj(list[i]);
                card.SetState(CardState.B);
                card.transform.SetParent(table);
                card.transform.SetAsLastSibling();
                card.transform.localScale = new Vector3(1, 1, 1);
                card.transform.position = pos[i];
            }
        }

        protected MCard GetCardObj(Card card)
        {
            return (MCard)card.UserData;
        }

        protected void ApplyCard(MCard cardObj, Vector3 pos)
        {
            cardObj.transform.position = pos;
            cardObj.transform.localScale = new Vector3(1f, 1f, 1f);
            cardObj.transform.SetParent(trans);
            cardObj.transform.SetAsFirstSibling();
        }
    }

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