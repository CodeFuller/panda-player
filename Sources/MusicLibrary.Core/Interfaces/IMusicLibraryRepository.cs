using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.Core.Interfaces
{
	/// <summary>
	/// Interface for music library repository.
	/// </summary>
	public interface IMusicLibraryRepository
	{
		Task AddSong(Song song);

		Task UpdateSong(Song song);

		Task UpdateDisc(Disc disc);

		Task AddDiscImage(DiscImage discImage);

		Task UpdateDiscImage(DiscImage discImage);

		Task DeleteDiscImage(DiscImage discImage);

		/// <summary>
		/// Returns all discs including deleted.
		/// </summary>
		Task<IEnumerable<Disc>> GetDiscs();

		Task AddSongPlayback(Song song, DateTime playbackTime);
	}
}
