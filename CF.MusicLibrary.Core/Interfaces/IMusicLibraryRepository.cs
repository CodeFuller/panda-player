using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.Core.Interfaces
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
		Task<IEnumerable<Disc>> GetDiscs();

		Task AddSongPlayback(Song song, DateTime playbackTime);
	}
}
