namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class AdviserSettings
	{
		public FavouriteArtistsAdviserSettings FavouriteArtistsAdviser { get; set; } = new FavouriteArtistsAdviserSettings();

		public HighlyRatedSongsAdviserSettings HighlyRatedSongsAdviser { get; set; } = new HighlyRatedSongsAdviserSettings();
	}
}
