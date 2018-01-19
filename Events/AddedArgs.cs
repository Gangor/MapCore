using System;

namespace MapCore.Events
{
	public class AddedArgs
	{
		public object Sender { get; set; }
		public Type Type { get; set; }

		public AddedArgs(object sender, Type type)
		{
			Sender = sender;
			Type = type;
		}
	}
}
