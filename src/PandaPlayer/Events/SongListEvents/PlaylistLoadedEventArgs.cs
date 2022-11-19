using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public class PlaylistLoadedEventArgs : BasicPlaylistEventArgs
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

				return UniqueAdviseSet != null && CurrentSong != null ? Discs.FirstOrDefault(x => x.Id == CurrentSong.Disc.Id) : null;
			}
		}

		public PlaylistLoadedEventArgs(IEnumerable<SongModel> songs, SongModel currentSong, int? currentSongIndex)
			: base(songs, currentSong, currentSongIndex)
		{
		}
	}
}
