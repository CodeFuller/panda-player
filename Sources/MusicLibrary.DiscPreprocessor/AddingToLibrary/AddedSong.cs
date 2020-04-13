using MusicLibrary.Core.Objects;

namespace MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public class AddedSong
	{
		public Song Song { get; }

		public string SourceFileName { get; }

		public AddedSong(Song song, string sourceFileName)
		{
			Song = song;
			SourceFileName = sourceFileName;
		}
	}
}
