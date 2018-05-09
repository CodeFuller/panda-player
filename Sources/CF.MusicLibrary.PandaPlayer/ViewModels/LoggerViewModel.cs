using System;
using System.Collections.ObjectModel;
using System.Windows;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.Logging;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class LoggerViewModel : ViewModelBase, ILoggerViewModel, ILogger
	{
		private sealed class EmptyDisposable : IDisposable
		{
			public static IDisposable Instance { get; } = new EmptyDisposable();

			public void Dispose()
			{
			}
		}

		public ObservableCollection<LogMessage> Messages { get; } = new ObservableCollection<LogMessage>();

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			var logMessage = new LogMessage(logLevel, formatter(state, exception));
			Application.Current.Dispatcher.Invoke(() => Messages.Add(logMessage));
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return EmptyDisposable.Instance;
		}
	}
}
