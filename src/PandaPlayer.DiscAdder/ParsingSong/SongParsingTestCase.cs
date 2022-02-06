namespace PandaPlayer.DiscAdder.ParsingSong
{
	internal class SongParsingTestCase
	{
		public string RawContent { get; }

		public string ExpectedTitle { get; }

		public SongParsingTestCase(string rawContent, string expectedTitle)
		{
			RawContent = rawContent;
			ExpectedTitle = expectedTitle;
		}
	}
}
