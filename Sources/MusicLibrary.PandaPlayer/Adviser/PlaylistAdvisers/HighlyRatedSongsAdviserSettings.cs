using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class HighlyRatedSongsAdviserSettings
	{
		public int PlaybacksBetweenHighlyRatedSongs { get; set; }

		public int OneAdviseSongsNumber { get; set; }

		public ICollection<MaxUnlistenedSongTerm> MaxUnlistenedTerms { get; } = new Collection<MaxUnlistenedSongTerm>();
	}
}
