using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public abstract class AddingSongsToPlaylistEventArgs : BaseSongListEventArgs
	{
		protected AddingSongsToPlaylistEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
