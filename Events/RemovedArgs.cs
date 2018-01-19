using System;

namespace MapCore.Events
{
	public class RemovedArgs : EventArgs
	{
		public int Index { get; set; }
		public Type Type { get; set; }

		public RemovedArgs(int index, Type type)
		{
			Index = index;
			Type = type;
		}
	}
}
