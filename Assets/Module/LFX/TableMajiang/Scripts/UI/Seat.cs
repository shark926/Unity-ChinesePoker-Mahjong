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

    public class SeatSelf : Seat
    {
        public SeatSelf()
        {
            trans = GameObject.Find("Content/Player0").transform;
            pengPos = GameObject.Find("Content/Player0/Peng").transform.position;
        }

        protected override Vector3 GetPengPosOffset()
        {
            return new Vector3(2.1f, 0, 0);
        }

        public override void DrawCard(Card card)
        {
            Vector3 pos = trans.position + new Vector3(3f, 0, 0);

            MCard cardObj = GetCardObj(card);
            cardObj.SetState(CardState.N);
            ApplyCard(cardObj, pos + new Vector3(2, 0, 0));
        }

        protected override Vector3 GetDropCard()
        {
            return trans.position + new Vector3(4.5f, 1.3f, 0);
        }

        public override void FreshCard(List<Card> list, int index)
        {
            Vector3 pos = trans.position + new Vector3(list.Count * -0.5f, 0, 0);

            if (index >= 0)
            {
                MCard cardObj = GetCardObj(list[index]);
                pos.x += 0.8f * (index + 1);
                cardObj.SetState(CardState.N);
                ApplyCard(cardObj, pos);

                return;
            }

            foreach (var item in list)
            {
                MCard cardObj = GetCardObj(item);

                pos.x += 0.8f;
                cardObj.SetState(CardState.N);
                ApplyCard(cardObj, pos);
            }
        }
    }

    public class SeatRight : Seat
    {
        public SeatRight()
        {
            trans = GameObject.Find("Content/PlayerAI1").transform;
            pengPos = GameObject.Find("Content/PlayerAI1/Peng").transform.position;
        }

        protected override Vector3 GetPengPosOffset()
        {
            return new Vector3(0, 1f, 0);
        }

        protected override Vector3 GetDropCard()
        {
            return trans.position + new Vector3(-1.5f, 2f, 0);
        }

        public override void FreshCard(List<Card> list, int index)
        {
            Vector3 pos = trans.position + new Vector3(0, list.Count * -0.25f, 0);

            if (index >= 0)
            {
                MCard cardObj = GetCardObj(list[index]);
                cardObj.SetState(CardState.Right);

                pos.y += 0.4f * (index + 1);
                ApplyCard(cardObj, pos);

                return;
            }

            foreach (var item in list)
            {
                MCard cardObj = GetCardObj(item);
                cardObj.SetState(CardState.Right);

                pos.y += 0.4f;

                ApplyCard(cardObj, pos);
            }
        }

        public override void DrawCard(Card card)
        {
            Vector3 pos = trans.position + new Vector3(-2f, 3.3f, 0);

            MCard cardObj = GetCardObj(card);
            cardObj.SetState(CardState.Right);
            ApplyCard(cardObj, pos + new Vector3(2, 0, 0));
        }
    }

    public class SeatOppsite : Seat
    {
        public SeatOppsite()
        {
            trans = GameObject.Find("Content/PlayerAI2").transform;
            pengPos = GameObject.Find("Content/PlayerAI2/Peng").transform.position;
        }

        protected override Vector3 GetPengPosOffset()
        {
            return new Vector3(-2.1f, 0, 0);
        }

        protected override Vector3 GetDropCard()
        {
            return trans.position + new Vector3(-5.5f, -1.2f, 0);
        }

        public override void FreshCard(List<Card> list, int index)
        {
            Vector3 pos = trans.position + new Vector3(list.Count * -0.5f, 0, 0);

            if (index >= 0)
            {
                MCard cardObj = GetCardObj(list[index]);
                pos.x += 0.8f * (index + 1);
                cardObj.SetState(CardState.Opp);
                ApplyCard(cardObj, pos);

                return;
            }

            foreach (var item in list)
            {
                MCard cardObj = GetCardObj(item);
                cardObj.SetState(CardState.Opp);
                pos.x += 0.8f;

                ApplyCard(cardObj, pos);
            }
        }

        public override void DrawCard(Card card)
        {
            Vector3
                pos = trans.position + new Vector3(-9f, 0, 0);

            MCard cardObj = GetCardObj(card);
            cardObj.SetState(CardState.Opp);
            ApplyCard(cardObj, pos + new Vector3(2, 0, 0));
        }
    }

    public class SeatLeft : Seat
    {
        public SeatLeft()
        {
            trans = GameObject.Find("Content/PlayerAI3").transform;
            pengPos = GameObject.Find("Content/PlayerAI3/Peng").transform.position;
        }

        protected override Vector3 GetPengPosOffset()
        {
            return new Vector3(0f, -1f, 0);
        }

        protected override Vector3 GetDropCard()
        {
            return trans.position + new Vector3(2f, -2.5f, 0);
        }

        public override void FreshCard(List<Card> list, int index)
        {
            Vector3 pos = trans.position + new Vector3(0, list.Count * -0.25f, 0);

            if (index >= 0)
            {
                MCard cardObj = GetCardObj(list[index]);
                pos.y += 0.4f * (index + 1);
                cardObj.SetState(CardState.Left);
                ApplyCard(cardObj, pos);

                return;
            }

            foreach (var item in list)
            {
                MCard cardObj = GetCardObj(item);

                pos.y += 0.4f;
                cardObj.SetState(CardState.Left);
                ApplyCard(cardObj, pos);
            }
        }

        public override void DrawCard(Card card)
        {
            MCard cardObj = GetCardObj(card);
            Vector3
                pos = trans.position + new Vector3(-2f, -3.8f, 0);
            cardObj.SetState(CardState.Left);

            ApplyCard(cardObj, pos + new Vector3(2, 0, 0));
        }
    }
}