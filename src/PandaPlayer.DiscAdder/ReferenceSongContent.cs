namespace PandaPlayer.DiscAdder
{
	internal class ReferenceSongContent
	{
		private readonly int expectedTrackNumber;

		public string ExpectedTitle { get; }

		public string ExpectedTitleWithTrackNumber => $"{expectedTrackNumber:D2} - {ExpectedTitle}";

		public ReferenceSongContent(int expectedTrackNumber, string expectedTitle)
		{
			this.expectedTrackNumber = expectedTrackNumber;
			ExpectedTitle = expectedTitle;
		}
	}
}
