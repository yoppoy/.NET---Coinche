using System;
using Mina.Core.Session;

namespace server
{
	public interface IGame
	{
		int             PlayersLeft();
		void            AddPlayer(IPlayer added);
		bool            DeletePlayer(IoSession session);
		IPlayer         GetPlayer(IoSession session);
		void            AcceptCommand(string command, IoSession session);
        bool            GetStart();
		void            Start();
		void            End();
	}

}