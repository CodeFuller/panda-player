using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Extensions;
using MusicLibrary.Logic.Comparers;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Services
{
	internal class StatisticsService : IStatisticsService
	{
		private readonly IDiscsRepository discsRepository;

		public StatisticsService(IDiscsRepository discsRepository)
		{
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
		}

		public async Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken)
		{
			var activeDiscs = (await discsRepository.GetAllDiscs(cancellationToken))
				.Where(disc => !disc.IsDeleted)
				.ToList();

			var allSongs = activeDiscs
				.SelectMany(disc => disc.Songs)
				.ToList();

			var activeSongs = allSongs
				.Where(song => !song.IsDeleted)
				.ToList();

			return new StatisticsModel
			{
				ArtistsNumber = activeSongs
					.Where(song => song.Artist != null)
					.Select(song => song.Artist)
					.Distinct(new ArtistEqualityComparer())
					.Count(),

				DiscArtistsNumber = activeDiscs
					.Select(GetDiscArtist)
					.Where(artist => artist != null)
					.Distinct(new ArtistEqualityComparer())
					.Count(),

				DiscsNumber = activeDiscs.Count,
				SongsNumber = activeSongs.Count,
				StorageSize = activeSongs.Sum(song => song.Size ?? 0) + activeDiscs.SelectMany(disc => disc.Images).Sum(image => image.Size),
				SongsDuration = activeSongs.Aggregate(TimeSpan.Zero, (sum, song) => sum + song.Duration),
				PlaybacksDuration = TimeSpan.FromTicks(allSongs.Sum(song => song.PlaybacksCount * song.Duration.Ticks)),
				PlaybacksNumber = allSongs.Sum(song => song.PlaybacksCount),
				UnheardSongsNumber = activeSongs.Count(s => s.PlaybacksCount == 0),
				UnratedSongsNumber = activeSongs.Count(s => s.Rating == null),
				NumberOfDiscsWithoutCoverImage = activeDiscs.Count(disc => disc.CoverImage == null),
			};
		}

		private static ArtistModel GetDiscArtist(DiscModel disc)
		{
			return disc.Songs
				.Where(song => !song.IsDeleted)
				.Select(song => song.Artist)
				.Where(artist => artist != null)
				.Distinct(new ArtistEqualityComparer())
				.UniqueOrDefault();
		}
	}
}
