using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistLoadedEventArgs : BasicPlaylistEventArgs
	{
		public PlaylistLoadedEventArgs(IEnumerable<SongModel> songs, SongModel currentSong, int? currentSongIndex)
			: base(songs, currentSong, currentSongIndex)
		{
		}
	}
}
