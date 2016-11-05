using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	/// <summary>
	/// Interface for music library repository.
	/// </summary>
	public interface IMusicLibraryRepository
	{
		/// <summary>
		/// Loads the library from the repository.
		/// </summary>
		DiscLibrary LoadLibrary();

		/// <summary>
		/// Stores the library in the repository.
		/// </summary>
		void Store(DiscLibrary library);
	}
}
