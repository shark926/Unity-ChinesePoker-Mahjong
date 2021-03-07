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
        private Image _image;

        private CardIndex card;

        public CardIndex Card
        {
            get
            {
                return card;
            }
        }

        public void SetCardIndex(CardIndex card)
        {
            this.card = card;
            ApplySprite();
        }

        private CardState state;

        public void SetState(CardState state)
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
                assetName = string.Format("{0}_{1}", state, (int)card);
            }
            else
            {
                assetName = state.ToString();
            }

            string assetPath = string.Format("{0}{1}", path, assetName.ToLower());

            Sprite sprite = Resources.Load<Sprite>(assetPath);

            _image.sprite = sprite;
        }

        //点击事件
        private Action<MCard> SetSelectEvent;

        public void AddSetSelectEvent(Action<MCard> action)
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
                SetSelectEvent(this);
            }
        }

        public static MCard Create(CardIndex card)
        {
            string path = "LFX/TableMajiang/Prefab/Majiang";

            GameObject go = Instantiate(Resources.Load<GameObject>(path));

            return go.GetComponent<MCard>();
        }
    }
}