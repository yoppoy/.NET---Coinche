using System;
using Mina.Core.Session;

namespace server
{
	public class handlerPlayer
	{
		private IoSession session;
		private IGame game;

		public handlerPlayer(IoSession newSession, IGame newGame)
		{
			session = newSession;
			game = newGame;
		}

		public virtual IGame Game
		{
			set
			{
				game = value;
			}
			get
			{
				return (game);
			}
		}

		public virtual IoSession Session
		{
			set
			{
				session = value;
			}
			get
			{
				return (session);
			}
		}


	}

}