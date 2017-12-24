namespace client.Card
{
    public abstract class ACard
    {
        private int _value;
        private string _color;

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public abstract void Dump();
    }
}