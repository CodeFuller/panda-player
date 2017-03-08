using System.IO;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public class SongContent
	{
		public string Title { get; set; }

		public SongContent(string title)
		{
			Title = title;
		}
	}
}
