using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public class PlaySongsListEventArgs : BaseSongListEventArgs
	{
		public DiscModel Disc
		{
			get
			{
				var uniqueDisc = UniqueDisc;
				if (uniqueDisc != null)
				{
					return uniqueDisc;
				}

				return UniqueAdviseSet != null ? Discs.First() : null;
			}
		}

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
