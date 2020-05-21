using System.Windows.Input;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongPlaylistViewModel : ISongListViewModel
	{
		SongModel CurrentSong { get; }

		int? CurrentSongIndex { get; }

		/// <summary>
		/// Gets id of currently playing disc, if all songs in playlist belong to one disc
		/// Otherwise, returns null.
		/// </summary>
		ItemId PlayingDiscId { get; }

		ICommand NavigateToSongDiscCommand { get; }

		void SwitchToNextSong();

		void SwitchToSong(SongModel song);
	}
}
