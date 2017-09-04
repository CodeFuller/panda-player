using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	/// <summary>
	/// Interface for music library repository.
	/// </summary>
	public interface IMusicLibraryRepository
	{
		Task AddSong(Song song);

		Task DeleteSong(Song song);

		Task DeleteDisc(Disc disc);

		Task<IEnumerable<Disc>> GetDiscsAsync();

		Task AddSongPlayback(Song song, DateTime playbackTime);
	}
}
