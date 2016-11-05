using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	/// <summary>
	/// Interface for music library builder.
	/// </summary>
	public interface ILibraryBuilder
	{
		/// <summary>
		/// Adds song to the built library.
		/// </summary>
		void AddSong(Song song);

		/// <summary>
		/// Builds library for added content.
		/// </summary>
		DiscLibrary Build();
	}
}
