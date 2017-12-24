using System;

namespace server
{
    class Server
    {
        private static readonly Int32 port = 8000;
        static void Main(string[] args)
        {
            ApacheMinaConnection connection = new ApacheMinaConnection();
            connection.run(port);
        }
    }
}
