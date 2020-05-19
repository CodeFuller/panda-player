using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb
{
	internal class StatisticsRepository : IStatisticsRepository
	{
		private readonly DiscLibrary discLibrary;

		public StatisticsRepository(DiscLibrary discLibrary)
		{
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken)
		{
			var statistics = new StatisticsModel
			{
				ArtistsNumber = discLibrary.Artists.Count(),
				DiscArtistsNumber = discLibrary.Discs.Select(d => d.Artist).Where(a => a != null).Distinct().Count(),
				DiscsNumber = discLibrary.Discs.Count(),
				SongsNumber = discLibrary.Songs.Count(),
				StorageSize = discLibrary.Songs.Sum(s => (long)s.FileSize) + discLibrary.Discs.SelectMany(d => d.Images).Sum(im => (long)im.FileSize),
				SongsDuration = discLibrary.Songs.Aggregate(TimeSpan.Zero, (currSum, currSong) => currSum + currSong.Duration),
				PlaybacksDuration = TimeSpan.FromTicks(discLibrary.AllSongs.Sum(song => song.PlaybacksCount * song.Duration.Ticks)),
				PlaybacksNumber = discLibrary.AllSongs.Sum(song => song.PlaybacksCount),
				UnheardSongsNumber = discLibrary.Songs.Count(s => s.PlaybacksCount == 0),
				UnratedSongsNumber = discLibrary.Songs.Count(s => s.Rating == null),
				NumberOfDiscsWithoutCoverImage = discLibrary.Discs.Count(disc => disc.CoverImage == null),
			};

			return Task.FromResult(statistics);
		}
	}
}
