using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ContentUpdate
{
	public interface ILibraryContentUpdater
	{
		Task UpdateSongs(IEnumerable<Song> songs, UpdatedSongProperties updatedProperties);

		Task DeleteDisc(Disc disc);
	}
}
