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

		int? CurrentSongIndex { get; }

		/// <summary>
		/// Gets currently playing disc, if all songs in playlist belong to one disc.
		/// Otherwise, returns null.
		/// </summary>
		DiscModel PlayingDisc { get; }

		ICommand NavigateToSongDiscCommand { get; }

		Task SetPlaylistSongs(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		Task SwitchToNextSong(CancellationToken cancellationToken);

		Task SwitchToSong(SongModel song, CancellationToken cancellationToken);
	}
}
