using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WT.UI;

namespace Mahjong
{
    /// <summary>
    /// 桌面控制
    /// </summary>
    public class Control
    {
        //桌面
        private TableUI majiangTable;

        //桌面牌数据
        private List<Card> tCards = new List<Card>();

        //玩家出的牌数据
        private Card pCard;

        //桌面上的玩家
        private List<Player> players = new List<Player>();

        //临时麻将
        private List<Card> temp = new List<Card>();

        //庄家索引
        private int startIndex;

        //玩家索引
        private int index;

        //赢家索引
        private int winIndex;

        //临时玩家索引
        private int tempIndex;

        //是否在发牌
        private bool isSend = true;

        //是否能点击麻将
        private bool canClick = true;

        public Control()
        {
            majiangTable = new TableUI();

            //生成玩家索引
            startIndex = MyUtil.GetRange(0, 4);
            index = startIndex;

            //生成四个玩家 Mplayer（控制器控制）
            for (int i = 0; i < 4; i++)
            {
                players.Add(new Player());
            }

            //添加按钮代理 Action (绑定界面按钮事件)
            AddAction();
        }

        //Action，碰，杠，胡，过, 开始游戏， 重新开始游戏

        //设置
        private Action settingEvent = null;

        public void AddSettingEvent(Action action)
        {
            settingEvent = action;
        }

        //输赢界面
        private Action<string> gameOverEvent = null;

        public void AddGameOverEvent(Action<string> action)
        {
            gameOverEvent = action;
        }

        /// <summary>
        /// 显示桌面UI
        /// </summary>
        public void ShowMajiangUI()
        {
            WTUIPage.ShowPage("MajiangUI", majiangTable);
        }

        /// <summary>
        /// 显示设置界面
        /// </summary>
        private void ShowSetting()
        {
            if (settingEvent != null)
            {
                settingEvent();
            }
        }

        /// <summary>
        /// 绑定代理事件
        /// </summary>
        private void AddAction()
        {
            majiangTable.AddStartGameEvent(StartGame);
            majiangTable.AddReStartGameEvent(RestartGame);
            majiangTable.AddSettingEvent(ShowSetting);

            //玩家点击麻将出牌事件
            majiangTable.AddPlayerClickEven(PlayerClickMaJiang);
            //完成发牌动画后出牌
            majiangTable.AddFinshSendAnitmaEvent(StartPlayMajiang);

            //玩家按钮按钮事件碰胡杠过
            majiangTable.AddPengEvent(delegate ()
            {
                canClick = true;
                ButtonPeng();
            });
            majiangTable.AddGangEvent(delegate ()
            {
                canClick = true;
                ButtonGang();
            });
            majiangTable.AddHuEvent(delegate ()
            {
                SetButton(false, false, false, false);
                HuMajiang();
            });
            majiangTable.AddPassEvent(delegate ()
            {
                canClick = true;
                SetButton(false, false, false, false);
                index = tempIndex;
                index++;
                index = index % 4;
                ComputerAIPlayMajiang();
            });
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            //生成麻将数据(包括洗牌)
            InitCards();

            //实例化到桌 桌调用UI（传数据）
            majiangTable.InstanceCards(tCards);

            //发牌给四个玩家（只发数据）
            SendMajiang();

            //排序
            PlayerSortCards();

            //显示发牌动画(发牌动画完成后UI回调开始出麻将)
            isSend = true;
            majiangTable.SendMajiangAnimation(players);
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void RestartGame()
        {
            //关闭所有协程
            UnitTool.ToolStopAllCoroutines();

            //清除玩家和桌控制器的数据，重置UI的显示
            ClearDatas();

            //调用开始游戏
            StartGame();
        }

        /// <summary>
        /// 发牌
        /// </summary>
        /// <returns></returns>
        private void SendMajiang()
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    players[j].AddCard(tCards[0]);
                    tCards.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 完成动画后开始出牌回调函数
        /// </summary>
        private void StartPlayMajiang()
        {
            //庄家先摸牌
            DrawMajiang();
            //摸牌后检测胡牌
            if (CheckHu(CurPlayerCards, null, index))
                return;

            isSend = false;

            if (index % 4 == 0)
            {
                //庄家是玩家
                return;
            }
            else
            {
                //庄家是电脑
                ComputerAIPlayMajiang();
            }
        }

        /// <summary>
        /// 玩家摸牌
        /// </summary>
        private void DrawMajiang()
        {
            //该玩家摸牌
            if (tCards.Count == 0)
            {
                UnitTool.ToolStopAllCoroutines();
                Debug.Log("牌抓完了...平局");
                //打开平局面板
                UnitTool.ToolStopAllCoroutines();
                if (gameOverEvent != null)
                {
                    gameOverEvent("平局");
                }
                return;
            }

            players[index].AddCard(tCards[0]);
            var moCard = tCards[0];
            majiangTable.DrawCard(tCards[0], index);
            SortCards(CurPlayerCards);
            tCards.RemoveAt(0);
        }

        /// <summary>
        /// 人家玩家点击麻将出牌
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="cardName"></param>
        private void PlayerClickMaJiang(CardIndex card)
        {
            if (index % 4 != 0 || isSend || !canClick)
                return;

            foreach (var item in CurPlayerCards)
            {
                if (item.CardIndex == card)
                {
                    PlayerPlay(item, true);
                    return;
                }
            }
        }

        /// <summary>
        /// 电脑玩家AI出麻将
        /// </summary>
        private void ComputerAIPlayMajiang()
        {
            if (index % 4 == 0)
                return;

            var card = AI.PlayCard(CurPlayerCards);
            PlayerPlay(card, false);
        }

        /// <summary>
        /// 出牌等待时间
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="cardName"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        private IEnumerator WaitCoroutine(Card cardInfo, bool go)
        {
            yield return new WaitForSeconds(1f);
            go = true;

            PlayerPlay(cardInfo, go);
        }

        private List<Card> CurPlayerCards
        {
            get
            {
                return players[index].myCards;
            }
        }
        /// <summary>
        /// 玩家出麻将
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="cardName"></param>
        /// <returns></returns>
        private void PlayerPlay(Card card, bool go)
        {
            if (!go)
            {
                UnitTool.ToolStartCoroutine(WaitCoroutine(card, false));
                return;
            }

            //当前玩家的出牌
            Debug.Log("Player " + index + " 出 " + card.CardIndex);
            int n = 0;
            for (int i = 0; i < CurPlayerCards.Count; i++)
            {
                if (CurPlayerCards[i] == card)
                {
                    n = i;
                    break;
                }
            }
            majiangTable.DropCard(card, CurPlayerCards, index);
            //保存这个玩家出的牌
            pCard = CurPlayerCards[n];
            //清除
            CurPlayerCards.RemoveAt(n);
            //当前玩家重新排序
            SortCards(CurPlayerCards);
            majiangTable.FreshCard(CurPlayerCards, index);

            //当前玩家出牌后，其他三个玩家进行检测胡、碰、杠操作
            CheckMajiang();
        }

        /// <summary>
        /// 控制器检测胡、杠、碰
        /// </summary>
        /// <returns></returns>
        private void CheckMajiang()
        {
            //检测其他三个玩家是否能胡牌
            if (CheckThreePlayerHu())
                return;

            //检测碰、杠
            if (CheckPengGang())
                return;

            //下一个玩家摸牌、检测、出牌
            index++;
            index = index % 4;
            DrawMajiang();

            if (CheckHu(CurPlayerCards, null, index))
                return;

            //电脑出牌
            if (index % 4 != 0)
            {
                ComputerAIPlayMajiang();
            }
        }

        /// <summary>
        /// 碰按钮调用
        /// </summary>
        private void ButtonPeng()
        {
            SetButton(false, false, false, false);
            UnitTool.ToolStopAllCoroutines();
            Peng(temp);
        }

        /// <summary>
        /// 杠按钮调用
        /// </summary>
        private void ButtonGang()
        {
            SetButton(false, false, false, false);
            UnitTool.ToolStopAllCoroutines();
            Gang(temp);
        }

        /// <summary>
        /// 检测碰、杠
        /// </summary>
        /// <returns></returns>
        private bool CheckPengGang()
        {
            CardIndex result = CardIndex.None;
            tempIndex = index;
            //检测其他三个玩家的碰、杠
            for (int i = index + 1; i < index + 4; i++)
            {
                if (Rules.IsPeng(players[i % 4].myCards, pCard)
                    || Rules.IsGang(players[i % 4].myCards, pCard))
                {
                    result = pCard.CardIndex;
                }

                if (result != CardIndex.None)
                {
                    index = i % 4;
                    //有玩家碰、杠了
                    temp.Clear();
                    for (int j = 0; j < CurPlayerCards.Count; j++)
                    {
                        if (CurPlayerCards[j].CardIndex == result)
                        {
                            temp.Add(CurPlayerCards[j]);
                        }
                    }
                    temp.Add(pCard);

                    //如果碰、杠的玩家是电脑玩家，直接碰、杠
                    if (index != 0)
                    {
                        if (temp.Count == 3)
                            Peng(temp);
                        if (temp.Count == 4)
                            Gang(temp);
                        temp.Clear();
                        return true;
                    }

                    //人玩家
                    if (index == 0)
                    {
                        canClick = false;
                        if (temp.Count == 3)
                            SetButton(true, false, false, true);
                        else
                            SetButton(false, true, false, true);
                        //等待过按钮按下
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 碰
        /// </summary>
        /// <param name="list"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private void Peng(List<Card> list)
        {
            Debug.Log("Player " + index.ToString() + " 碰!");

            for (int i = 0; i < list.Count; i++)
                CurPlayerCards.Remove(list[i]);

            majiangTable.FreshCard(CurPlayerCards, index);
            majiangTable.ShowPengGang(list, index);

            //yield return new WaitForSeconds(1f);

            //电脑碰完直接出
            if (index % 4 != 0)
            {
                ComputerAIPlayMajiang();
            }
        }

        /// <summary>
        /// 杠
        /// </summary>
        /// <param name="list"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private void Gang(List<Card> list)
        {
            Debug.Log("Player " + index.ToString() + " 杠!");

            for (int i = 0; i < list.Count; i++)
                CurPlayerCards.Remove(list[i]);

            majiangTable.FreshCard(CurPlayerCards, index);
            majiangTable.ShowPengGang(list, index);

            //杠完再摸一张、检测、再出
            DrawMajiang();
            if (CheckHu(CurPlayerCards, null, index))
                return;

            //yield return new WaitForSeconds(1f);

            if (index % 4 != 0)
            {
                ComputerAIPlayMajiang();
            }
        }

        /// <summary>
        /// 检查某个玩家是否胡牌
        /// </summary>
        /// <param name="list"></param>
        /// <param name="mCardInfo"></param>
        /// <returns></returns>
        private bool CheckHu(List<Card> list, Card mCardInfo, int win)
        {
            bool canHu = false;
            if (Rules.IsCanHU(list, mCardInfo))
            {
                canHu = true;
                winIndex = win;
                if (mCardInfo != null)
                    players[win].myCards.Add(pCard);
                if (winIndex % 4 != 0)
                {
                    HuMajiang();
                }
                else
                {
                    SetButton(false, false, true, false);
                }
            }

            return canHu;
        }

        private bool CheckThreePlayerHu()
        {
            //检测三个玩家是否胡
            bool isHu = false;
            for (int i = index + 1; i < index + 4; i++)
            {
                if (CheckHu(players[i % 4].myCards, pCard, i % 4))
                {
                    isHu = true;
                    break;
                }
            }

            return isHu;
        }

        /// <summary>
        /// 胡牌
        /// </summary>
        private void HuMajiang()
        {
            //停止所有协程
            UnitTool.ToolStopAllCoroutines();

            //玩家胡,显示赢家的麻将
            SortCards(players[winIndex].myCards);
            majiangTable.ShowHu(players[winIndex].myCards);

            //控制器显示胜利
            GameOver();
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <returns></returns>
        private void GameOver()
        {
            Debug.Log("Player " + winIndex + " Win");

            if (gameOverEvent != null)
            {
                string info;
                if (winIndex % 4 == 0)
                    info = "你赢了！！！";
                else
                    info = "Player " + winIndex + "赢了\n" + "你输了...";
                gameOverEvent(info);
            }
        }

        /// <summary>
        /// 设置玩家按钮
        /// </summary>
        /// <param name="peng"></param>
        /// <param name="gang"></param>
        /// <param name="hu"></param>
        /// <param name="pass"></param>
        private void SetButton(bool peng, bool gang, bool hu, bool pass)
        {
            majiangTable.SetButton(peng, gang, hu, pass);
        }

        /// <summary>
        /// 生成卡牌数据
        /// </summary>
        private void InitCards()
        {
            foreach (int item in Enum.GetValues(typeof(CardIndex)))
            {
                CardIndex cardType = (CardIndex)item;
                if (cardType > CardIndex.None
                    && cardType < CardIndex.Dong)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Card cardInfo = new Card(cardType);
                        tCards.Add(cardInfo);
                    }
                }
            }

            //洗牌
            tCards = GetRandomList<Card>(tCards);
        }

        /// <summary>
        /// 排序当前手牌
        /// </summary>
        private void SortCards(List<Card> list)
        {
            if (list.Count == 0)
                return;

            list.Sort(delegate (Card x, Card y)
            {
                return x.CardIndex.CompareTo(y.CardIndex);
            });
        }

        /// <summary>
        /// list随机排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputList"></param>
        /// <returns></returns>
        private List<T> GetRandomList<T>(List<T> inputList)
        {
            //赋值数组
            T[] copyArray = new T[inputList.Count];
            inputList.CopyTo(copyArray);

            //添加
            List<T> copyList = new List<T>();
            copyList.AddRange(copyArray);

            //设置随机
            List<T> outputList = new List<T>();

            while (copyList.Count > 0)
            {
                //选择一个index和item
                int rdIndex = MyUtil.GetRange(0, copyList.Count - 1);
                T remove = copyList[rdIndex];
                //从赋值list删除添加到输出列表
                copyList.Remove(remove);
                outputList.Add(remove);
            }
            return outputList;
        }

        /// <summary>
        /// 玩家对手里的麻将排序
        /// </summary>
        private void PlayerSortCards()
        {
            for (int i = 0; i < 4; i++)
            {
                SortCards(players[i].myCards);
            }
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        private void ClearDatas()
        {
            //清除桌数据
            pCard = null;
            tCards.Clear();
            temp.Clear();
            //清除玩家手牌
            for (int i = 0; i < 4; i++)
            {
                players[i].myCards.Clear();
            }
            //重置玩家索引
            index = MyUtil.GetRange(0, 4);
            //清除UI
            majiangTable.ClearUi();
        }
    }
}