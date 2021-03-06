﻿using System.Collections.Generic;

namespace Mahjong
{
    /// <summary>
    /// 麻将胡牌的判断
    /// </summary>
    public class Rules
    {
        /// <summary>
        /// 碰判断
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        public static bool IsPeng(List<Card> list, Card cardInfo)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].CardIndex == list[i + 1].CardIndex
                    && list[i].CardIndex == cardInfo.CardIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断是否能杠牌
        /// </summary>
        /// <param name="list"></param>
        /// <param name="mCardInfo"></param>
        /// <returns></returns>
        public static bool IsGang(List<Card> list, Card mCardInfo)
        {
            for (int i = 0; i < list.Count - 2; i++)
            {
                if (list[i].CardIndex == list[i + 1].CardIndex
                    && list[i].CardIndex == mCardInfo.CardIndex
                    && list[i + 2].CardIndex == list[i + 1].CardIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断是否能胡牌
        /// </summary>
        /// <param name="list"></param>
        /// <param name="mCardInfo"></param>
        /// <returns></returns>
        public static bool IsCanHU(List<Card> list, Card mCardInfo)
        {
            List<CardIndex> pais = new List<CardIndex>();
            for (int i = 0; i < list.Count; i++)
            {
                pais.Add(list[i].CardIndex);
            }
            if (mCardInfo != null)
                pais.Add(mCardInfo.CardIndex);

            //只有两张牌
            if (pais.Count == 2)
            {
                return pais[0] == pais[1];
            }

            //先排序
            pais.Sort();

            //依据牌的顺序从左到右依次分出将牌
            for (int i = 0; i < pais.Count; i++)
            {
                List<CardIndex> paiT = new List<CardIndex>(pais);
                List<CardIndex> ds = pais.FindAll(delegate (CardIndex d)
                {
                    return pais[i] == d;
                });

                //判断是否能做将牌
                if (ds.Count >= 2)
                {
                    //移除两张将牌
                    paiT.Remove(pais[i]);
                    paiT.Remove(pais[i]);

                    //避免重复运算 将光标移到其他牌上
                    i += ds.Count;

                    if (HuPaiPanDin(paiT))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool HuPaiPanDin(List<CardIndex> mahs)
        {
            if (mahs.Count == 0)
            {
                return true;
            }

            List<CardIndex> fs = mahs.FindAll(delegate (CardIndex a)
            {
                return mahs[0] == a;
            });

            //组成克子
            if (fs.Count == 3)
            {
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);

                return HuPaiPanDin(mahs);
            }
            else
            { //组成顺子
                if (mahs.Contains(mahs[0] + 1) && mahs.Contains(mahs[0] + 2))
                {
                    mahs.Remove(mahs[0] + 2);
                    mahs.Remove(mahs[0] + 1);
                    mahs.Remove(mahs[0]);

                    return HuPaiPanDin(mahs);
                }
                return false;
            }
        }

        /// <summary>
        /// 排序当前手牌
        /// </summary>
        private static List<Card> SortCards(List<Card> list)
        {
            if (list.Count == 0)
                return null;

            list.Sort(delegate (Card x, Card y)
            {
                return x.CardIndex.CompareTo(y.CardIndex);
            });

            return list;
        }
    }
}