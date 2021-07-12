using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public class BasicPlaylistEventArgs : BaseSongListEventArgs
	{
		public SongModel CurrentSong { get; }

		public int? CurrentSongIndex { get; }

		public BasicPlaylistEventArgs(IEnumerable<SongModel> songs, SongModel currentSong, int? currentSongIndex)
			: base(songs)
		{
			CurrentSong = currentSong;
			CurrentSongIndex = currentSongIndex;
		}
	}
}
