using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistChangedEventArgs : BasicPlaylistEventArgs
	{
		public PlaylistChangedEventArgs(IEnumerable<SongModel> songs, SongModel currentSong, int? currentSongIndex)
			: base(songs, currentSong, currentSongIndex)
		{
		}
	}
}
