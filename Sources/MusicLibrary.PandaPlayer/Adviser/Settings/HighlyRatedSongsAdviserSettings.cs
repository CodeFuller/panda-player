using System.Collections.Generic;

namespace MusicLibrary.PandaPlayer.Adviser.Settings
{
	internal class HighlyRatedSongsAdviserSettings
	{
		public int PlaybacksBetweenHighlyRatedSongs { get; set; }

		public int OneAdviseSongsNumber { get; set; }

		public IReadOnlyCollection<MaxUnlistenedSongTerm> MaxUnlistenedTerms { get; set; }
	}
}
