using System;

namespace MapCore.Events
{
	/// <summary>
	/// Adding event
	/// </summary>
	public class AddedArgs
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
		/// Constructor for adding event object 
		/// </summary>
		/// <param name="sender">Object to send</param>
		/// <param name="type">Type of the object</param>
		public AddedArgs(object sender, Type type)
		{
			Sender = sender;
			Type = type;
		}
	}
}
