using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
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