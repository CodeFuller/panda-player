using System;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PandaPlayer.Settings;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Logging;

namespace PandaPlayer.ViewModels
{
	internal class LoggerViewModel : ViewModelBase, ILoggerViewModel, ILogger
	{
		private sealed class EmptyDisposable : IDisposable
		{
			public static IDisposable Instance { get; } = new EmptyDisposable();

			public void Dispose()
			{
			}
		}

		private readonly LoggingSettings settings;

		public ObservableCollection<LogMessage> Messages { get; } = new ObservableCollection<LogMessage>();

		public LoggerViewModel(IOptions<LoggingSettings> options)
		{
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			var logMessage = new LogMessage(logLevel, formatter(state, exception));
			Application.Current.Dispatcher.Invoke(() => Messages.Add(logMessage));
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= settings.LogLevel;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return EmptyDisposable.Instance;
		}
	}
}
