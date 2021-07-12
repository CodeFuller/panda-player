using System;
using Microsoft.Extensions.Logging;

namespace PandaPlayer.ViewModels.Logging
{
	public class LogMessage
	{
		public LogLevel Level { get; set; }

		public string Message { get; set; }

		public LogMessage(LogLevel level, string message)
		{
			Level = level;
			Message = $"{DateTime.Now:yyyy-MM-dd  HH:mm:ss}  {GetLevelString(level)}  {message}";
		}

		private static string GetLevelString(LogLevel level)
		{
			return level switch
			{
				LogLevel.Critical => "CRITICAL:",
				LogLevel.Error => "ERROR:  ",
				LogLevel.Warning => "WARNING:",
				LogLevel.Information => "INFO:   ",
				LogLevel.Debug => "DEBUG:  ",
				LogLevel.Trace => "TRACE:  ",
				_ => throw new NotSupportedException($"Log level {level} is not supported"),
			};
		}
	}
}
