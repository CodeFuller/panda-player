using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class PlaylistData
	{
		public IReadOnlyCollection<PlaylistSongData> Songs { get; set; }

		public PlaylistSongData CurrentSong { get; set; }

		public PlaylistData()
		{
		}

		public PlaylistData(ISongPlaylistViewModel songPlaylistViewModel)
		{
			Songs = songPlaylistViewModel.Songs.Select(s => new PlaylistSongData(s)).ToCollection();
			CurrentSong = songPlaylistViewModel.CurrentSong == null ? null : Songs.Single(s => s.Matches(songPlaylistViewModel.CurrentSong));
		}
	}
}
