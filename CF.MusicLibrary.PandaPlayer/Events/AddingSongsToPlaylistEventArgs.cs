using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public abstract class AddingSongsToPlaylistEventArgs : BaseSongListEventArgs
	{
		protected AddingSongsToPlaylistEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
