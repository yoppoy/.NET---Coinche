using System;
using Mina.Core.Session;

namespace server
{
	public interface    IPlayer
	{
		IoSession       Session {get;}
		void            DistributeCard(Card card);
	}

}