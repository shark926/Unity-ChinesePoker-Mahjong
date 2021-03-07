using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
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
}