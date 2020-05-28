using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Extensions;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
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
				.SelectMany(disc => disc.AllSongs)
				.ToList();

			var activeSongs = activeDiscs
				.SelectMany(disc => disc.ActiveSongs)
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
				StorageSize = activeSongs.Sum(song => song.Size ?? 0) + activeDiscs.SelectMany(disc => disc.Images).Sum(image => image.Size ?? 0),
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
			return disc.ActiveSongs
				.Select(song => song.Artist)
				.Where(artist => artist != null)
				.UniqueOrDefault(new ArtistEqualityComparer());
		}
	}
}
