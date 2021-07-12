using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public class PlaylistLoadedEventArgs : BasicPlaylistEventArgs
	{
		public PlaylistLoadedEventArgs(IEnumerable<SongModel> songs, SongModel currentSong, int? currentSongIndex)
			: base(songs, currentSong, currentSongIndex)
		{
		}
	}
}
