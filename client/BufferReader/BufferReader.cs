using System;
using client.Game;
using Mina.Core.Session;

namespace client.BufferReader
{
    class BufferReaderClass
    {
        async public void ReadInput(IoSession s, IGame g)
        {
            while (true)
            {
                String r = Console.ReadLine();
                String [] input = r.Split(new Char[] { ' ' });
                if (input[0].Equals("/help"))
                    g.DisplayHelp();
                else
                    g.ParseClientInput(input, s);
            }
        }
    }
}
