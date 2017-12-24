using System;
using System.Net;
using System.Text;
using client.BufferReader;
using client.Game;
using Mina.Core.Future;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Codec.TextLine;
using Mina.Filter.Logging;
using Mina.Transport.Socket;
using Common.Logging;
using System.Configuration;

namespace client.Connexion
{

    public class MinaConnexion : IConnexion
    {

        private IoSession _s = null;
        private IGame _g;

        public IoSession Session { get => _s; set => _s = value; }

        public bool Connect(string ip, int port, IGame game)
        {
            IoConnector connector = new AsyncSocketConnector();
            this._g = game;
            connector.FilterChain.AddLast("logger", new LoggingFilter());
            connector.FilterChain.AddLast("codec", new ProtocolCodecFilter(new TextLineCodecFactory(Encoding.UTF8)));
            byte[] ipB = Encoding.ASCII.GetBytes(ip);
            IPAddress ipO = IPAddress.Parse(ip);
            
            // Configure the service.
            connector.ConnectTimeoutInMillis = 30000;
            connector.SessionOpened += this.SessionOpen;
            connector.ExceptionCaught += this.ExceptionCatcher;
            connector.MessageReceived += this.MessageRcv;
            connector.SessionClosed += this.CloseConnection;
            BufferReaderClass br = new BufferReaderClass();
            while (true)
            {
                try
                {
                    IConnectFuture future = connector.Connect(new IPEndPoint(ipO, port));
                    future.Await();
                    this._s = future.Session;
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Waiting for the server");
                }
            }

            // wait until the summation is done
            br.ReadInput(this._s, this._g);   
            this._s.CloseFuture.Await();
            Console.WriteLine("Press any key to exit");
            Console.Read();
            return true;
        }

        private void CloseConnection(object sender, IoSessionEventArgs e)
        {
            Console.WriteLine("Server closed connection, now exiting");
            Environment.Exit(1);
        }

        public void SessionOpen(object s, IoSessionEventArgs e)
        {
            this._g.SessionOpen(this._s);
        }

        public void ExceptionCatcher(object s, IoSessionExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception);
        }

        public void MessageRcv(object s, IoSessionMessageEventArgs e)
        {
            string mess = (string)e.Message;
            string[] input = mess.Split(new Char[] { ' ' });
            this._g.ParseServerInput(input, this._s);
        }
    }
}
