using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ISongListTabViewModel
	{
		IDiscSongListViewModel DiscSongListViewModel { get; }

		IPlaylistViewModel PlaylistViewModel { get; }

		bool IsDiscSongListSelected { get; }

		bool IsPlaylistSelected { get; }

		ICommand SwitchToDiscSongListCommand { get; }

		ICommand SwitchToPlaylistCommand { get; }
	}
}
