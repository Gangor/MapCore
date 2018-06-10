using System;

namespace MapCore.Events
{
	/// <summary>
	/// Events remove
	/// </summary>
	public class RemovedArgs : EventArgs
	{
		/// <summary>
		/// Get the index
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Get the type object
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index">Index removed</param>
		/// <param name="type">Type object</param>
		public RemovedArgs(int index, Type type)
		{
			Index = index;
			Type = type;
		}
	}
}
