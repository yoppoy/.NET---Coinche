using System;
using Mina.Core.Session;

namespace client.Game
{
    public interface IGame
    {
        void ParseClientInput(String [] input, IoSession session);

        void ParseServerInput(String [] input, IoSession session);

        void SessionOpen(IoSession session);

        void DisplayHelp();
    }
}
