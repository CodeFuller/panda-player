using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Application;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public class InconsistencyRegistratorToLog : ILibraryInconsistencyRegistrator
	{
		public void RegisterInconsistency_SuspiciousAlbumTitle(Disc disc)
		{
			LogInconsistency(Current($"Album title looks suspicious for {disc.Uri}: '{disc.AlbumTitle}'"));
		}

		public void RegisterInconsistency_DiscWithoutSongs(Disc disc)
		{
			LogInconsistency(Current($"Disc '{disc.Uri}' does not contain any songs"));
		}

		public void RegisterInconsistency_BadTrackNumbersForDisc(Disc disc, IEnumerable<short?> trackNumbers)
		{
			LogInconsistency(Current($"Disc '{disc.Uri}' contains bad track numbers: {String.Join(", ", trackNumbers)}"));
		}

		public void RegisterInconsistency_DifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres)
		{
			LogInconsistency(Current($"Disc '{disc.Uri}' contains different genres: {String.Join(", ", genres)}"));
		}

		public void RegisterInconsistency_BadTagData(string inconsistencyMessage)
		{
			LogInconsistency(inconsistencyMessage);
		}

		public void RegisterInconsistency_BadTagData(Song song, IEnumerable<SongTagType> tagTypes)
		{
			LogInconsistency(Current($"Bad tag types for {song.Uri + ":",-100}: [{String.Join(", ", tagTypes)}]"));
		}

		public void RegisterInconsistency_ArtistNotFound(Artist artist)
		{
			LogInconsistency(Current($"Artist not found on Last.fm: '{artist.Name}'"));
		}

		public void RegisterInconsistency_ArtistNameCorrected(string originalArtistName, string correctedArtistName)
		{
			LogInconsistency(Current($"Artist name corrected by Last.fm: '{originalArtistName}' -> '{correctedArtistName}'"));
		}

		public void RegisterInconsistency_NoListensForArtist(Artist artist)
		{
			LogInconsistency(Current($"No listens for artist on Last.fm: '{artist.Name}'"));
		}

		public void RegisterInconsistency_AlbumNotFound(Disc disc)
		{
			LogInconsistency(Current($"Album not found on Last.fm: '{disc.Uri}'"));
		}

		public void RegisterInconsistency_NoListensForAlbum(Disc disc)
		{
			LogInconsistency(Current($"No listens for album on Last.fm: '{disc.Uri}'"));
		}

		public void RegisterInconsistency_SongNotFound(Song song)
		{
			LogInconsistency(Current($"Song not found on Last.fm: '{song.Uri}'"));
		}

		public void RegisterInconsistency_SongTitleCorrected(Song song, string correctedSongTitle)
		{
			LogInconsistency(Current($"Song name corrected by Last.fm: '{song.Title}' -> '{correctedSongTitle}' for '{song.Uri}'"));
		}

		public void RegisterInconsistency_NoListensForSong(Song song)
		{
			LogInconsistency(Current($"No listens for song on Last.fm: '{song.Uri}'"));
		}

		public void RegisterInconsistency_MissingSongData(Song song)
		{
			LogInconsistency(Current($"Missing song data: '{song.Uri}'"));
		}

		public void RegisterInconsistency_LibraryData(string inconsistencyMessage)
		{
			LogInconsistency(inconsistencyMessage);
		}

		private static void LogInconsistency(string inconsistencyMessage)
		{
			Logger.WriteWarning(inconsistencyMessage);
		}
	}
}
