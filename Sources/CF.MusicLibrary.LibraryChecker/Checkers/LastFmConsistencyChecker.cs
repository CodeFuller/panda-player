using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class LastFMConsistencyChecker : ILastFMConsistencyChecker
	{
		private readonly ILastFMApiClient lastFmApiClient;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		private readonly ICheckScope checkScope;
		private readonly ILogger<LastFMConsistencyChecker> logger;
		private readonly CheckingSettings settings;

		public LastFMConsistencyChecker(ILastFMApiClient lastFMApiClient, ILibraryInconsistencyRegistrator inconsistencyRegistrator,
			ICheckScope checkScope, ILogger<LastFMConsistencyChecker> logger, IOptions<CheckingSettings> options)
		{
			this.lastFmApiClient = lastFMApiClient ?? throw new ArgumentNullException(nameof(lastFMApiClient));
			this.inconsistencyRegistrator = inconsistencyRegistrator ?? throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			this.checkScope = checkScope ?? throw new ArgumentNullException(nameof(checkScope));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task CheckArtists(DiscLibrary library, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking Last.fm artists ...");

			foreach (var artist in library.Songs
				.Where(checkScope.Contains)
				.Select(s => s.Artist)
				.Where(a => a != null)
				.OrderBy(a => a.Name))
			{
				var artistInfo = await lastFmApiClient.GetArtistInfo(artist.Name, settings.LastFmUsername);

				if (artistInfo.Artist == null)
				{
					inconsistencyRegistrator.RegisterArtistNotFound(artist);
					continue;
				}

				if (artistInfo.Artist.Name != artist.Name)
				{
					inconsistencyRegistrator.RegisterArtistNameCorrected(artist.Name, artistInfo.Artist.Name);
				}

				if (artistInfo.Artist.Stats.UserPlayCount == 0 &&
					library.Songs.Where(s => s.Artist?.Id == artist.Id)
						.Any(s => s.LastPlaybackTime.HasValue && s.LastPlaybackTime >= LastFMConstants.ScrobbleStartTime))
				{
					inconsistencyRegistrator.RegisterNoListensForArtist(artist);
				}
			}
		}

		public async Task CheckAlbums(IEnumerable<Disc> discs, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking Last.fm albums ...");

			// Several disc could map to one album.
			// That's why we remember what albums weere already checked.
			HashSet<Album> checkedAlbums = new HashSet<Album>();

			foreach (var disc in discs
				.Where(checkScope.Contains)
				.Where(disc => disc.Artist != null)
				.Where(disc => disc.AlbumTitle != null))
			{
				var album = new Album(disc.Artist.Name, disc.AlbumTitle);
				if (checkedAlbums.Contains(album))
				{
					continue;
				}

				checkedAlbums.Add(album);

				var albumInfo = await lastFmApiClient.GetAlbumInfo(album, settings.LastFmUsername);
				if (albumInfo.Album == null)
				{
					inconsistencyRegistrator.RegisterAlbumNotFound(disc);
				}
				else if (albumInfo.Album.UserPlayCount == 0 &&
					disc.Songs.Any(song => song.LastPlaybackTime.HasValue && song.LastPlaybackTime >= LastFMConstants.ScrobbleStartTime))
				{
					inconsistencyRegistrator.RegisterNoListensForAlbum(disc);
				}
			}
		}

		public async Task CheckSongs(IEnumerable<Song> songs, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking Last.fm songs ...");

			// Several songs could map to one track.
			// That's why we remember what tracks weere already checked.
			HashSet<Track> checkedTracks = new HashSet<Track>();

			foreach (var song in songs
				.Where(checkScope.Contains)
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

				var trackInfo = await lastFmApiClient.GetTrackInfo(track, settings.LastFmUsername);
				if (trackInfo.Track == null)
				{
					inconsistencyRegistrator.RegisterSongNotFound(song);
					continue;
				}

				if (trackInfo.Track.Name != song.Title)
				{
					inconsistencyRegistrator.RegisterSongTitleCorrected(song, trackInfo.Track.Name);
				}

				if (trackInfo.Track.UserPlayCount == 0 &&
					(song.LastPlaybackTime.HasValue && song.LastPlaybackTime >= LastFMConstants.ScrobbleStartTime))
				{
					inconsistencyRegistrator.RegisterNoListensForSong(song);
				}
			}
		}
	}
}
