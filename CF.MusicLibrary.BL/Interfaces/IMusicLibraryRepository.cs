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

		Task UpdateSong(Song song);

		Task UpdateDisc(Disc disc);

		/// <summary>
		/// Returns all discs including deleted.
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<Disc>> GetDiscsAsync();

		Task AddSongPlayback(Song song, DateTime playbackTime);
	}
}
