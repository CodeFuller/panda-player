using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IExplorerSongListViewModel : ISongListViewModel
	{
		ICommand DeleteSongsFromDiscCommand { get; }
	}
}
