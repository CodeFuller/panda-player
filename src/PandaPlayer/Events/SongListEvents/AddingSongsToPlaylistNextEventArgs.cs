using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistNextEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistNextEventArgs(IEnumerable<SongModel> songs)
			: base(songs)
		{
		}
	}
}
