using Mina.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using client.Connexion;
using client.Game;

namespace client
{
    static class Client
    {
        public static void Main(string[] args)
        {
            IGame g = new CoincheGame();
            MinaConnexion c = new MinaConnexion();
            c.Connect("127.0.0.1", 8000, g);
            Console.ReadLine();
        }
    }
}
