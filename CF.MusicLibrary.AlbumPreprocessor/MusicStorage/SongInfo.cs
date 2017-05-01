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

		public SongInfo(string sourcePath)
		{
			SourcePath = sourcePath;
		}
	}
}
