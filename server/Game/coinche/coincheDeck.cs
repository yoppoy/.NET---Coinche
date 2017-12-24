using System;
using System.Collections.Generic;

namespace server
{
    public class coincheDeck
	{
		private List<Card> cards = new List<Card>();

		public virtual void Fill()
		{
			for (int i = 0; i < 32; ++i)
			{
				cards.Add(new Card(i));
			}
			Shuffle();
		}

		public virtual void ReceiveCard(Card card)
		{
			cards.Add(card);
		}

		public virtual void Shuffle()
		{
            List<Card> shuffled = new List<Card>();

            Random r = new Random();
            int randomIndex = 0;
            while (cards.Count > 0)
            {
                randomIndex = r.Next(0, cards.Count);
                shuffled.Add(cards[randomIndex]);
                cards.RemoveAt(randomIndex);
            }
            cards = shuffled;
        }

		public virtual Card Distribute()
		{
            Card back;

            back = cards[0];
            cards.RemoveAt(0);           
            return (back);
		}

		public virtual Card Get(int a)
		{
			return cards[a];
		}

		public virtual void Clear()
		{
			cards.Clear();
		}

		public virtual List<Card> List
		{
			get
			{
				return cards;
			}
		}
	}

}