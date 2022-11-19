using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistLastEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistLastEventArgs(IEnumerable<SongModel> songs)
			: base(songs)
		{
		}
	}
}
