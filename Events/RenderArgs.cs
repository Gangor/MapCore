using System;

namespace MapCore.Events
{
	/// <summary>
	/// Render event 
	/// </summary>
	public class RenderArgs : EventArgs
	{
		/// <summary>
		/// Get or set object to send
		/// </summary>
		public object Sender { get; }

		/// <summary>
		/// Get or set type to the object
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Constructor for event render
		/// </summary>
		/// <param name="sender">Object to send</param>
		/// <param name="type">Type of the object</param>
		public RenderArgs(object sender, Type type)
		{
			Sender = sender;
			Type = type;
		}
	}
}
