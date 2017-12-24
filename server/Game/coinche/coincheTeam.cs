using Mina.Core.Session;
using System.Collections.Generic;
using System;

namespace server
{
	public class coincheTeam : ITeam
	{
		private int score = 0;
		private int scoreRound;
		private List<coinchePlayer> players = new List<coinchePlayer>();

		public virtual bool AddPlayer(IPlayer player)
		{
			if (players.Count == 2)
			{
				return (false);
			}
			players.Add((coinchePlayer)player);
			return (true);
		}

		public virtual bool DeletePlayer(IoSession session)
		{
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].Session == session)
				{
					players.RemoveAt(i);
					return (true);
				}
			}
			return (false);
		}

		public virtual coinchePlayer GetPlayer(IoSession session)
		{
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].Session.Equals(session))
				{
					return (players[i]);
				}
			}
			return (null);
		}

		public virtual int GetPlayerIndex(IoSession session)
		{
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].Session.Equals(session))
				{
					return (i);
				}
			}
			return (-1);
		}

		public virtual coinchePlayer GetPlayer(int num)
		{
			return (players[num]);
		}

		public virtual void ResetDeck()
		{
			players[0].resetDeck();
			players[1].resetDeck();
		}

		public virtual void StartRound()
		{
			scoreRound = 0;
		}

		public virtual void ResetPlayed()
		{
			players[0].Played = false;
			players[1].Played = false;
			players[0].PlayedCard = null;
			players[1].PlayedCard = null;
		}

		public virtual void EndRound(int tmp)
		{
			score += tmp;
		}

		public virtual void SendStart()
		{
			IoSession tmp;

			tmp = players[0].Session;
			tmp.Write("0004 " + tmp.Id + ";" + players[1].Session.Id);
			tmp = players[1].Session;
			tmp.Write("0004 " + tmp.Id + ";" + players[0].Session.Id);
		}

		public virtual void SendMessage(string message)
		{
			players[0].Session.Write(message);
			players[1].Session.Write(message);
		}

		public virtual void SendEnd()
		{
			if (players[0].Session.Connected)
			{
				players[0].Session.Write("0000");
			}
			if (players[1].Session.Connected)
			{
				players[1].Session.Write("0000");
			}
		}

		public virtual int GetPlayerCount()
		{
		    return (players.Count);
		}

		public virtual void appendScore(int tmp)
		{
				score += tmp;
		}

		public virtual int Score
		{
			get
			{
				return (score);
			}
		}

		public virtual int ScoreRound
		{
			get
			{
				return (scoreRound);
			}
		}
    }
}