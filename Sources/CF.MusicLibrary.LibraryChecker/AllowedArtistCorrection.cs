namespace CF.MusicLibrary.LibraryChecker
{
	internal struct AllowedArtistCorrection
	{
		public string OriginalArtistName { get; }

		public string CorrectedArtistName { get; }

		public AllowedArtistCorrection(string originalArtistName, string correctedArtistName)
		{
			OriginalArtistName = originalArtistName;
			CorrectedArtistName = correctedArtistName;
		}
	}
}
