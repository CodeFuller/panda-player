namespace CF.MusicLibrary.AlbumPreprocessor.ParsingSong
{
	internal class SongParsingTest
	{
		public string InputTitle { get; }

		public string ExpectedTitle { get; }

		public SongParsingTest(string inputTitle, string expectedTitle)
		{
			InputTitle = inputTitle;
			ExpectedTitle = expectedTitle;
		}
	}
}
