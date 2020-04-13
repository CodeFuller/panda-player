using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaySongsListEventArgs : BaseSongListEventArgs
	{
		public PlaySongsListEventArgs(Disc disc)
			: this(disc.Songs)
		{
		}

		public PlaySongsListEventArgs(IEnumerable<Song> songs)
			: base(songs)
		{
		}
	}
}
