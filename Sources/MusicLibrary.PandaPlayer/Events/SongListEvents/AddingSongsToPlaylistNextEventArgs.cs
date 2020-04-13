using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistNextEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistNextEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
