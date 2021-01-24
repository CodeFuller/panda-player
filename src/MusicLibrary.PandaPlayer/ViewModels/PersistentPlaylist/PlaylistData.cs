using System.Collections.Generic;
using System.Linq;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Shared.Extensions;

namespace MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class PlaylistData
	{
		public IReadOnlyCollection<PlaylistSongData> Songs { get; set; }

		public int? CurrentSongIndex { get; set; }

		public PlaylistData()
		{
		}

		public PlaylistData(ISongPlaylistViewModel songPlaylistViewModel)
		{
			Songs = songPlaylistViewModel.Songs.Select(s => new PlaylistSongData(s)).ToCollection();
			CurrentSongIndex = songPlaylistViewModel.CurrentSongIndex;
		}
	}
}
