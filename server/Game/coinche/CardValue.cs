using System;

namespace server
{
	public class CardValue
	{
		private int [] values = new int[] {7, 8, 9, 10, 11, 12, 13, 14};
		private int value = -1;

		public virtual void SetValue(int nb)
		{
			value = values[nb % 8];
		}

		public virtual int GetValue()
		{
			return value;
		}
	}

}