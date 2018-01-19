using System;

namespace MapCore.Events
{
	public class RenderArgs
	{
		public object Sender { get; set; }
		public Type Type { get; set; }

		public RenderArgs(object sender, Type type)
		{
			Sender = sender;
			Type = type;
		}
	}
}
