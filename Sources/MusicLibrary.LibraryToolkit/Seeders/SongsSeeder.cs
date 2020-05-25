using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicLibrary.LibraryToolkit.Interfaces;
using MusicLibrary.LibraryToolkit.Settings;
using MusicLibraryApi.Client.Interfaces;

namespace MusicLibrary.LibraryToolkit.Seeders
{
	public class SongsSeeder : ISongsSeeder
	{
		private readonly ISongsMutation songsMutation;

		private readonly ILogger<SongsSeeder> logger;

		private readonly SongsSeederSettings settings;

		public SongsSeeder(ISongsMutation songsMutation, ILogger<SongsSeeder> logger, IOptions<SongsSeederSettings> options)
		{
			this.songsMutation = songsMutation ?? throw new ArgumentNullException(nameof(songsMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public Task<IReadOnlyDictionary<int, int>> SeedSongs(IReadOnlyDictionary<int, int> discs, IReadOnlyDictionary<int, int> artists,
			IReadOnlyDictionary<int, int> genres, CancellationToken cancellationToken)
		{
			// TODO: Restore this functionality or delete it completely.
			throw new NotImplementedException();
/*
			logger.LogInformation("Seeding songs ...");

			var songsExplicitInfo = LoadExplicitSongsInfo();

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

				if (songsExplicitInfo.TryGetValue(song.Uri, out var treeTitle))
				{
					// Removing explicit info entries for later sanity check.
					songsExplicitInfo.Remove(song.Uri);
				}
				else
				{
					treeTitle = song.Uri.GetLastPart();
				}

				var songData = new InputSongData
				{
					DiscId = discId,
					ArtistId = artistId,
					GenreId = genreId,
					Title = song.Title,
					TreeTitle = treeTitle,
					TrackNumber = song.TrackNumber,
					Duration = song.Duration,
					Rating = ConvertRating(song.Rating),
					BitRate = song.Bitrate,
					DeleteDate = song.DeleteDate,
					DeleteComment = song.DeleteDate != null ? String.Empty : null,
				};

				int songId;
				if (song.IsDeleted)
				{
					songId = await songsMutation.CreateDeletedSong(songData, cancellationToken);
				}
				else
				{
					var songFilePath = await musicLibraryReader.GetSongFile(song);
					using (var contentStream = File.OpenRead(songFilePath))
					{
						songId = await songsMutation.CreateSong(songData, contentStream, cancellationToken);
					}
				}

				songs.Add(song.Id, songId);
			}

			if (songsExplicitInfo.Any())
			{
				var unusedExplicitSongsUris = String.Join("\n", songsExplicitInfo.Keys);
				logger.LogCritical("The following explicit songs info was not used:\n\n{UnusedExplicitSongsUris}", unusedExplicitSongsUris);
				throw new InvalidOperationException("Some explicit songs info was not used");
			}

			logger.LogInformation("Seeded {SongsNumber} songs", songs.Count);

			return songs;
*/
		}

/*
		private IDictionary<Uri, string> LoadExplicitSongsInfo()
		{
			var songsInfo = new Dictionary<Uri, string>();

			if (String.IsNullOrEmpty(settings.ExplicitSongsInfoFile))
			{
				return songsInfo;
			}

			foreach (var line in File.ReadLines(settings.ExplicitSongsInfoFile))
			{
				if (line.Length == 0 || line.StartsWith(";", StringComparison.Ordinal))
				{
					continue;
				}

				var values = line.Split('\t');
				if (values.Length != 3)
				{
					throw new InvalidOperationException($"File '{settings.ExplicitSongsInfoFile}' contains invalid line: '{line}'");
				}

				// values[1] (song title) is only for easier file processing by a human, it is not used by the seeder.
				songsInfo.Add(new Uri(values[0], UriKind.Relative), values[2]);
			}

			return songsInfo;
		}

		private static MusicLibraryApi.Client.Contracts.Rating? ConvertRating(Core.Objects.Rating? rating)
		{
			switch (rating)
			{
				case null:
					return null;

				case Core.Objects.Rating.R1:
					return MusicLibraryApi.Client.Contracts.Rating.R1;

				case Core.Objects.Rating.R2:
					return MusicLibraryApi.Client.Contracts.Rating.R2;

				case Core.Objects.Rating.R3:
					return MusicLibraryApi.Client.Contracts.Rating.R3;

				case Core.Objects.Rating.R4:
					return MusicLibraryApi.Client.Contracts.Rating.R4;

				case Core.Objects.Rating.R5:
					return MusicLibraryApi.Client.Contracts.Rating.R5;

				case Core.Objects.Rating.R6:
					return MusicLibraryApi.Client.Contracts.Rating.R6;

				case Core.Objects.Rating.R7:
					return MusicLibraryApi.Client.Contracts.Rating.R7;

				case Core.Objects.Rating.R8:
					return MusicLibraryApi.Client.Contracts.Rating.R8;

				case Core.Objects.Rating.R9:
					return MusicLibraryApi.Client.Contracts.Rating.R9;

				case Core.Objects.Rating.R10:
					return MusicLibraryApi.Client.Contracts.Rating.R10;

				case Core.Objects.Rating.Invalid:
				default:
					throw new InvalidOperationException($"Unknown rating value {rating}");
			}
		}
*/
	}
}
