using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistLastEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistLastEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
