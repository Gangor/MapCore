using MapCore.Enum;
using System;

namespace MapCore.Events
{
    public class LogArgs : EventArgs
	{
		public Levels Level { get; set; }
		public string Message { get; set; }

		public LogArgs() { }
		public LogArgs(Levels level, string message)
		{
			Level = level;
			Message = message;
		}
	}
}
