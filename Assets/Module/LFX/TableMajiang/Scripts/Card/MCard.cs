using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mahjong
{
    public enum CardState
    {
        Hide,
        B,
        N,
        Opp,
        Left,
        Right
    }

    public class MCard : MonoBehaviour
    {
        private string _name;
        private int _cardIndex;               //索引
        private Image _image;                 //图片

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int cardIndex
        {
            get
            {
                return _cardIndex;
            }
            set
            {
                _cardIndex = value;
            }
        }

        public Image image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        private CardType card;

        public CardType Card
        {
            get
            {
                return card;
            }
        }

        public void SetCard(CardType card)
        {
            this.card = card;
            ApplySprite();
        }

        private CardState state;

        public void SetCardState(CardState state)
        {
            this.state = state;

            ApplySprite();
        }

        private void ApplySprite()
        {
            if (state == CardState.Hide)
            {
                _image.sprite = null;

                return;
            }

            string path = "LFX/TableMajiang/SpritePack/Majiang/";

            string assetName = null;
            if (state == CardState.B || state == CardState.N)
            {
                assetName = string.Format("{0}_{1}", state, card);
            }
            else
            {
                assetName = state.ToString();
            }

            string assetPath = string.Format("{0}{1}", path, assetName);

            Sprite sprite = Resources.Load<Sprite>(assetPath);

            _image.sprite = sprite;
        }

        //点击事件
        private Action<string> SetSelectEvent;

        public void AddSetSelectEvent(Action<string> action)
        {
            SetSelectEvent = action;
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        /// <summary>
        /// 设置选择状态,EventTrigger的点击事件
        /// </summary>
        public void SetSelectState()
        {
            if (transform.parent.name != "Player0")
                return;

            if (SetSelectEvent != null)
            {
                SetSelectEvent(this.name);
            }
        }

        public static MCard Create(CardType card)
        {
            string path = "LFX/TableMajiang/Prefab/Majiang";

            GameObject go = Instantiate(Resources.Load<GameObject>(path));

            return go.GetComponent<MCard>();
        }
    }
}