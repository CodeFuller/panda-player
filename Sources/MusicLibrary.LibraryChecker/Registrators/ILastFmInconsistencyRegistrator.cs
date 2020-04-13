using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker.Registrators
{
	public interface ILastFMInconsistencyRegistrator
	{
		void RegisterArtistNotFound(Artist artist);

		void RegisterArtistNameCorrected(string originalArtistName, string correctedArtistName);

		void RegisterNoListensForArtist(Artist artist);

		void RegisterAlbumNotFound(Disc disc);

		void RegisterNoListensForAlbum(Disc disc);

		void RegisterSongNotFound(Song song);

		void RegisterSongTitleCorrected(Song song, string correctedSongTitle);

		void RegisterNoListensForSong(Song song);
	}
}
