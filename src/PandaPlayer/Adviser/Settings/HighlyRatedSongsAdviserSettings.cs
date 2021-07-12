using System.Collections.Generic;

namespace PandaPlayer.Adviser.Settings
{
	internal class HighlyRatedSongsAdviserSettings
	{
		public int PlaybacksBetweenHighlyRatedSongs { get; set; }

		public int OneAdviseSongsNumber { get; set; }

		public IReadOnlyCollection<MaxRatingTerm> MaxTerms { get; set; }
	}
}
