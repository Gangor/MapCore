using System;

namespace MapCore.Events
{
	public class UpdatedArgs : EventArgs
	{
		public object Sender { get; set; }
		public Type Type { get; set; }

		public UpdatedArgs(object sender, Type type)
		{
			Sender = sender;
			Type = type;
		}
	}
}
