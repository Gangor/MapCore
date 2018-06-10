using System;

namespace MapCore.Events
{
	/// <summary>
	/// Update event
	/// </summary>
	public class UpdatedArgs : EventArgs
	{
		/// <summary>
		/// Get the index
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Get or set object to send
		/// </summary>
		public object Sender { get; }

		/// <summary>
		/// Get or set type to the object
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Constructor for event update
		/// </summary>
		/// <param name="index">Index removed</param>
		/// <param name="sender">Object to send</param>
		/// <param name="type">Type of the object</param>
		public UpdatedArgs(int index, object sender, Type type)
		{
			Index = index;
			Sender = sender;
			Type = type;
		}
	}
}
