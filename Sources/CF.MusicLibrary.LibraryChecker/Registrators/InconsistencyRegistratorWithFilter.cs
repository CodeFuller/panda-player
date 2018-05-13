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

		public void RegisterInconsistency_SuspiciousAlbumTitle(Disc disc)
		{
			inconsistencyRegistrator.RegisterInconsistency_SuspiciousAlbumTitle(disc);
		}

		public void RegisterInconsistency_DiscWithoutSongs(Disc disc)
		{
			inconsistencyRegistrator.RegisterInconsistency_DiscWithoutSongs(disc);
		}

		public void RegisterInconsistency_BadTrackNumbersForDisc(Disc disc, IEnumerable<short?> trackNumbers)
		{
			inconsistencyRegistrator.RegisterInconsistency_BadTrackNumbersForDisc(disc, trackNumbers);
		}

		public void RegisterInconsistency_DifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres)
		{
			var genresList = genres.ToList();
			if (inconsistencyFilter.SkipInconsistency_DifferentGenresForDisc(disc, genresList))
			{
				return;
			}

			inconsistencyRegistrator.RegisterInconsistency_DifferentGenresForDisc(disc, genresList);
		}

		public void RegisterInconsistency_BadTagData(string inconsistencyMessage)
		{
			inconsistencyRegistrator.RegisterInconsistency_BadTagData(inconsistencyMessage);
		}

		public void RegisterInconsistency_BadTagData(Song song, IEnumerable<SongTagType> tagTypes)
		{
			inconsistencyRegistrator.RegisterInconsistency_BadTagData(song, tagTypes);
		}

		public void RegisterInconsistency_ArtistNotFound(Artist artist)
		{
			inconsistencyRegistrator.RegisterInconsistency_ArtistNotFound(artist);
		}

		public void RegisterInconsistency_ArtistNameCorrected(string originalArtistName, string correctedArtistName)
		{
			if (inconsistencyFilter.SkipInconsistency_ArtistNameCorrected(originalArtistName, correctedArtistName))
			{
				return;
			}

			inconsistencyRegistrator.RegisterInconsistency_ArtistNameCorrected(originalArtistName, correctedArtistName);
		}

		public void RegisterInconsistency_NoListensForArtist(Artist artist)
		{
			inconsistencyRegistrator.RegisterInconsistency_NoListensForArtist(artist);
		}

		public void RegisterInconsistency_AlbumNotFound(Disc disc)
		{
			inconsistencyRegistrator.RegisterInconsistency_AlbumNotFound(disc);
		}

		public void RegisterInconsistency_NoListensForAlbum(Disc disc)
		{
			inconsistencyRegistrator.RegisterInconsistency_NoListensForAlbum(disc);
		}

		public void RegisterInconsistency_SongNotFound(Song song)
		{
			inconsistencyRegistrator.RegisterInconsistency_SongNotFound(song);
		}

		public void RegisterInconsistency_SongTitleCorrected(Song song, string correctedSongTitle)
		{
			if (inconsistencyFilter.SkipInconsistency_SongTitleCorrected(song, correctedSongTitle))
			{
				return;
			}

			inconsistencyRegistrator.RegisterInconsistency_SongTitleCorrected(song, correctedSongTitle);
		}

		public void RegisterInconsistency_NoListensForSong(Song song)
		{
			inconsistencyRegistrator.RegisterInconsistency_NoListensForSong(song);
		}

		public void RegisterInconsistency_MissingStorageData(Uri itemUri)
		{
			inconsistencyRegistrator.RegisterInconsistency_MissingStorageData(itemUri);
		}

		public void RegisterInconsistency_UnexpectedStorageData(string itemPath, string itemType)
		{
			inconsistencyRegistrator.RegisterInconsistency_UnexpectedStorageData(itemPath, itemType);
		}

		public void RegisterInconsistency_ErrorInStorageData(string errorMessage)
		{
			inconsistencyRegistrator.RegisterInconsistency_ErrorInStorageData(errorMessage);
		}

		public void RegisterInconsistency_DiscCoverIsTooSmall(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterInconsistency_DiscCoverIsTooSmall(disc, imageInfo);
		}

		public void RegisterInconsistency_DiscCoverIsTooBig(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterInconsistency_DiscCoverIsTooBig(disc, imageInfo);
		}

		public void RegisterInconsistency_ImageFileIsTooBig(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterInconsistency_ImageFileIsTooBig(disc, imageInfo);
		}

		public void RegisterInconsistency_ImageHasUnsupportedFormat(Disc disc, ImageInfo imageInfo)
		{
			inconsistencyRegistrator.RegisterInconsistency_ImageHasUnsupportedFormat(disc, imageInfo);
		}

		public void RegisterFix_ErrorInStorageData(string fixMessage)
		{
			inconsistencyRegistrator.RegisterFix_ErrorInStorageData(fixMessage);
		}
	}
}
