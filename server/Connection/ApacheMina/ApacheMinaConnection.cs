using System;
using System.Net;
using System.Text;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Codec.TextLine;
using Mina.Filter.Logging;
using Mina.Transport.Socket;

namespace server
{
    class ApacheMinaConnection : IConnection
    {
        public ApacheMinaConnection()
        {
            Console.WriteLine("Server type : Apacha Mina");
        }
        public void run(int port)
        {
            IoAcceptor acceptor = new AsyncSocketAcceptor();
            acceptor.FilterChain.AddLast("logger", new LoggingFilter());
            acceptor.FilterChain.AddLast("codec", new ProtocolCodecFilter(new TextLineCodecFactory(Encoding.UTF8)));
            acceptor.Handler = new ApacheMinaHandler();
            Console.WriteLine("-> Server running");
            acceptor.SessionConfig.ReadBufferSize = 2048;
            acceptor.SessionConfig.SetIdleTime(IdleStatus.BothIdle, 10);
            acceptor.Bind(new IPEndPoint(IPAddress.Any, port));
            Console.ReadLine();
        }   
    }
}
