using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	/// <summary>
	/// Interface for music library repository.
	/// </summary>
	public interface IMusicLibraryRepository
	{
		/// <summary>
		/// Loads disc library from the repository.
		/// </summary>
		Task<DiscLibrary> GetDiscLibraryAsync();

		Task<IEnumerable<Genre>> GetGenresAsync();
	}
}
