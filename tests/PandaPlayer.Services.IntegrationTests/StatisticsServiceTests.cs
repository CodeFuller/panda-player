using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class StatisticsServiceTests : BasicServiceTests<IStatisticsService>
	{
		[TestMethod]
		public async Task GetLibraryStatistics()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var statistics = await target.GetLibraryStatistics(CancellationToken.None);

			// Assert

			var expectedStatistics = new StatisticsModel
			{
				ArtistsNumber = 1,
				DiscArtistsNumber = 1,
				DiscsNumber = 2,
				SongsNumber = 3,
				StorageSize = 1614431,
				SongsDuration = TimeSpan.FromMilliseconds(32844),
				PlaybacksDuration = TimeSpan.FromMilliseconds(529191),
				PlaybacksNumber = 5,
				UnheardSongsNumber = 1,
				UnratedSongsNumber = 1,
				NumberOfDiscsWithoutCoverImage = 1,
			};

			statistics.Should().BeEquivalentTo(expectedStatistics);
		}
	}
}
