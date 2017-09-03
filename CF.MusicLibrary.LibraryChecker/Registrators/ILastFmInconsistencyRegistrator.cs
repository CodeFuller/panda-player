using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface ILastFMInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_ArtistNotFound(Artist artist);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_ArtistNameCorrected(string originalArtistName, string correctedArtistName);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_NoListensForArtist(Artist artist);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_AlbumNotFound(Disc disc);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_NoListensForAlbum(Disc disc);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_SongNotFound(Song song);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_SongTitleCorrected(Song song, string correctedSongTitle);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_NoListensForSong(Song song);
	}
}
