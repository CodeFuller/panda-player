using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistLastEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistLastEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
