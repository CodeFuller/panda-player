using System;
using Microsoft.Extensions.Logging;

namespace MusicLibrary.PandaPlayer
{
	public sealed class InstanceLoggerProvider : ILoggerProvider
	{
		private readonly ILogger loggerInstance;

		public InstanceLoggerProvider(ILogger loggerInstance)
		{
			this.loggerInstance = loggerInstance ?? throw new ArgumentNullException(nameof(loggerInstance));
		}

		public ILogger CreateLogger(string categoryName)
		{
			return loggerInstance;
		}

		public void Dispose()
		{
		}
	}
}
