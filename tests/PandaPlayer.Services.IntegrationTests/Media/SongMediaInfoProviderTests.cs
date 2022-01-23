using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Services.Media;

namespace PandaPlayer.Services.IntegrationTests.Media
{
	[TestClass]
	public class SongMediaInfoProviderTests
	{
		[TestMethod]
		public async Task GetSongMediaInfo_ReturnsCorrectMediaInfo()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongMediaInfoProvider>();

			// Act

			var mediaInfo = await target.GetSongMediaInfo("Content/Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/01 - Про женщин.mp3");

			// Assert

			var expectedMediaInfo = new SongMediaInfo
			{
				Size = 405582,
				Bitrate = 320000,
				Duration = new TimeSpan(106268000),
			};

			mediaInfo.Should().BeEquivalentTo(expectedMediaInfo, x => x.WithStrictOrdering());
		}
	}
}
