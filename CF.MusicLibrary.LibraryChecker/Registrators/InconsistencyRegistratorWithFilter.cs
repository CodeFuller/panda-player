using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public class InconsistencyRegistratorWithFilter : ILibraryInconsistencyRegistrator
	{
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		private readonly ILibraryInconsistencyFilter inconsistencyFilter;

		public InconsistencyRegistratorWithFilter(ILibraryInconsistencyRegistrator inconsistencyRegistrator,
			ILibraryInconsistencyFilter inconsistencyFilter)
		{
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}
			if (inconsistencyFilter == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyFilter));
			}

			this.inconsistencyRegistrator = inconsistencyRegistrator;
			this.inconsistencyFilter = inconsistencyFilter;
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

		public void RegisterInconsistency_BadSongContent(Song song)
		{
			inconsistencyRegistrator.RegisterInconsistency_BadSongContent(song);
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
	}
}