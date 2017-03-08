using System.IO;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public class SongFileFilter : ISongFileFilter
	{
		public bool IsSongFile(string filePath)
		{
			var filename = Path.GetFileName(filePath);
			return filename != "cover.jpg";
		}
	}
}
