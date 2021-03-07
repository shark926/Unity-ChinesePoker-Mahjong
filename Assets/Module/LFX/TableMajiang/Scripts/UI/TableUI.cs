using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WT.UI;

namespace Mahjong
{
    public class TableUI : WTUIPage
    {
        private List<Seat> seats = new List<Seat>(4);

        //Transform table = null;
        //一些按钮
        private Button btnStart;
        private Button btnRestart;
        private Button btnSetting;
        private Button btnBack;
        private Button btnPeng;
        private Button btnGang;
        private Button btnHu;
        private Button btnPass;

        public TableUI() : base(UIType.Normal, UIMode.DoNothing, UICollider.None)
        {
            uiIndex = R.Prefab.MAJIANGTABLE;
        }

        public override void Awake(GameObject go)
        {
            seats.Add(new SeatSelf());
            seats.Add(new SeatRight());
            seats.Add(new SeatOppsite());
            seats.Add(new SeatLeft());

            //table = GameObject.Find("Content/Table").transform;

            btnStart = GameObject.Find("Content/Button_start").GetComponent<Button>();
            btnRestart = GameObject.Find("Content/Button_restart").GetComponent<Button>();
            btnSetting = GameObject.Find("Content/Button_tablesetting").GetComponent<Button>();
            btnBack = GameObject.Find("Content/Button_back").GetComponent<Button>();

            btnPeng = GameObject.FindGameObjectWithTag("Peng").GetComponent<Button>();
            btnGang = GameObject.FindGameObjectWithTag("Gang").GetComponent<Button>();
            btnHu = GameObject.FindGameObjectWithTag("Hu").GetComponent<Button>();
            btnPass = GameObject.FindGameObjectWithTag("Pass").GetComponent<Button>();

            btnStart.onClick.AddListener(OnStartGame);
            btnRestart.onClick.AddListener(OnRestart);
            btnSetting.onClick.AddListener(OnSetting);
            btnBack.onClick.AddListener(OnBack);

            btnPeng.onClick.AddListener(OnPeng);
            btnGang.onClick.AddListener(OnGang);
            btnHu.onClick.AddListener(OnHu);
            btnPass.onClick.AddListener(OnPass);

            btnRestart.gameObject.SetActive(false);
        }

        //开始游戏
        private Action startGameEvent = null;
        public void AddStartGameEvent(Action action)
        {
            startGameEvent = action;
        }

        //重新开始游戏
        private Action restartGameEvent = null;
        public void AddReStartGameEvent(Action action)
        {
            restartGameEvent = action;
        }

        //打开设置
        private Action settingEvetn = null;

        public void AddSettingEvent(Action action)
        {
            settingEvetn = action;
        }

        //完成发牌动画，开始出牌
        private Action finshSendAnitmaEvent;

        public void AddFinshSendAnitmaEvent(Action action)
        {
            finshSendAnitmaEvent = action;
        }

        //碰
        private Action pengEvent;

        public void AddPengEvent(Action action)
        {
            pengEvent = action;
        }

        //杠
        private Action gangEvent;

        public void AddGangEvent(Action action)
        {
            gangEvent = action;
        }

        //胡
        private Action huEvent;

        public void AddHuEvent(Action action)
        {
            huEvent = action;
        }

        //过
        private Action passEvent;

        public void AddPassEvent(Action action)
        {
            passEvent = action;
        }

        //出牌
        private Action<CardIndex> playerClickEvent;

        public void AddPlayerClickEven(Action<CardIndex> action)
        {
            playerClickEvent = action;
        }

        /// <summary>
        /// 实例化麻将
        /// </summary>
        /// <param name="list"></param>
        public void InstanceCards(List<Card> list)
        {
            //根据数据实例化麻将到桌面

            GameObject obj = WTUIPage.delegateSyncLoadUIByLocal(R.Prefab.MAJIANG) as GameObject;

            for (int i = 0; i < list.Count; i++)
            {
                GameObject o = GameObject.Instantiate(obj, new Vector3(0, 3, 0), Quaternion.identity, transform) as GameObject;
                MCard c = o.GetComponent<MCard>();

                c.SetCardIndex(list[i].CardIndex);
                list[i].UserData = c;
                //绑定麻将点击事件
                c.AddSetSelectEvent(_clickMajiang);
            }
        }

        /// <summary>
        /// 发牌动画
        /// </summary>
        /// <param name="players"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public void SendMajiangAnimation(List<Player> players)
        {
            UnitTool.ToolStartCoroutine(SendMajiangCoroutine(players));
        }

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

        /// <summary>
        /// 清除UI数据
        /// </summary>
        public void ClearUi()
        {
        }

        /// <summary>
        /// 设置麻将按钮状态
        /// </summary>
        /// <param name="peng"></param>
        /// <param name="gang"></param>
        /// <param name="hu"></param>
        /// <param name="pass"></param>
        public void SetButton(bool peng, bool gang, bool hu, bool pass)
        {
            btnPeng.interactable = peng;
            btnGang.interactable = gang;
            btnHu.interactable = hu;
            btnPass.interactable = pass;
        }

        /// <summary>
        /// 返回大厅
        /// </summary>
        private void OnBack()
        {
            Hide();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void OnStartGame()
        {
            if (startGameEvent != null)
            {
                startGameEvent();
            }
            //点击开始游戏后再显示重新开始按钮
            btnRestart.gameObject.SetActive(true);
            btnStart.gameObject.SetActive(false);
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void OnRestart()
        {
            if (restartGameEvent != null)
            {
                restartGameEvent();
            }
        }

        /// <summary>
        /// 桌面设置界面
        /// </summary>
        private void OnSetting()
        {
            if (settingEvetn != null)
            {
                settingEvetn();
            }
        }

        /// <summary>
        /// 麻将点击事件
        /// </summary>
        /// <param name="card"></param>
        private void _clickMajiang(MCard card)
        {
            if (playerClickEvent != null)
            {
                playerClickEvent(card.Card);
            }
        }

        /// <summary>
        /// 玩家点击碰
        /// </summary>
        private void OnPeng()
        {
            if (pengEvent != null)
            {
                pengEvent();
            }
        }

        /// <summary>
        /// 玩家点击杠
        /// </summary>
        private void OnGang()
        {
            if (gangEvent != null)
            {
                gangEvent();
            }
        }

        /// <summary>
        /// 玩家点击胡
        /// </summary>
        private void OnHu()
        {
            if (huEvent != null)
            {
                huEvent();
            }
        }

        /// <summary>
        /// 玩家点击过
        /// </summary>
        private void OnPass()
        {
            if (passEvent != null)
            {
                passEvent();
            }
        }
    }
}