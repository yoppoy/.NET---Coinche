using System;
using Mina.Core.Service;
using Mina.Core.Session;
using System.Collections.Generic;

namespace server
{
    class ApacheMinaHandler : IoHandlerAdapter
    {
        private List<IGame> games = new List<IGame>();
        private List<handlerPlayer> players = new List<handlerPlayer>();

        public override void ExceptionCaught(IoSession session, Exception cause)
        {
            Console.WriteLine(cause);
        }

        public override void MessageReceived(IoSession session, Object message)
        {
            string str = message.ToString();
            Console.WriteLine("Received ---> " + str);
            ParseCommand(str, session);
        }

        public virtual void ParseCommand(string str, IoSession session)
        {
            string code;

            if (str.Length >= 4)
            {
                code = str.Substring(0, 4);
                GetGamebySession(session).AcceptCommand(str, session);
            }
        }

        public override void SessionIdle(IoSession session, IdleStatus status)
        {
        }

        public override void SessionOpened(IoSession session)
        {
            Console.WriteLine("New player connected");
            AddPlayer(new coinchePlayer(session), session);
        }

        public override void SessionClosed(IoSession session)
        {
            Console.Write("CLIENT CLOSED {0}\n", session.Id);
            if (GetGamebySession(session).DeletePlayer(session) == true)
            {
                for (int i = 0; i < games.Count; i++)
                {
                    if (games[i] == GetGamebySession(session))
                    {
                        games.RemoveAt(i);
                    }
                }
            }
        }

        public virtual IGame FreeGame
        {
            get
            {
                if (games.Count == 0)
                {
                    return (null);
                }
                for (int i = 0; i < games.Count; i++)
                {
                    if (games[i].PlayersLeft() > 0)
                    {
                        return (games[i]);
                    }
                }
                return (null);
            }
        }

        public virtual void AddPlayer(IPlayer player, IoSession session)
        {
            IGame tmp;

            session.Write("0002");
            if ((tmp = FreeGame) == null)
            {
                Console.WriteLine("New game created !");
                tmp = new CoincheGame();
                games.Add(tmp);
            }
            tmp.AddPlayer(player);
            players.Add(new handlerPlayer(session, tmp));
        }

        public virtual IGame GetGamebySession(IoSession session)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Session == session)
                {
                    return (players[i].Game);
                }
            }
            return (null);
        }

        public virtual IPlayer GetPlayerbySession(IoSession session)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Session == session)
                {
                    return (players[i].Game.GetPlayer(session));
                }
            }
            return (null);
        }
    }
}
