using System;
using Mina.Core.Session;

namespace server
{
	public class coinchePlayer : IPlayer
	{
		private IoSession session;
		private coincheDeck deck = new coincheDeck();
		private bool bid = false;
		private bool played;
		private Card playedCard = null;

		public coinchePlayer(IoSession added)
		{
			session = added;
		}

		public virtual void DistributeCard(Card card)
		{
			deck.ReceiveCard(card);
		}

		public virtual IoSession Session
		{
			get
			{
				return (session);
			}
		}

		public virtual void resetDeck()
		{
			deck.Clear();
		}

		public virtual Card PlayedCard
		{
			set
			{
				playedCard = value;
			}
			get
			{
				return (playedCard);
			}
		}

		public virtual bool Played
		{
			set
			{
				played = value;
			}
		}

		public virtual bool hasPlayed()
		{
			return (played);
		}

		public virtual bool Bid
		{
			set
			{
				bid = value;
			}
		}

		public virtual bool hasBid()
		{
			return (bid);
		}

	}

}