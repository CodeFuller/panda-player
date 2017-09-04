using System.IO;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;

namespace CF.MusicLibrary.DiscPreprocessor
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
