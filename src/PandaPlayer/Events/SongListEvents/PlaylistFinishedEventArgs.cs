using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public class PlaylistFinishedEventArgs : BaseSongListEventArgs
	{
		public PlaylistFinishedEventArgs(IEnumerable<SongModel> playlistSongs)
			: base(playlistSongs)
		{
		}
	}
}
