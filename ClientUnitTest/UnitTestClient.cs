using System;
using System.Runtime.CompilerServices;
using client.Card;
using client.Game;
using client.PlayerHand;
using Mina.Core.Session;

namespace ClientUnitTest
{
    public class UnitTestClient
    {
        private PlayerHand _hand = new PlayerHand();
        private IGame _g = new CoincheGame();
        private IoSession _s = null;
        
        public void Run()
        {
            this.TestPlayerHand();
            this.TestPlayerCommand();
        }

        private void TestPlayerCommand()
        {
            string mess = "0008 {\"Color\":\"club\",\"Value\":8}";
            string mess2 = "0008 {\"Color\":\"club\",\"Value\":9}";
            string mess3 = "0008 {\"Color\":\"club\",\"Value\":10}";

            Console.WriteLine();
            Console.WriteLine("Let's try the players commands");
            Console.WriteLine("Let's add some card in your hand");
            Console.WriteLine("Here are the card to add :");
            Console.WriteLine(mess);
            Console.WriteLine(mess2);
            Console.WriteLine(mess3);
            string[] input = mess.Split(new Char[] { ' ' });
            string[] input2 = mess2.Split(new Char[] { ' ' });
            string[] input3 = mess3.Split(new Char[] { ' ' });
            this._g.ParseServerInput(input, _s);
            this._g.ParseServerInput(input2, _s);
            this._g.ParseServerInput(input3, _s);
            
            Console.WriteLine();
            Console.WriteLine("Testing basics commands (not good state)");
            string mess4 = "/gameinfo";
            string mess5 = "/playerinfo";
            string mess6 = "/bid spade 100";
            string mess7 = "/pass";
            string mess8 = "/play club 8";
            string[] input4 = mess4.Split(new Char[] { ' ' });
            string[] input5 = mess5.Split(new Char[] { ' ' });
            string[] input6 = mess6.Split(new Char[] { ' ' });
            string[] input7 = mess7.Split(new Char[] { ' ' });
            string[] input8 = mess8.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess4);
            this._g.ParseClientInput(input4, _s);
            Console.WriteLine("Send command :" + mess5);
            this._g.ParseClientInput(input5, _s);
            Console.WriteLine("Send command :" + mess6);
            this._g.ParseClientInput(input6, _s);
            Console.WriteLine("Send command :" + mess7);
            this._g.ParseClientInput(input7, _s);
            Console.WriteLine("Send command :" + mess8);
            this._g.ParseClientInput(input8, _s);

            Console.WriteLine();
            Console.WriteLine("Changing state");
            string mess9 = "0004 1;2";
            string[] input9 = mess9.Split(new Char[] { ' ' });
            string mess10 = "0020";
            string[] input10 = mess10.Split(new Char[] { ' ' });
            this._g.ParseServerInput(input9, _s);
            this._g.ParseServerInput(input10, _s);
            
            Console.WriteLine();
            Console.WriteLine("Now trying /bid (not enought arguments)");
            string mess11 = "/bid ";
            string[] input11 = mess11.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess11);
            this._g.ParseClientInput(input11, _s);
            Console.WriteLine("Now trying /bid (bad arguments)");
            string mess12 = "/bid hicks 100";
            string[] input12 = mess12.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess12);
            this._g.ParseClientInput(input12, _s);
            string mess13 = "/bid spade toto";
            string[] input13 = mess13.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess13);
            this._g.ParseClientInput(input13, _s);

            
            string mess18 = "7000 1";
            string[] input18 = mess18.Split(new Char[] { ' ' });
            this._g.ParseServerInput(input18, _s);
            this._g.ParseServerInput(input10, _s);
            Console.WriteLine("Now trying /play (not enought arguments)");
            string mess14 = "/play ";
            string[] input14 = mess14.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess14);
            this._g.ParseClientInput(input14, _s);
            Console.WriteLine("Now trying /play (bad arguments)");
            string mess15 = "/play hicks 8";
            string[] input15 = mess15.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess15);
            this._g.ParseClientInput(input15, _s);
            string mess16 = "/play club 100";
            string[] input16 = mess16.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess16);
            this._g.ParseClientInput(input16, _s);
            Console.WriteLine("Now trying /play (bad card)");
            string mess17 = "/play spike 10";
            string[] input17 = mess17.Split(new Char[] { ' ' });
            Console.WriteLine("Send command :" + mess17);
            this._g.ParseClientInput(input17, _s);
        }

        private void TestPlayerHand()
        {
            ACard c1 = new CoincheCard();
            ACard c2 = new CoincheCard();
            ACard c3 = new CoincheCard();
            ACard b1 = new CoincheCard();
            
            Console.WriteLine("Testing the player's hand");
            Console.WriteLine("Adding new cards in the hand");
            c1.Color = "club";
            c1.Value = 8;
            c2.Color = "club";
            c2.Value = 7;
            c3.Color = "club";
            c3.Value = 9;
            b1.Color = "spike";
            b1.Value = 12;
            c1.Dump();
            c2.Dump();
            c3.Dump();
            
            Console.WriteLine("Looking for card in your hand (invalid ones)");
            _hand.AddCard(c1);
            _hand.AddCard(c2);
            _hand.AddCard(c3);
            if (_hand.FindCard(19, "heart") != null)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            if (_hand.FindCard(9, "spade") != null)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            if (_hand.FindCard(8, "diamond") != null)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            
            Console.WriteLine("Looking for card in your hand (good ones)");
            if (_hand.FindCard(8, "club") == null)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            if (_hand.FindCard(9, "club") == null)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            if (_hand.FindCard(7, "club") == null)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            
            Console.WriteLine("Deleting cards in your hand (invalid ones)");
            if (_hand.DeleteCard(b1) != false)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            
            Console.WriteLine("Deleting cards in your hand (valid ones)");
            Console.WriteLine("Dumping hand");
            _hand.Dump();
            if (_hand.DeleteCard(c1) == false)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            if (_hand.DeleteCard(c2) == false)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            if (_hand.DeleteCard(c3) == false)
                Console.WriteLine("KO!");
            else
                Console.WriteLine("OK!");
            Console.WriteLine("Re - dumping hand :");
            _hand.Dump();
            Console.WriteLine("{empty}");
        }
    }
}