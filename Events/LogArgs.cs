using MapCore.Enum;
using System;

namespace MapCore.Events
{
	/// <summary>
	/// Events log
	/// </summary>
    public class LogArgs : EventArgs
	{
		/// <summary>
		/// Get level of the log
		/// </summary>
		public Levels Level { get; }

		/// <summary>
		/// Get log message
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// Constructor for event log
		/// </summary>
		/// <param name="level">Level of the log</param>
		/// <param name="message">Message to send</param>
		public LogArgs(Levels level, string message)
		{
			Level = level;
			Message = message;
		}
	}
}
