using System.Collections.Generic;

namespace CF.MusicLibrary.AlbumPreprocessor.Interfaces
{
	public interface IEthalonAlbumParser
	{
		AlbumContent Parse(IEnumerable<string> albumContent);
	}
}
