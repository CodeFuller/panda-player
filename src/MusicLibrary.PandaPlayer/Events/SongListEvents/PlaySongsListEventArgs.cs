using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaySongsListEventArgs : BaseSongListEventArgs
	{
		public PlaySongsListEventArgs(DiscModel disc)
			: this(disc.ActiveSongs)
		{
		}

		public PlaySongsListEventArgs(IEnumerable<SongModel> songs)
			: base(songs)
		{
		}
	}
}
