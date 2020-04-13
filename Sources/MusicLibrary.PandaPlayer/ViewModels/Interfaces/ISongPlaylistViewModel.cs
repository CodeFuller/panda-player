using System.Windows.Input;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongPlaylistViewModel : ISongListViewModel
	{
		Song CurrentSong { get; }

		int? CurrentSongIndex { get; }

		/// <summary>
		/// If all songs in playlist belong to one Disc, returns this Disc.
		/// Returns null otherwise.
		/// </summary>
		Disc PlayedDisc { get; }

		ICommand NavigateToSongDiscCommand { get; }

		void SwitchToNextSong();

		void SwitchToSong(Song song);
	}
}
