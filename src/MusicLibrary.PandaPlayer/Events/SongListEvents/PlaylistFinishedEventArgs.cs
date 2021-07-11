using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistFinishedEventArgs : BaseSongListEventArgs
	{
		public PlaylistFinishedEventArgs(IEnumerable<SongModel> playlistSongs)
			: base(playlistSongs)
		{
		}
	}
}
