using System.Collections.Generic;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	public interface IEthalonAlbumParser
	{
		AlbumContent Parse(IEnumerable<string> albumContent);
	}
}
