using System;
using CF.Library.Core.Exceptions;
using Microsoft.Extensions.Logging;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Logging
{
	public class LogMessage
	{
		public LogLevel Level { get; set; }

		public string Message { get; set; }

		public LogMessage(LogLevel level, string message)
		{
			Level = level;
			Message = Current($"{DateTime.Now:yyyy-MM-dd  HH:mm:ss}    {GetLevelString(level)}    {message}");
		}

		private static string GetLevelString(LogLevel level)
		{
			switch (level)
			{
				case LogLevel.Critical:
					return "CRITICAL:";

				case LogLevel.Error:
					return "ERROR:  ";

				case LogLevel.Warning:
					return "WARNING:";

				case LogLevel.Information:
					return "INFO:   ";

				case LogLevel.Debug:
					return "DEBUG:  ";

				case LogLevel.Trace:
					return "TRACE:  ";

				default:
					throw new UnexpectedEnumValueException(level);
			}
		}
	}
}
