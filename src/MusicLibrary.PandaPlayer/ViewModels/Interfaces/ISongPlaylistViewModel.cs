using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongPlaylistViewModel : ISongListViewModel
	{
		SongModel CurrentSong { get; }

		/// <summary>
		/// Gets currently played disc.
		/// If CurrentSong is not null, returns disc of current song.
		/// If CurrentSong is null and all songs in playlist belong to the same disc, returns this disc.
		/// Otherwise, returns null.
		/// </summary>
		DiscModel CurrentDisc { get; }

		ICommand PlayFromSongCommand { get; }

		ICommand RemoveSongsFromPlaylistCommand { get; }

		ICommand ClearPlaylistCommand { get; }

		ICommand NavigateToSongDiscCommand { get; }

		Task SetPlaylistSongs(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		Task SwitchToNextSong(CancellationToken cancellationToken);
	}
}
