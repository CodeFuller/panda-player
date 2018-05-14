using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public class InconsistencyRegistratorWithFilter : ILibraryInconsistencyRegistrator
	{
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		private readonly ILibraryInconsistencyFilter inconsistencyFilter;

		public InconsistencyRegistratorWithFilter(ILibraryInconsistencyRegistrator inconsistencyRegistrator, ILibraryInconsistencyFilter inconsistencyFilter)
		{
			this.inconsistencyRegistrator = inconsistencyRegistrator ?? throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			this.inconsistencyFilter = inconsistencyFilter ?? throw new ArgumentNullException(nameof(inconsistencyFilter));
		}

		public void RegisterSuspiciousAlbumTitle(Disc disc)
		{
			inconsistencyRegistrator.RegisterSuspiciousAlbumTitle(disc);
		}

		public void RegisterDiscWithoutSongs(Disc disc)
		{
			inconsistencyRegistrator.RegisterDiscWithoutSongs(disc);
		}

		public void RegisterBadTrackNumbersForDisc(Disc disc, IEnumerable<short?> trackNumbers)
		{
			inconsistencyRegistrator.RegisterBadTrackNumbersForDisc(disc, trackNumbers);
		}

		public void RegisterDifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres)
		{
			var genresList = genres.ToList();
			if (inconsistencyFilter.ShouldSkipDifferentGenresForDisc(disc, genresList))
			{
				return;
			}

			inconsistencyRegistrator.RegisterDifferentGenresForDisc(disc, genresList);
		}

		public void RegisterBadTagData(string inconsistencyMessage)
		{
			inconsistencyRegistrator.RegisterBadTagData(inconsistencyMessage);
		}

		public void RegisterBadTagData(Song song, IEnumerable<SongTagType> tagTypes)
		{
			inconsistencyRegistrator.RegisterBadTagData(song, tagTypes);
		}

		public void RegisterArtistNotFound(Artist artist)
		{
			inconsistencyRegistrator.RegisterArtistNotFound(artist);
		}

		public void RegisterArtistNameCorrected(string originalArtistName, string correctedArtistName)
		{
			if (inconsistencyFilter.ShouldSkipArtistNameCorrection(originalArtistName, correctedArtistName))
			{
				return;
			}

			inconsistencyRegistrator.RegisterArtistNameCorrected(originalArtistName, correctedArtistName);
		}

		public void RegisterNoListensForArtist(Artist artist)
		{
			inconsistencyRegistrator.RegisterNoListensForArtist(artist);
		}

		public void RegisterAlbumNotFound(Disc disc)
		{
			inconsistencyRegistrator.RegisterAlbumNotFound(disc);
		}

		public void RegisterNoListensForAlbum(Disc disc)
		{
			inconsistencyRegistrator.RegisterNoListensForAlbum(disc);
		}

		public void RegisterSongNotFound(Song song)
		{
			inconsistencyRegistrator.RegisterSongNotFound(song);
		}

		public void RegisterSongTitleCorrected(Song song, string correctedSongTitle)
		{
			if (inconsistencyFilter.ShouldSkipSongTitleCorrection(song, correctedSongTitle))
			{
				return;
			}

			inconsistencyRegistrator.RegisterSongTitleCorrected(song, correctedSongTitle);
		}

		public void RegisterNoListensForSong(Song song)
		{
			inconsistencyRegistrator.RegisterNoListensForSong(song);
		}

		public void RegisterMissingStorageData(Uri itemUri)
		{
			inconsistencyRegistrator.RegisterMissingStorageData(itemUri);
		}

		public void RegisterUnexpectedStorageData(string itemPath, string itemType)
		{
			inconsistencyRegistrator.RegisterUnexpectedStorageData(itemPath, itemType);
		}

		public void RegisterErrorInStorageData(string errorMessage)
		{
			inconsistencyRegistrator.RegisterErrorInStorageData(errorMessage);
		}

		public void RegisterDiscCoverIsTooSmall(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterDiscCoverIsTooSmall(disc, imageInfo);
		}

		public void RegisterDiscCoverIsTooBig(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterDiscCoverIsTooBig(disc, imageInfo);
		}

		public void RegisterImageFileIsTooBig(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterImageFileIsTooBig(disc, imageInfo);
		}

		public void RegisterImageHasUnsupportedFormat(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterImageHasUnsupportedFormat(disc, imageInfo);
		}

		public void RegisterFixOfErrorInStorageData(string fixMessage)
		{
			inconsistencyRegistrator.RegisterFixOfErrorInStorageData(fixMessage);
		}
	}
}
