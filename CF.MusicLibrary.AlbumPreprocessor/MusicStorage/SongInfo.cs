using System.IO;

namespace CF.MusicLibrary.AlbumPreprocessor.MusicStorage
{
	public class SongInfo
	{
		public string SourcePath { get; }

		public string SourceFileName => Path.GetFileName(SourcePath);

		public string Artist { get; set; }

		public int? Track { get; set; }

		public string Title { get; set; }

		/// <summary>
		/// Original song title, with preserved artist if it presents
		/// </summary>
		public string FullTitle { get; set; }

		public SongInfo(string sourcePath)
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
