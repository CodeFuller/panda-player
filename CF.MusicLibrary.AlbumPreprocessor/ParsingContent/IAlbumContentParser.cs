using System.Collections.Generic;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	public interface IAlbumContentParser
	{
		IEnumerable<AlbumContent> Parse(string content);
	}
}
