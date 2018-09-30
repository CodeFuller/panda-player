using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistNextEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistNextEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
