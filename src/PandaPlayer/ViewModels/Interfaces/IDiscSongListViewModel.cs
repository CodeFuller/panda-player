using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscSongListViewModel : ISongListViewModel
	{
		ICommand DeleteSongsFromDiscCommand { get; }
	}
}
