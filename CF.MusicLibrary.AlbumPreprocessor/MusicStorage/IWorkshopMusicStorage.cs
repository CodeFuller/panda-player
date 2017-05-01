using System.Collections.Generic;

namespace CF.MusicLibrary.AlbumPreprocessor.MusicStorage
{
	public interface IWorkshopMusicStorage
	{
		AlbumInfo GetAlbumInfo(string albumPath, IEnumerable<string> songFiles);
	}
}
