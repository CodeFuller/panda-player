using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IPlaylistViewModel : ISongListViewModel
	{
		SongModel CurrentSong { get; }

		/// <summary>
		/// Gets currently played disc.
		/// If CurrentSong is not null, returns disc of current song.
		/// If CurrentSong is null and all songs in playlist belong to the same disc, returns this disc.
		/// Otherwise, returns null.
		/// </summary>
		DiscModel CurrentDisc { get; }

		IEnumerable<BasicMenuItem> ContextMenuItems { get; }

		Task SetPlaylistSongs(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		Task SwitchToNextSong(CancellationToken cancellationToken);
	}
}
