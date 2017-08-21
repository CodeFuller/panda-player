using System;
using CF.Library.Core.Exceptions;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Logging
{
	public class LogMessage
	{
		public LogMessageLevel Level { get; set; }

		public string Message { get; set; }

		public LogMessage(LogMessageLevel level, string message)
		{
			Level = level;
			Message = Current($"{DateTime.Now:yyyy-MM-dd  HH:mm:ss}    {GetLevelString(level)}    {message}");
		}

		private static string GetLevelString(LogMessageLevel level)
		{
			switch (level)
			{
				case LogMessageLevel.Error:
					return "ERROR:  ";

				case LogMessageLevel.Warning:
					return "WARNING:";

				case LogMessageLevel.Info:
					return "INFO:   ";

				case LogMessageLevel.Debug:
					return "DEBUG:  ";

				default:
					throw new UnexpectedEnumValueException(level);
			}
		}
	}
}
