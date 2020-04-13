using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public abstract class AddingSongsToPlaylistEventArgs : BaseSongListEventArgs
	{
		protected AddingSongsToPlaylistEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
