using System;
using System.Collections.Generic;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using Microsoft.Extensions.Logging;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public class InconsistencyRegistratorToLog : ILibraryInconsistencyRegistrator
	{
		private readonly ILogger<InconsistencyRegistratorToLog> logger;

		public InconsistencyRegistratorToLog(ILogger<InconsistencyRegistratorToLog> logger)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void RegisterSuspiciousAlbumTitle(Disc disc)
		{
			LogInconsistency(Current($"Album title looks suspicious for {disc.Uri}: '{disc.AlbumTitle}'"));
		}

		public void RegisterDiscWithoutSongs(Disc disc)
		{
			LogInconsistency(Current($"Disc '{disc.Uri}' does not contain any songs"));
		}

		public void RegisterBadTrackNumbersForDisc(Disc disc, IEnumerable<short?> trackNumbers)
		{
			LogInconsistency(Current($"Disc '{disc.Uri}' contains bad track numbers: {String.Join(", ", trackNumbers)}"));
		}

		public void RegisterDifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres)
		{
			LogInconsistency(Current($"Disc '{disc.Uri}' contains different genres: {String.Join(", ", genres)}"));
		}

		public void RegisterBadTagData(string inconsistencyMessage)
		{
			LogInconsistency(inconsistencyMessage);
		}

		public void RegisterBadTagData(Song song, IEnumerable<SongTagType> tagTypes)
		{
			LogInconsistency(Current($"Bad tag types for {song.Uri + ":",-100}: [{String.Join(", ", tagTypes)}]"));
		}

		public void RegisterArtistNotFound(Artist artist)
		{
			LogInconsistency(Current($"Artist not found on Last.fm: '{artist.Name}'"));
		}

		public void RegisterArtistNameCorrected(string originalArtistName, string correctedArtistName)
		{
			LogInconsistency(Current($"Artist name corrected by Last.fm: '{originalArtistName}' -> '{correctedArtistName}'"));
		}

		public void RegisterNoListensForArtist(Artist artist)
		{
			LogInconsistency(Current($"No listens for artist on Last.fm: '{artist.Name}'"));
		}

		public void RegisterAlbumNotFound(Disc disc)
		{
			LogInconsistency(Current($"Album not found on Last.fm: '{disc.Uri}'"));
		}

		public void RegisterNoListensForAlbum(Disc disc)
		{
			LogInconsistency(Current($"No listens for album on Last.fm: '{disc.Uri}'"));
		}

		public void RegisterSongNotFound(Song song)
		{
			LogInconsistency(Current($"Song not found on Last.fm: '{song.Uri}'"));
		}

		public void RegisterSongTitleCorrected(Song song, string correctedSongTitle)
		{
			LogInconsistency(Current($"Song name corrected by Last.fm: '{song.Title}' -> '{correctedSongTitle}' for '{song.Uri}'"));
		}

		public void RegisterNoListensForSong(Song song)
		{
			LogInconsistency(Current($"No listens for song on Last.fm: '{song.Uri}'"));
		}

		public void RegisterMissingStorageData(Uri itemUri)
		{
			LogInconsistency(Current($"Missing storage data: '{itemUri}'"));
		}

		public void RegisterUnexpectedStorageData(string itemPath, string itemType)
		{
			LogInconsistency(Current($"Detected unexpected {itemType} withing the storage: '{itemPath}'"));
		}

		public void RegisterErrorInStorageData(string errorMessage)
		{
			LogInconsistency(errorMessage);
		}

		public void RegisterFixOfErrorInStorageData(string fixMessage)
		{
			LogFix(fixMessage);
		}

		public void RegisterDiscCoverIsTooSmall(Disc disc, ImageInfo imageInfo)
		{
			LogInconsistency(Current($"Disc cover is too small: {imageInfo.Width} x {imageInfo.Height} for {disc.Uri}"));
		}

		public void RegisterDiscCoverIsTooBig(Disc disc, ImageInfo imageInfo)
		{
			LogInconsistency(Current($"Disc cover is too big: {imageInfo.Width} x {imageInfo.Height} for {disc.Uri}"));
		}

		public void RegisterImageFileIsTooBig(Disc disc, ImageInfo imageInfo)
		{
			LogInconsistency(Current($"Disc cover file is too big: {imageInfo.FileSize:n0} for {disc.Uri}"));
		}

		public void RegisterImageHasUnsupportedFormat(Disc disc, ImageInfo imageInfo)
		{
			LogInconsistency(Current($"Disc cover has unsupported format '{imageInfo.FormatName}' for {disc.Uri}"));
		}

		private void LogInconsistency(string inconsistencyMessage)
		{
			logger.LogWarning(inconsistencyMessage);
		}

		private void LogFix(string fixMessage)
		{
			logger.LogWarning(fixMessage);
		}
	}
}
