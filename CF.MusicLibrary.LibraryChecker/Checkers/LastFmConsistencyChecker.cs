using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Configuration;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class LastFMConsistencyChecker : ILastFMConsistencyChecker
	{
		private readonly ILastFMApiClient lastFmApiClient;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;

		public LastFMConsistencyChecker(ILastFMApiClient lastFMApiClient, ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (lastFMApiClient == null)
			{
				throw new ArgumentNullException(nameof(lastFMApiClient));
			}
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.lastFmApiClient = lastFMApiClient;
			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public async Task CheckArtists(DiscLibrary library)
		{
			Logger.WriteInfo("Checking Last.fm artists ...");

			var username = AppSettings.GetRequiredValue<string>("LastFmUsername");

			foreach (var artist in library.Artists)
			{
				var artistInfo = await lastFmApiClient.GetArtistInfo(artist.Name, username);

				if (artistInfo.Artist == null)
				{
					inconsistencyRegistrator.RegisterInconsistency_ArtistNotFound(artist);
					continue;
				}

				if (artistInfo.Artist.Name != artist.Name)
				{
					inconsistencyRegistrator.RegisterInconsistency_ArtistNameCorrected(artist.Name, artistInfo.Artist.Name);
				}

				if (artistInfo.Artist.Stats.UserPlayCount == 0 &&
					library.Songs.Where(s => s.Artist?.Id == artist.Id)
						.Any(s => s.LastPlaybackTime.HasValue && s.LastPlaybackTime >= LastFMConstants.ScrobbleStartTime))
				{
					inconsistencyRegistrator.RegisterInconsistency_NoListensForArtist(artist);
				}
			}
		}

		public async Task CheckAlbums(IEnumerable<Disc> discs)
		{
			Logger.WriteInfo("Checking Last.fm albums ...");

			var username = AppSettings.GetRequiredValue<string>("LastFmUsername");

			//	Several disc could map to one album.
			//	That's why we remember what albums weere already checked.
			HashSet<Album> checkedAlbums = new HashSet<Album>();

			foreach (var disc in discs
				.Where(disc => disc.Artist != null)
				.Where(disc => disc.AlbumTitle != null))
			{
				var album = new Album(disc.Artist.Name, disc.AlbumTitle);
				if (checkedAlbums.Contains(album))
				{
					continue;
				}
				checkedAlbums.Add(album);

				var albumInfo = await lastFmApiClient.GetAlbumInfo(album, username);
				if (albumInfo.Album == null)
				{
					inconsistencyRegistrator.RegisterInconsistency_AlbumNotFound(disc);
				}
				else if (albumInfo.Album.UserPlayCount == 0 &&
					disc.Songs.Any(song => song.LastPlaybackTime.HasValue && song.LastPlaybackTime >= LastFMConstants.ScrobbleStartTime))
				{
					inconsistencyRegistrator.RegisterInconsistency_NoListensForAlbum(disc);
				}
			}
		}

		public async Task CheckSongs(IEnumerable<Song> songs)
		{
			Logger.WriteInfo("Checking Last.fm songs ...");

			var username = AppSettings.GetRequiredValue<string>("LastFmUsername");

			//	Several songs could map to one track.
			//	That's why we remember what tracks weere already checked.
			HashSet<Track> checkedTracks = new HashSet<Track>();

			foreach (var song in songs
				.Where(song => song.Artist != null)
				.Where(song => song.Duration >= LastFMConstants.MinScrobbledTrackLength)
				.Where(song => song.LastPlaybackTime.HasValue && song.LastPlaybackTime >= LastFMConstants.ScrobbleStartTime))
			{
				var track = new Track
				{
					Title = song.Title,
					Artist = song.Artist.Name,
				};
				if (checkedTracks.Contains(track))
				{
					continue;
				}
				checkedTracks.Add(track);

				var trackInfo = await lastFmApiClient.GetTrackInfo(track, username);
				if (trackInfo.Track == null)
				{
					inconsistencyRegistrator.RegisterInconsistency_SongNotFound(song);
					continue;
				}

				if (trackInfo.Track.Name != song.Title)
				{
					inconsistencyRegistrator.RegisterInconsistency_SongTitleCorrected(song, trackInfo.Track.Name);
				}

				if (trackInfo.Track.UserPlayCount == 0 &&
					(song.LastPlaybackTime.HasValue && song.LastPlaybackTime >= LastFMConstants.ScrobbleStartTime))
				{
					inconsistencyRegistrator.RegisterInconsistency_NoListensForSong(song);
				}
			}
		}
	}
}
