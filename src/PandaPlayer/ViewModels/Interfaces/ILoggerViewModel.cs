using System.Collections.ObjectModel;
using PandaPlayer.ViewModels.Logging;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILoggerViewModel
	{
		ObservableCollection<LogMessage> Messages { get; }
	}
}
