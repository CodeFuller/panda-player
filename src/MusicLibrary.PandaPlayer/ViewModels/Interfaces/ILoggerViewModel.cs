using System.Collections.ObjectModel;
using MusicLibrary.PandaPlayer.ViewModels.Logging;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILoggerViewModel
	{
		ObservableCollection<LogMessage> Messages { get; }
	}
}
