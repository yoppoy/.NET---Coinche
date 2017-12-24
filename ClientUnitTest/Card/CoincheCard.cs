using System;

namespace client.Card
{
    public class CoincheCard : ACard
    {
        public override void Dump()
        {
            string val = Value.ToString();
            if (val.Contains("11"))
            {
                val = "Jack";
            }
            else if (val.Contains("12"))
            {
                val = "Queen";
            }
            else if (val.Contains("13"))
            {
                val = "King";
            }
            else if (val.Contains("14"))
            {
                val = "Ace";
            }
            Console.Write("[");
            Console.Write(Color);
            Console.Write("] - [");
            Console.Write(val);
            Console.WriteLine("]");
        }
    }
}