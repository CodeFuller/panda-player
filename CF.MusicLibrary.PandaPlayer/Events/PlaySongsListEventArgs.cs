using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
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
