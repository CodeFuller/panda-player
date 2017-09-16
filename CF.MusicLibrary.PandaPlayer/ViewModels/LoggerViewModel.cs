using System.Collections.ObjectModel;
using System.Windows;
using CF.Library.Core.Logging;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.Logging;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class LoggerViewModel : ViewModelBase, IMessageLogger, ILoggerViewModel
	{
		public ObservableCollection<LogMessage> Messages { get; } = new ObservableCollection<LogMessage>();

		public void WriteDebug(string message)
		{
			AddMessage(new LogMessage(LogMessageLevel.Debug, message));
		}

		public void WriteInfo(string message)
		{
			AddMessage(new LogMessage(LogMessageLevel.Info, message));
		}

		public void WriteWarning(string message)
		{
			AddMessage(new LogMessage(LogMessageLevel.Warning, message));
		}

		public void WriteError(string message)
		{
			AddMessage(new LogMessage(LogMessageLevel.Error, message));
		}

		private void AddMessage(LogMessage logMessage)
		{
			Application.Current.Dispatcher.Invoke(() => Messages.Add(logMessage));
		}
	}
}
