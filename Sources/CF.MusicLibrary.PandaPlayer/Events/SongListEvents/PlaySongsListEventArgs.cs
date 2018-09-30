using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
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
