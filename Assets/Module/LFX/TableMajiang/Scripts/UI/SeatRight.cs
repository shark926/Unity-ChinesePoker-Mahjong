using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
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
}