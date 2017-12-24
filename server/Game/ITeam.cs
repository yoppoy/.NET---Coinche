using System;
using Mina.Core.Session;

namespace server
{
    public interface    ITeam
	{
		bool            AddPlayer(IPlayer player);
		void            SendStart();
		void            SendEnd();
		void            SendMessage(string message);
		bool            DeletePlayer(IoSession session);
	}

}