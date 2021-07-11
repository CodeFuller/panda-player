using System.Windows.Input;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IExplorerSongListViewModel : ISongListViewModel
	{
		ICommand DeleteSongsFromDiscCommand { get; }
	}
}
