using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services
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
			var allDiscs = await discsRepository.GetAllDiscs(cancellationToken);

			var activeDiscs = allDiscs
				.Where(disc => !disc.IsDeleted)
				.ToList();

			var allSongs = allDiscs
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
			return disc.ActiveSongs
				.Select(song => song.Artist)
				.Where(artist => artist != null)
				.UniqueOrDefault(new ArtistEqualityComparer());
		}
	}
}
