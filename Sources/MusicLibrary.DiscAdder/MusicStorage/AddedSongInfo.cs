namespace MusicLibrary.DiscAdder.MusicStorage
{
	public class AddedSongInfo
	{
		public string SourcePath { get; }

		public string Artist { get; set; }

		public short? Track { get; set; }

		public string Title { get; set; }

		public string TreeTitle { get; set; }

		/// <summary>
		/// Gets or sets original song title, with preserved artist if it presents.
		/// </summary>
		public string FullTitle { get; set; }

		public AddedSongInfo(string sourcePath)
		{
			SourcePath = sourcePath;
		}

		public void DismissArtistInfo()
		{
			Artist = null;
			Title = FullTitle;
		}
	}
}
