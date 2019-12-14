using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryToolkit.Extensions;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using Microsoft.Extensions.Logging;
using MusicLibraryApi.Client.Contracts.Songs;
using MusicLibraryApi.Client.Interfaces;
using static System.FormattableString;

namespace CF.MusicLibrary.LibraryToolkit.Seeders
{
	public class SongsSeeder : ISongsSeeder
	{
		private readonly ISongsMutation songsMutation;

		private readonly ILogger<SongsSeeder> logger;

		public SongsSeeder(ISongsMutation songsMutation, ILogger<SongsSeeder> logger)
		{
			this.songsMutation = songsMutation ?? throw new ArgumentNullException(nameof(songsMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IReadOnlyDictionary<int, int>> SeedSongs(DiscLibrary discLibrary, IReadOnlyDictionary<int, int> discs,
			IReadOnlyDictionary<int, int> artists, IReadOnlyDictionary<int, int> genres, CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding songs ...");

			var songs = new Dictionary<int, int>();
			foreach (var song in discLibrary.AllSongs.OrderBy(s => s.Id))
			{
				if (!discs.TryGetValue(song.Disc.Id, out var discId))
				{
					throw new InvalidOperationException($"The new id for disc {song.Disc.Title} is unknown");
				}

				int? artistId = null;
				if (song.Artist != null)
				{
					if (!artists.TryGetValue(song.Artist.Id, out var newArtistId))
					{
						throw new InvalidOperationException($"The new id for artist {song.Artist.Name} is unknown");
					}

					artistId = newArtistId;
				}

				int? genreId = null;
				if (song.Genre != null)
				{
					if (!genres.TryGetValue(song.Genre.Id, out var newGenreId))
					{
						throw new InvalidOperationException($"The new id for genre {song.Genre.Name} is unknown");
					}

					genreId = newGenreId;
				}

				var trackNumberPart = song.TrackNumber == null ? String.Empty : Invariant($"{song.TrackNumber:D2}");
				var treeTitle = song.Uri.GetLastPart();

				// Playbacks data is seeded by playbacks seeder.
				var songData = new InputSongData(discId: discId, artistId: artistId, genreId: genreId, title: song.Title, treeTitle: treeTitle,
					trackNumber: song.TrackNumber, duration: song.Duration, rating: ConvertRating(song.Rating), bitRate: song.Bitrate,
					lastPlaybackTime: null, playbacksCount: 0, deleteDate: song.DeleteDate, deleteComment: song.DeleteDate != null ? String.Empty : null);

				var songId = await songsMutation.CreateSong(songData, cancellationToken);
				songs.Add(song.Id, songId);
			}

			logger.LogInformation("Seeded {SongsNumber} songs", songs.Count);

			return songs;
		}

		private static MusicLibraryApi.Client.Contracts.Songs.Rating? ConvertRating(Core.Objects.Rating? rating)
		{
			switch (rating)
			{
				case null:
					return null;

				case Core.Objects.Rating.R1:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R1;

				case Core.Objects.Rating.R2:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R2;

				case Core.Objects.Rating.R3:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R3;

				case Core.Objects.Rating.R4:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R4;

				case Core.Objects.Rating.R5:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R5;

				case Core.Objects.Rating.R6:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R6;

				case Core.Objects.Rating.R7:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R7;

				case Core.Objects.Rating.R8:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R8;

				case Core.Objects.Rating.R9:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R9;

				case Core.Objects.Rating.R10:
					return MusicLibraryApi.Client.Contracts.Songs.Rating.R10;

				case Core.Objects.Rating.Invalid:
				default:
					throw new InvalidOperationException($"Unknown rating value {rating}");
			}
		}
	}
}
