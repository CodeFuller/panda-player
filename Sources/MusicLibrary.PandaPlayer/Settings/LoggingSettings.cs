﻿using Microsoft.Extensions.Logging;

namespace MusicLibrary.PandaPlayer.Settings
{
	internal class LoggingSettings
	{
		public LogLevel LogLevel { get; set; } = LogLevel.Information;

		public LogLevel DatabaseOperationsLogLevel { get; set; } = LogLevel.Warning;
	}
}
