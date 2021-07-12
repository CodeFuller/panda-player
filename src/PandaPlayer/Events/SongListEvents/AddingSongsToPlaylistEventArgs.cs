using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public abstract class AddingSongsToPlaylistEventArgs : BaseSongListEventArgs
	{
		protected AddingSongsToPlaylistEventArgs(IEnumerable<SongModel> songs)
			: base(songs)
		{
		}
	}
}
