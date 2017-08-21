using System.Collections.ObjectModel;
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
			Messages.Add(new LogMessage(LogMessageLevel.Debug, message));
		}

		public void WriteInfo(string message)
		{
			Messages.Add(new LogMessage(LogMessageLevel.Info, message));
		}

		public void WriteWarning(string message)
		{
			Messages.Add(new LogMessage(LogMessageLevel.Warning, message));
		}

		public void WriteError(string message)
		{
			Messages.Add(new LogMessage(LogMessageLevel.Error, message));
		}
	}
}
