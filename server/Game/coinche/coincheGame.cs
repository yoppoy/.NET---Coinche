using System;
using System.Collections.Generic;
using Mina.Core.Session;
using Newtonsoft.Json;

namespace server
{
	public class CoincheGame : IGame
	{
		private coincheTeam[]   teams = new coincheTeam[2];
		private bool            hasEnded = false;
		private int             roundBid;
		private coinchePlayer   roundBidder;
		private int             bidPass;
		private char            roundState = (char)3;
		private int             roundNumber = 0;
		private CardColor       roundColor = null;
		private coincheDeck     roundDeck = new coincheDeck();
		private int             playNumber;
		private coinchePlayer   currentPlayer;

		public CoincheGame()
		{
			teams[0] = new coincheTeam();
			teams[1] = new coincheTeam();
		}

		public virtual void Start()
		{
			Console.WriteLine("---> A new GAME has STARTED");
			teams[0].SendStart();
			teams[1].SendStart();
			StartRound();
		}

		public virtual void End()
		{
			teams[0].SendEnd();
			teams[1].SendEnd();
			hasEnded = true;
			Console.WriteLine("---> GAME ENDED<---");
		}

		public virtual void StartRound()
		{
			roundState = (char)3;
			Console.WriteLine("------> A new ROUND has STARTED");
			SendMessage("0007 " + roundNumber);
			teams[0].StartRound();
			teams[1].StartRound();
			playNumber = 0;
			roundBid = 0;
			roundDeck.Fill();
			DistributeDeck();
			StartBidding();
		}

		public virtual void EndRound()
		{
            int             index;

            index = GetPlayerTeam(roundBidder.Session);
            if (teams[index].Score >= roundBid)
            {
                teams[index].EndRound(teams[index].ScoreRound + roundBid);
                teams[(index == 0) ? 1 : 0].EndRound(teams[(index == 0) ? 1 : 0].ScoreRound);
                Console.Write("Bidding Team " + index + " won the round !\n");
            }
            else
            {
                teams[index].EndRound(0);
                teams[(index == 0) ? 1 : 0].appendScore(roundBid + 160);
                Console.Write("Bidding Team " + index + " lost the round !\n");
            }
            roundNumber++;
			Console.Write("------> Round OVER : {0:D} - {1:D}\n", teams[0].Score, teams[1].Score);
            teams[0].SendMessage("0077 " + teams[0].Score + " " + teams[1].Score);
            teams[1].SendMessage("0077 " + teams[1].Score + " " + teams[0].Score);
			if ((teams[0].Score >= 500 || teams[1].Score >= 500) || roundNumber == 8)
			{
				if (teams[0].Score > teams[1].Score)
				{
                    Console.Write("Team 0 won the game !\n");
					teams[0].SendMessage("0777 1");
					teams[1].SendMessage("0777 0");
				}
				else
				{
                    Console.Write("Team 1 won the game !\n");
                    teams[0].SendMessage("0777 0");
					teams[1].SendMessage("0777 1");
				}
				End();
			}
			else
			{
				StartRound();
			}
		}

		public virtual void StartBidding()
		{
			Console.WriteLine("---------> Bidding STARTS");
			roundState = (char)0;
			bidPass = 0;
			teams[0].ResetPlayed();
			teams[1].ResetPlayed();
			roundBid = 0;
			roundBidder = null;
			roundColor = new CardColor();
			currentPlayer = (coinchePlayer)teams[0].GetPlayer(0);
			currentPlayer.Session.Write("0020");
		}

		public virtual void EndBidding()
		{
			currentPlayer = (coinchePlayer)teams[0].GetPlayer(0);
			if (roundBidder == null)
			{
				Console.WriteLine("------> ! RE-SHUFFLING !");
				SendMessage("8888");
				teams[0].ResetDeck();
				teams[1].ResetDeck();
				roundDeck.Clear();
				roundDeck.Fill();
				roundDeck.Shuffle();
				DistributeDeck();
				StartBidding();
			}
			else
			{
				Console.Write("---------> Bidding : {0}\n", roundColor.GetColor());
				SendMessage("7000");
				StartPlay();
			}
		}

		public virtual void DistributeDeck()
		{
			for (int i = 0; i < 8; i++)
			{
				DistributeCard(teams[0].GetPlayer(0));
				DistributeCard(teams[1].GetPlayer(0));
				DistributeCard(teams[0].GetPlayer(1));
				DistributeCard(teams[1].GetPlayer(1));
			}
		}

		public virtual void DistributeCard(IPlayer player)
		{
            Card            distributed;

            distributed = roundDeck.Distribute();
			player.DistributeCard(distributed);
			player.Session.Write("0008 " + JsonConvert.SerializeObject((distributed)));
		}

		public virtual void AcceptCommand(string cmd, IoSession session)
		{
			string code;

			Console.WriteLine("---------> Accepting Command : " + cmd);
			code = cmd.Substring(0, 4);
			if (roundState != (char)3)
			{
				switch (code)
				{
					case "0010" :
						AcceptBid(cmd, (coinchePlayer)GetPlayer(session));
						break;
					case "0012" :
						AcceptBid(null, (coinchePlayer)GetPlayer(session));
						break;
                    case "0011" :
                        AcceptCard(cmd.Substring(5), (coinchePlayer)GetPlayer(session));
                        break;
                    case "0100" :
                        SendToAllExcept("0110 Player " + GetPlayerIndex(session) + ": " + cmd.Substring(5));
                        break;
				}
			}
		}

		public virtual void AcceptBid(string bid, coinchePlayer player)
		{
			string[] arr = null;

			if (!string.ReferenceEquals(bid, null))
			{
				arr = bid.Split(" ", true);
			}
			if (currentPlayer == 
			    player 
			    && roundState == (char)0 
			    && string.ReferenceEquals(bid, null) 
			    || (arr.Length == 3))
			{
				if (!string.ReferenceEquals(bid, null))
				{
					Console.Write("Setting round Color {0}\n", arr[1]);
					roundColor.SetColor(arr[1]);
					roundBid = int.Parse(arr[2]);
					roundBidder = player;
					SendToAllExcept("7777 " + currentPlayer.Session.Id + " " + arr[1] + " " + arr[2]);
					currentPlayer.Bid = true;
				}
				else
				{
					SendToAllExcept("7778 " + currentPlayer.Session.Id);
					bidPass++;
				}
				currentPlayer.Played = true;
				if ((roundBidder != null && bidPass == 3) || bidPass == 4)
				{
					EndBidding();
				}
				else
				{
					UpdateCurrentPlayer();
					while (currentPlayer.hasBid() == false && currentPlayer.hasPlayed() == true)
					{
						UpdateCurrentPlayer();
					}
					currentPlayer.Session.Write("0020");
				}
			}
		}

		public virtual void StartPlay()
		{
			Console.WriteLine("---------> Starting Play");
			teams[0].ResetPlayed();
			teams[1].ResetPlayed();
			roundState = (char)1;
			currentPlayer.Session.Write("0020");
		}

		public virtual void EndPlay()
		{
			playNumber++;
			currentPlayer = PlayWinner;
			UpdateRoundScore();
			if (playNumber == 8)
			{
				EndRound();
			}
			else
			{
				StartPlay();
			}
		}

		public virtual void AcceptCard(string cardData, coinchePlayer player)
		{
			Card card;

			if (player == currentPlayer && roundState == (char)1)
			{
				Console.WriteLine(cardData);
                card = JsonConvert.DeserializeObject<Card>(cardData);			
                roundDeck.ReceiveCard(card);
				SendToAllExcept("0088 " + currentPlayer.Session.Id + " " + cardData);
				currentPlayer.PlayedCard = card;
				currentPlayer.Played = true;
				if (UpdateCurrentPlayer() == false)
				{
					EndPlay();
				}
				else
				{
					currentPlayer.Session.Write("0020");
				}
			}
		}

		public virtual bool UpdateCurrentPlayer()
		{
		   int team;
		   int index;
		   int new_team;
		   int new_index;

		   team = GetPlayerTeam(currentPlayer.Session);
		   index = GetPlayerIndex(currentPlayer.Session);
		   if (teams[team].GetPlayer(index).hasPlayed() == true)
		   {
			   new_team = (team == 0) ? 1 : 0;
			   if ((index == 0 && team == 1) || (index == 1 && team == 0))
			   {
				   new_index = 1;
			   }
			   else
			   {
				   new_index = 0;
			   }
			   Console.Write("New player Team:{0:D} Index: {1:D}\n", new_team, new_index);
			   currentPlayer = teams[new_team].GetPlayer(new_index);
			   if (teams[new_team].GetPlayer(new_index).hasPlayed() == true)
			   {
				   return (false);
			   }
		   }
		   return (true);
		}

		public virtual void UpdateRoundScore()
		{
			int[] points = new int[] {0, 0, 0, 5, 2, 3, 4, 19};
			int[] pointsSuit = new int[] {0, 0, 9, 5, 14, 1, 3, 6};
			Card tmp;
			int score;
			int index;

			score = 0;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					tmp = teams[i].GetPlayer(j).PlayedCard;
					if (string.ReferenceEquals(tmp.Color, roundColor.GetColor()))
					{
						score += pointsSuit[tmp.Value - 7];
					}
					else
					{
						score += points[tmp.Value - 7];
					}
				}
			}
			Console.Write("---------> Round winner : {0:D}\n", PlayWinner.Session.Id);
			Console.Write("--------->       Card   : {0} {1:D}\n", teams[0].GetPlayer(0).PlayedCard.Color, teams[0].GetPlayer(0).PlayedCard.Value);
			Console.Write("--------->       Card   : {0} {1:D}\n", teams[0].GetPlayer(1).PlayedCard.Color, teams[0].GetPlayer(1).PlayedCard.Value);
			Console.Write("--------->       Card   : {0} {1:D}\n", teams[1].GetPlayer(0).PlayedCard.Color, teams[1].GetPlayer(0).PlayedCard.Value);
			Console.Write("--------->       Card   : {0} {1:D}\n", teams[1].GetPlayer(1).PlayedCard.Color, teams[1].GetPlayer(1).PlayedCard.Value);
			index = GetPlayerTeam(PlayWinner.Session);  
			teams[index].SendMessage("0006 " + score);
			teams[(index == 0) ? 1 : 0].SendMessage("0006 " + 0);
			teams[index].appendScore(score);
			//TODO : UPDATE SCORE ACCORDING TO WINNER
		}

		public virtual void SendMessage(string message)
		{
			teams[0].SendMessage(message);
			teams[1].SendMessage(message);
		}

		public virtual void SendToAllExcept(string message)
		{
			int index = GetPlayerIndex(currentPlayer.Session);
			int team = GetPlayerTeam(currentPlayer.Session);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					if (i != index || j != team)
					{
						teams[j].GetPlayer(i).Session.Write(message);
					}
				}
			}
		}

		public virtual int PlayersLeft()
		{
			return (4 - (teams[0].GetPlayerCount() + teams[1].GetPlayerCount()));
		}


		public virtual bool IsBigger(Card one, Card two)
		{
			if (one.Color.Equals(roundColor.GetColor()) && !one.Color.Equals(two.Color))
			{
				return (true);
			}
			else if (one.Color.Equals(roundColor.GetColor()) && one.Color.Equals(two.Color))
			{
			   return ((one.Value > two.Value) ? true : false);
			}
			else if (one.Color.Equals(two.Color))
			{
				return ((one.Value > two.Value) ? true : false);
			}
			return (false);
		}


		public virtual void AddPlayer(IPlayer added)
		{
			if (teams[0].AddPlayer(added) == false)
			{
				teams[1].AddPlayer(added);
				Console.WriteLine("Player added to team 1");
			}
			else
			{
				Console.WriteLine("Player added to team 0");
			}
			if (PlayersLeft() != 0)
			{
				added.Session.Write("0044");
			}
			else
			{
				Start();
			}
		}

		public virtual bool DeletePlayer(IoSession session)
		{
			Console.WriteLine("Deleting player");
			if (roundState == (char)3)
			{
				Console.WriteLine("Player disconnected : Deleting player");
				if (teams[0].DeletePlayer(session) == false)
				{
					teams[1].DeletePlayer(session);
				}
				return (false);
			}
			else
			{
				End();
			}
			return (true);
		}

		public virtual IPlayer GetPlayer(IoSession session)
		{
			IPlayer tmp;

			if ((tmp = teams[0].GetPlayer(session)) == null)
			{
				return (teams[1].GetPlayer(session));
			}
			return (tmp);
		}

		public virtual int GetPlayerIndex(IoSession session)
		{
			int tmp;

			if ((tmp = teams[0].GetPlayerIndex(session)) == -1)
			{
				return (teams[1].GetPlayerIndex(session));
			}
			return (tmp);
		}

		public virtual int GetPlayerTeam(IoSession session)
		{
		   if ((teams[0].GetPlayer(session)) == null)
		   {
				return (1);
		   }
			return (0);
		}

		public virtual coinchePlayer PlayWinner
		{
			get
			{
				coinchePlayer tmp;
				coinchePlayer winner;
    
				winner = teams[0].GetPlayer(0);
				tmp = teams[0].GetPlayer(1);
				if (IsBigger(tmp.PlayedCard, winner.PlayedCard))
				{
					winner = teams[0].GetPlayer(1);
				}
				for (int i = 0; i < 2; i++)
				{
					tmp = teams[1].GetPlayer(i);
					if (IsBigger(tmp.PlayedCard, winner.PlayedCard))
					{
						winner = teams[1].GetPlayer(i);
					}
				}
				return (winner);
			}
		}

		public virtual bool GetStart()
		{
			if (roundState == (char)3)
				{
					return (false);
				}
			return (true);
		}
	}

}