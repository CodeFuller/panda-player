using System.Collections.ObjectModel;
using CF.MusicLibrary.PandaPlayer.ViewModels.Logging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILoggerViewModel
	{
		ObservableCollection<LogMessage> Messages { get; }
	}
}
