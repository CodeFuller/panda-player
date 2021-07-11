using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
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

		public PlaylistData(IEnumerable<SongModel> songs, int? currentSongIndex)
		{
			Songs = songs.Select(s => new PlaylistSongData(s)).ToCollection();
			CurrentSongIndex = currentSongIndex;
		}
	}
}
