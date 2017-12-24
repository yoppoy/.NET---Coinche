using System;

namespace server
{
	public class CardColor
	{
		private string[] colors = new string[] {"spade", "heart", "spike", "club"};
		private string color = null;

		public string GetColor()
		{
			return color;
		}

		public virtual void SetColor(int nb)
		{
			if (nb < 8)
			{
				color = colors[0];
			}
			else if (nb < 16)
			{
				color = colors[1];
			}
			else if (nb < 24)
			{
				color = colors[2];
			}
			else if (nb < 32)
			{
				color = colors[3];
			}
		}

		public virtual void SetColor(string Color)
		{
			if (Color.Equals(colors[0]))
			{
				color = colors[0];
			}
			else if (Color.Equals(colors[1]))
			{
				color = colors[1];
			}
			else if (Color.Equals(colors[2]))
			{
				color = colors[2];
			}
			else if (Color.Equals(colors[3]))
			{
				color = colors[3];
			}
			else
			{
                Console.Error.WriteLine("Error : invalid color : -%s-\n", Color);
			}
		}
	}

}