﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mahjong
{
    public class MCard : MonoBehaviour
    {
        #region 定义变量

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

        #endregion 定义变量

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
    }
}