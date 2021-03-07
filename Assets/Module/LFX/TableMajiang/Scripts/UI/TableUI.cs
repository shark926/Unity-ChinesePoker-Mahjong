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

        //4个玩家碰杠的位置
        private List<Vector3> playerPengPos = new List<Vector3>();

        //一些按钮
        private Button btnStart;

        private Button btnRestart;
        private Button btnSetting;
        private Button btnBack;
        private Button btnPeng;
        private Button btnGang;
        private Button btnHu;
        private Button btnPass;
        private Button temp;

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

            btnStart = GameObject.Find("Content/Button_start").GetComponent<Button>();
            btnRestart = GameObject.Find("Content/Button_restart").GetComponent<Button>();
            btnSetting = GameObject.Find("Content/Button_tablesetting").GetComponent<Button>();
            btnBack = GameObject.Find("Content/Button_back").GetComponent<Button>();

            btnPeng = GameObject.FindGameObjectWithTag("Peng").GetComponent<Button>();
            btnGang = GameObject.FindGameObjectWithTag("Gang").GetComponent<Button>();
            btnHu = GameObject.FindGameObjectWithTag("Hu").GetComponent<Button>();
            btnPass = GameObject.FindGameObjectWithTag("Pass").GetComponent<Button>();


            playerPengPos.Add(GameObject.Find("Content/Player0/Peng").transform.position);
            playerPengPos.Add(GameObject.Find("Content/PlayerAI1/Peng").transform.position);
            playerPengPos.Add(GameObject.Find("Content/PlayerAI2/Peng").transform.position);
            playerPengPos.Add(GameObject.Find("Content/PlayerAI3/Peng").transform.position);

            btnStart.onClick.AddListener(_btn_StartGame);
            btnRestart.onClick.AddListener(_btn_Restart);
            btnSetting.onClick.AddListener(_btn_Setting);
            btnBack.onClick.AddListener(_btn_Back);

            btnPeng.onClick.AddListener(_btn_Peng);
            btnGang.onClick.AddListener(_btn_Gang);
            btnHu.onClick.AddListener(_btn_Hu);
            btnPass.onClick.AddListener(_btn_Pass);

            btnRestart.gameObject.SetActive(false);
        }

        //开始游戏
        private Action _startGameEvent = null;

        public void AddStartGameEvent(Action action)
        {
            _startGameEvent = action;
        }

        //重新开始游戏
        private Action _reStartGameEvent = null;

        public void AddReStartGameEvent(Action action)
        {
            _reStartGameEvent = action;
        }

        //打开设置
        private Action _settingEvetn = null;

        public void AddSettingEvent(Action action)
        {
            _settingEvetn = action;
        }

        //完成发牌动画，开始出牌
        private Action _finshSendAnitmaEvent;

        public void AddFinshSendAnitmaEvent(Action action)
        {
            _finshSendAnitmaEvent = action;
        }

        //碰
        private Action _pengEvent;

        public void AddPengEvent(Action action)
        {
            _pengEvent = action;
        }

        //杠
        private Action _gangEvent;

        public void AddGangEvent(Action action)
        {
            _gangEvent = action;
        }

        //胡
        private Action _huEvent;

        public void AddHuEvent(Action action)
        {
            _huEvent = action;
        }

        //过
        private Action _passEvent;

        public void AddPassEvent(Action action)
        {
            _passEvent = action;
        }

        //出牌
        private Action<CardIndex> _playerClickEvent;

        public void AddPlayerClickEven(Action<CardIndex> action)
        {
            _playerClickEvent = action;
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
            if (_finshSendAnitmaEvent != null)
            {
                _finshSendAnitmaEvent();
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
        /// 根据数据返回卡牌物体
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        public MCard GetCardObject(Card cardInfo)
        {
            return (MCard)cardInfo.UserData;
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
        private void _btn_Back()
        {
            Hide();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void _btn_StartGame()
        {
            if (_startGameEvent != null)
            {
                _startGameEvent();
            }
            //点击开始游戏后再显示重新开始按钮
            btnRestart.gameObject.SetActive(true);
            btnStart.gameObject.SetActive(false);
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void _btn_Restart()
        {
            if (_reStartGameEvent != null)
            {
                _reStartGameEvent();
            }
        }

        /// <summary>
        /// 桌面设置界面
        /// </summary>
        private void _btn_Setting()
        {
            if (_settingEvetn != null)
            {
                _settingEvetn();
            }
        }

        /// <summary>
        /// 麻将点击事件
        /// </summary>
        /// <param name="card"></param>
        private void _clickMajiang(MCard card)
        {
            if (_playerClickEvent != null)
            {
                _playerClickEvent(card.Card);
            }
        }

        /// <summary>
        /// 玩家点击碰
        /// </summary>
        private void _btn_Peng()
        {
            if (_pengEvent != null)
            {
                _pengEvent();
            }
        }

        /// <summary>
        /// 玩家点击杠
        /// </summary>
        private void _btn_Gang()
        {
            if (_gangEvent != null)
            {
                _gangEvent();
            }
        }

        /// <summary>
        /// 玩家点击胡
        /// </summary>
        private void _btn_Hu()
        {
            if (_huEvent != null)
            {
                _huEvent();
            }
        }

        /// <summary>
        /// 玩家点击过
        /// </summary>
        private void _btn_Pass()
        {
            if (_passEvent != null)
            {
                _passEvent();
            }
        }
    }
}