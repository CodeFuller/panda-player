using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;

namespace MusicLibrary.PandaPlayer.UnitTests.Adviser.Extensions
{
	[TestClass]
	public class DiscModelExtensionsTests
	{
		[TestMethod]
		public void GetLastPlaybackTime_ForDiscWithAllActiveSongs_ReturnsMinLastPlaybackTimeAmongAllSongs()
		{
			// Arrange

			var songs = new[]
			{
				CreateTestSong(1, lastPlaybackTime: new DateTime(2021, 06, 29)),
				CreateTestSong(2, lastPlaybackTime: new DateTime(2021, 06, 27)),
				CreateTestSong(3, lastPlaybackTime: new DateTime(2021, 06, 28)),
			};

			var disc = CreateTestDisc(1, songs);

			// Act

			var lastPlaybackTime = disc.GetLastPlaybackTime();

			// Assert

			Assert.AreEqual(new DateTime(2021, 06, 27), lastPlaybackTime);
		}

		[TestMethod]
		public void GetLastPlaybackTime_ForDiscWithActiveAndDeletedSongs_ReturnsMinLastPlaybackTimeAmongActiveSongs()
		{
			// Arrange

			var songs = new[]
			{
				CreateTestSong(1, lastPlaybackTime: new DateTime(2021, 06, 29)),
				CreateTestSong(2, lastPlaybackTime: new DateTime(2021, 06, 27)),
				CreateTestSong(3, lastPlaybackTime: new DateTime(2021, 06, 26), isDeleted: true),
				CreateTestSong(4, lastPlaybackTime: new DateTime(2021, 06, 28)),
			};

			var disc = CreateTestDisc(1, songs);

			// Act

			var lastPlaybackTime = disc.GetLastPlaybackTime();

			// Assert

			Assert.AreEqual(new DateTime(2021, 06, 27), lastPlaybackTime);
		}

		[TestMethod]
		public void GetLastPlaybackTime_ForDeletedDisc_ReturnsMinLastPlaybackTimeAmongAllSongs()
		{
			// Arrange

			var songs = new[]
			{
				CreateTestSong(1, lastPlaybackTime: new DateTime(2021, 06, 29), isDeleted: true),
				CreateTestSong(2, lastPlaybackTime: new DateTime(2021, 06, 27), isDeleted: true),
				CreateTestSong(3, lastPlaybackTime: new DateTime(2021, 06, 28), isDeleted: true),
			};

			var disc = CreateTestDisc(1, songs);

			// Act

			var lastPlaybackTime = disc.GetLastPlaybackTime();

			// Assert

			Assert.AreEqual(new DateTime(2021, 06, 27), lastPlaybackTime);
		}

		private static SongModel CreateTestSong(int id, RatingModel? rating = null, DateTimeOffset? lastPlaybackTime = null, bool isDeleted = false)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				Rating = rating,
				LastPlaybackTime = lastPlaybackTime,
				DeleteDate = isDeleted ? new DateTime(2021, 06, 29) : null,
			};
		}

		private static DiscModel CreateTestDisc(int id, IEnumerable<SongModel> songs)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				Folder = new ShallowFolderModel(),
				AllSongs = songs.ToList(),
			};
		}
	}
}
