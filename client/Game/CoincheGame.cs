using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using client.Card;
using Mina.Core.Session;
using Newtonsoft.Json;

namespace client.Game
{
    public class CoincheGame : IGame
    {
        private int _partnerId;
        private int _id;
        private int _lastBid = -1;
        private string _color = "";
        private PlayerState _pstate;
        private GameState _gstate;
        private PlayerHand.PlayerHand _hand;
        private int _score = 0;
        private int _ennemyScore = 0;
        private List<ACard> _fold;

        private enum PlayerState
        {
            PlayerTurn,
            OtherTurn
        }
        
        private enum GameState
        {
            NotPlaying,
            BindingPhase,
            Playing
        }
        
        public CoincheGame()
        {
            _pstate = PlayerState.OtherTurn;
            _gstate = GameState.NotPlaying;
            _partnerId = -1;
            _hand = new PlayerHand.PlayerHand();
            _fold = new List<ACard>();
        }
      
        public void ParseClientInput(string[] input, IoSession session)
        {
            if (input[0].Equals("/hand"))
                this.DisplayHand();
            else if (input[0].Equals("/playerinfo"))
                this.DisplayPlayerInfo();
            else if (input[0].Equals("/pass"))
                this.HandlePassAction(session);
            else if (input[0].Equals("/bid"))
                this.HandleBidAction(input, session);
            else if (input[0].Equals("/gameinfo"))
                this.HandleGameInfo();
            else if (input[0].Equals("/play"))
                this.HandlePlayAction(input, session);
            else if (input[0].Equals("/fold"))
                this.HandleFoldAction();
            else
                Console.WriteLine("Unknown command " + input[0] + ", use /help to have further informations");
        }

        private void HandleFoldAction()
        {
            if (_gstate.Equals(GameState.Playing))
            {
                Console.WriteLine("Current fold is :");
                foreach (var card in _fold)
                    card.Dump();
            }
            else
                Console.WriteLine("Actually not playing");
        }

        private int getValue(string s)
        {
            int value;
            switch (s)
            {
                case "queen":
                    value = 12;
                    break;
                case "Queen":
                    value = 12;
                    break;
                case "King":
                    value = 13;
                    break;
                case "king":
                    value = 13;
                    break;
                case "jack":
                    value = 11;
                    break;
                case "Jack":
                    value = 11;
                    break;
                case "Ace":
                    value = 14;
                    break;
                case "ace":
                    value = 14;
                    break;
                default:
                    value = 0;
                    break;
            }
            return (value);
        }
        
        private void HandlePlayAction(string[] input, IoSession session)
        {
            if (_gstate.Equals(GameState.Playing) && _pstate.Equals(PlayerState.PlayerTurn))
            {
                if (input.Length == 2)
                    this.PlayCardNumber(input, session);
                else if (input.Length != 3)
                    Console.WriteLine("Not enought argument, use /help to have further informations");
                else
                {
                    string color = input[1];
                    int value = this.getValue(input[2]);

                    if (value.Equals(0))
                    {
                        bool res = int.TryParse(input[2], out value);
                        if (!res)
                        {
                            Console.WriteLine("Second argument is not a number, run /help");
                            return;
                        }

                    }
                    if (!color.Equals("spike") && !color.Equals("spade")
                        && !color.Equals("club") && !color.Equals("heart"))
                        Console.WriteLine("Color is not good, run /help");
                    else if (value < 7 || value > 14)
                        Console.WriteLine("Value is not good, run with /help");
                    else
                    {
                        ACard card = _hand.FindCard(value, color);
                        if (card == null)
                            Console.WriteLine("Cannot find the card in your hand, use /hand");
                        else
                        {
                            _hand.DeleteCard(card);
                            session.Write("0011 " + JsonConvert.SerializeObject(card));
                            _pstate = PlayerState.OtherTurn;
                        }
                    }
                }
            }
            else
                Console.WriteLine("Not playing or not your turn");
        }

        private void PlayCardNumber(string[] input, IoSession session)
        {
            int value = 0;
            bool res = int.TryParse(input[1], out value);
            if (!res)
                Console.WriteLine("Second argument is not a number, run /help");
            else if (value > _hand.GetSize())
                Console.WriteLine("You don't have enought card in your hand");
            else
            {
                ACard card = _hand.FindCard(value);
                session.Write("0011 " + JsonConvert.SerializeObject(card));
                _hand.DeleteCard(value);
                _pstate = PlayerState.OtherTurn;
            }
        }

        private void HandleGameInfo()
        {
            if (!_gstate.Equals(GameState.NotPlaying))
            {
                Console.WriteLine("Actual bid is " + _lastBid.ToString() + " on color " + _color);
                Console.WriteLine("Your total score is " + _score);
                Console.WriteLine("Ennemy score is " + _ennemyScore);
            }
            else
                Console.WriteLine("Actually not playing");
        }

        private void HandleBidAction(string[] input, IoSession session)
        {
            if (_gstate == GameState.BindingPhase && _pstate == PlayerState.PlayerTurn)
            {
                if (input.Length != 3)
                    Console.WriteLine("Not enought argument, run /help");
                else
                {
                    string color = input[1];
                    int bid = 0;
                    bool res = int.TryParse(input[2], out bid);
                    if (!res)
                        Console.WriteLine("Second argument is not a number, run /help");
                    else if (!color.Equals("spike") && !color.Equals("spade")
                             && !color.Equals("club") && !color.Equals("heart"))
                        Console.WriteLine("Color is not good, run /help");
                    else if (bid < _lastBid || bid < 80 || bid > 160)
                        Console.WriteLine("Bad bid amount, run /help");
                    else
                    {
                        _lastBid = bid;
                        _color = color;
                        string toSend = "0010 " + color + " " + bid.ToString();
                        session.Write(toSend);
                        _pstate = PlayerState.OtherTurn;
                    }

                }
            }
            else
            {
                Console.WriteLine("Not playing or not your turn");
            }
        }

        private void HandlePassAction(IoSession session)
        {
            if (_gstate.Equals(GameState.BindingPhase) && _pstate.Equals(PlayerState.PlayerTurn))
            {
                session.Write("0012");
                _pstate = PlayerState.OtherTurn;
            }
            else
                Console.WriteLine("Actually not in Binding Phase or not your turn");
        }

        private void DisplayPlayerInfo()
        {
            if (_gstate.Equals(GameState.NotPlaying))
                Console.WriteLine("Actually not playing");
            else
            {
                Console.Write("Your ID is ");
                Console.WriteLine(this._id.ToString());
                Console.Write("Your partner ID is ");
                Console.WriteLine(this._partnerId.ToString());
            }
        }

        private void DisplayHand()
        {
            if (_hand.Size() < 1)
                Console.WriteLine("There is no card in your hand");
            else
                _hand.Dump();
        }

        public void ParseServerInput(string[] input, IoSession session)
        {
            this.ManagePlayerTurn(input[0]);
            if (input[0].Equals("0044"))
                Console.WriteLine("Waiting for other players");
            else if (input[0].Equals("0000"))
                Environment.Exit(0);
            else if (input[0].Equals("0004"))
            {
                Console.WriteLine("Let's coinche");
                this._gstate = GameState.BindingPhase;
                this.GetYourIdAndPartnerId(input);
            }
            else if (input[0].Equals("0007"))
                PrintNbRound(input);
            else if (input[0].Equals("0008"))
                this.GetCard(input);
            else if (input[0].Equals("8888"))
            {
                Console.WriteLine("Everyone has passed, now redistribuing cards");
                _lastBid = -1;
                this.DeleteHand();
            }
            else if (input[0].Equals("7778"))
                this.PrintPassAction(input);
            else if (input[0].Equals("7777"))
                this.PrintBidAction(input);
            else if (input[0].Equals("0077"))
                this.PrintEndOfRound(input);
            else if (input[0].Equals("7000"))
            {
                Console.WriteLine("You can play now !!");
                _gstate = GameState.Playing;
            }
            else if (input[0].Equals("0088"))
                this.PrintPlayAction(input);
            else if (input[0].Equals("0777"))
                this.PrintEndOfGame(input);
            else if (!input[0].Equals("0002") && !input[0].Equals("0020") && !input[0].Equals("0006"))
            {
                Console.WriteLine("---------------------");
                foreach (var s in input)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine("----------------------");
            }

        }

        private void PrintEndOfGame(string[] input)
        {
            int victory = int.Parse(input[1]);

            if (victory == 1)
                Console.WriteLine("Congrats you won !");
            else
                Console.WriteLine("You noob, you lose !");
        }

        private void PrintEndOfRound(string[] input)
        {
            this._hand.DeleteHand();
            this._fold.Clear();
            Console.WriteLine("End of the round");
            _gstate = GameState.BindingPhase;
            int point = int.Parse(input[1]);
            int opoint = int.Parse(input[2]);
            _score += point;
            _ennemyScore += opoint;
            this.HandleGameInfo();
        }

        private void PrintPlayAction(string[] input)
        {
            ACard card = JsonConvert.DeserializeObject<CoincheCard>(input[2]);
            int id = int.Parse(input[1]);
            if (id.Equals(_partnerId))
                Console.WriteLine("Your partner " + _partnerId.ToString() + " played " + card.Color + " " + card.Value);
            else
                Console.WriteLine("Player " + id.ToString() + " played " + card.Color + " " + card.Value);
            _fold.Add(card);
        }

        private void PrintBidAction(string[] input)
        {
            int id = int.Parse(input[1]);
            string color = input[2];
            int bid = int.Parse(input[3]);
            if (id.Equals(_partnerId))
                Console.WriteLine("Your partner " + id + " bid on " + color + " for " + bid.ToString());
            else
                Console.WriteLine("Player " + id + " bid on " + color + " for " + bid.ToString());
            _lastBid = bid;
            _color = color;
        }

        private void PrintPassAction(string[] input)
        {
            if (int.Parse(input[1]) == _partnerId)
            {
                Console.Write("Your partner ");
                Console.Write(_partnerId.ToString());
                Console.WriteLine(" has passed his turn");
            }
            else
            {
                Console.Write("The player ");
                Console.Write(input[1]);
                Console.WriteLine(" has passed");
            }
        }

        private void DeleteHand()
        {
            Console.WriteLine("Deleting hand");
            this._hand.DeleteHand();
        }

        private void ManagePlayerTurn(string s)
        {
            if (s.Equals("0020"))
            {
                Console.WriteLine("--------------");
                Console.WriteLine("It's your turn");
                _pstate = PlayerState.PlayerTurn;
                this.DisplayHand();
            }
            else
                _pstate = PlayerState.OtherTurn;
        }

        private void GetCard(string[] input)
        {
            ACard card = JsonConvert.DeserializeObject<CoincheCard>(input[1]);
            if (card != null)
            {
                Console.WriteLine("New card received :");
                card.Dump();
                _hand.AddCard(card);
            }
            else
            {
                Console.WriteLine("Error while receiving the new card (cannot deserialize)");
                Environment.Exit(1);
            }
        }

        private void GetYourIdAndPartnerId(string[] input)
        {
            string[] ids = input[1].Split(new char[] { ';' });
            _id = int.Parse(ids[0]);
            _partnerId = int.Parse(ids[1]);
            Console.Write("Your ID is ");
            Console.WriteLine(ids[0]);
            Console.Write("Your partner ID is ");
            Console.WriteLine(ids[1]);
        }

        private void PrintNbRound(string[] input)
        {
            Console.Write("Round number : ");
            Console.Write(input[1]);
            Console.WriteLine(" start");
        }

        public void SessionOpen(IoSession session)
        {
            Console.WriteLine("Connected to the server");
            Console.WriteLine("Game is coinche");
            session.Write("0001 Hello World");
        }

        public void DisplayHelp()
        {
            Console.WriteLine("Coinche game helper :");
            Console.WriteLine("/help : display this window");
        }
    }
}