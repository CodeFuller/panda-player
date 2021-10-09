namespace PandaPlayer.Adviser.Settings
{
	internal class AdviserSettings
	{
		public int PlaybacksBetweenFavoriteAdviseGroups { get; set; }

		public HighlyRatedSongsAdviserSettings HighlyRatedSongsAdviser { get; set; } = new();
	}
}
