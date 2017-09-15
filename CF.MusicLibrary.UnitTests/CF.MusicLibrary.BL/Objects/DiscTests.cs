using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.BL.Objects
{
	[TestFixture]
	public class DiscTests
	{
		[Test]
		public void LastPlaybackTime_WhenAllSongsHaveLastPlaybackTime_ReturnsEarliestSongLastPlaybackTime()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>()
				{
					new Song { LastPlaybackTime = new DateTime(2017, 11, 28), },
					new Song { LastPlaybackTime = new DateTime(2017, 03, 04), },
					new Song { LastPlaybackTime = new DateTime(2017, 05, 09), },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.AreEqual(new DateTime(2017, 03, 04), lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTime_WhenSomeSongsHaveNoLastPlaybackTime_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>()
				{
					new Song { LastPlaybackTime = new DateTime(2017, 11, 28), },
					new Song { LastPlaybackTime = new DateTime(2017, 03, 04), },
					new Song { LastPlaybackTime = null, },
					new Song { LastPlaybackTime = new DateTime(2017, 05, 09), },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.IsNull(lastPlaybackTime);
		}
	}
}
