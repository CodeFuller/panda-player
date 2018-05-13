namespace CF.MusicLibrary.LibraryChecker
{
	internal struct AllowedSongCorrection
	{
		public string Artist { get; }

		public string OriginalSongTitle { get; }

		public string CorrectedSongTitle { get; }

		public AllowedSongCorrection(string artist, string originalSongTitle, string correctedSongTitle)
		{
			Artist = artist;
			OriginalSongTitle = originalSongTitle;
			CorrectedSongTitle = correctedSongTitle;
		}
	}
}
