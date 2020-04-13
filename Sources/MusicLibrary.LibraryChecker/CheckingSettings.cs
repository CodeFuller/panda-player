namespace MusicLibrary.LibraryChecker
{
	public class CheckingSettings
	{
		public string LastFmUsername { get; set; }

		public InconsistencyFilterSettings InconsistencyFilter { get; } = new InconsistencyFilterSettings();
	}
}
