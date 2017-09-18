namespace CF.MusicLibrary.BL.Media
{
	public class SongTagData
	{
		public string Artist { get; set; }

		public string Album { get; set; }

		public int? Year { get; set; }

		public string Genre { get; set; }

		public int? Track { get; set; }

		public string Title { get; set; }

		public static UpdatedSongProperties TaggedProperties => UpdatedSongProperties.Artist | UpdatedSongProperties.Album | UpdatedSongProperties.Year |
																UpdatedSongProperties.Genre | UpdatedSongProperties.Track | UpdatedSongProperties.Title | UpdatedSongProperties.ForceTagUpdate;
	}
}
