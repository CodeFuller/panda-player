namespace MusicLibrary.PandaPlayer.Adviser.Settings
{
	internal class AdviserSettings
	{
		public FavoriteArtistsAdviserSettings FavoriteArtistsAdviser { get; set; } = new();

		public HighlyRatedSongsAdviserSettings HighlyRatedSongsAdviser { get; set; } = new();
	}
}
