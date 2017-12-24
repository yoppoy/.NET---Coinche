namespace server
{
	public class Card
	{
		private CardColor _color = new CardColor();
		private CardValue _value = new CardValue();

		public Card(int nb)
		{
			_color.SetColor(nb);
			_value.SetValue(nb);
		}

		public virtual void Dump()
		{
			string val = _value.GetValue().ToString();
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
            System.Console.WriteLine("[" + _color.GetColor() + "] - [" + val + "]");
		}

		public virtual int Value
		{
			get
			{
				return (_value.GetValue());
			}
			set
			{
				_value.SetValue(value);
			}
		}

		public virtual string Color
		{
			get
			{
				return (_color.GetColor());
			}
			set { _color.SetColor(value); }
		}
	}
}