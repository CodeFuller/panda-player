using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Models;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.ViewModels.PersistentPlaylist
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
