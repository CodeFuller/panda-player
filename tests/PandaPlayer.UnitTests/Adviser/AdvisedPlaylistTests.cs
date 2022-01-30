using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Extensions;

namespace PandaPlayer.UnitTests.Adviser
{
	[TestClass]
	public class AdvisedPlaylistTests
	{
		[TestMethod]
		public void ForAdviseSet_FillsPlaylistTypeCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel { Id = new ItemId("1") }
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.AdvisedPlaylistType.Should().Be(AdvisedPlaylistType.AdviseSet);
		}

		[TestMethod]
		public void ForAdviseSet_ForImplicitAdviseSet_FillsTitleCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel { Id = new ItemId("1"), Title = "Some Disc" }
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.Title.Should().Be("Some Artist / Some Disc");
		}

		[TestMethod]
		public void ForAdviseSet_ForExplicitAdviseSet_FillsTitleCorrectly()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("Advise Set Id"), Name = "Some Advise Set" };

			var disc1 = new DiscModel { Id = new ItemId("1"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 1), Title = "Disc 1" };
			var disc2 = new DiscModel { Id = new ItemId("2"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 2), Title = "Disc 2" };

			var folder = new FolderModel { Name = "Some Artist" };
			folder.AddDiscs(disc1, disc2);

			var adviseSetContent = new AdviseSetContent("AdviseSetContent Id");
			adviseSetContent.AddDisc(disc1);
			adviseSetContent.AddDisc(disc2);

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.Title.Should().Be("Some Advise Set");
		}

		[TestMethod]
		public void ForAdviseSet_IfAdviseSetContainsMultipleDiscs_OrdersPlaylistSongsCorrectly()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("Advise Set Id"), Name = "Some Advise Set" };

			var song11 = new SongModel { Id = new ItemId("Song 1.1") };
			var song12 = new SongModel { Id = new ItemId("Song 1.2") };
			var disc1 = new DiscModel
			{
				Id = new ItemId("1"),
				TreeTitle = "Disc 1",
				AdviseSetInfo = new AdviseSetInfo(adviseSet, 2),
				AllSongs = new[] { song11, song12 },
			};

			var song2 = new SongModel { Id = new ItemId("Song 2.1") };
			var disc2 = new DiscModel
			{
				Id = new ItemId("2"),
				TreeTitle = "Disc 2",
				AdviseSetInfo = new AdviseSetInfo(adviseSet, 1),
				AllSongs = new[] { song2 },
			};

			var song3 = new SongModel { Id = new ItemId("Song 3.1") };
			var disc3 = new DiscModel
			{
				Id = new ItemId("3"),
				TreeTitle = "Disc 3",
				AdviseSetInfo = new AdviseSetInfo(adviseSet, 3),
				AllSongs = new[] { song3 },
			};

			var adviseSetContent = new AdviseSetContent("AdviseSetContent Id");

			adviseSetContent.AddDisc(disc1);
			adviseSetContent.AddDisc(disc2);
			adviseSetContent.AddDisc(disc3);

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			var expectedSongs = new[]
			{
				song2,
				song11,
				song12,
				song3,
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ForAdviseSet_IfAdviseSetContainsDeletedSongs_FillsPlaylistOnlyWithActiveSongs()
		{
			// Arrange

			var deletedSong = new SongModel { Id = new ItemId("1"), DeleteDate = new DateTime(2017, 10, 06) };
			var activeSong1 = new SongModel { Id = new ItemId("2") };
			var activeSong2 = new SongModel { Id = new ItemId("3") };

			var adviseSetContent = new DiscModel { Id = new ItemId("1"), AllSongs = new[] { deletedSong, activeSong1, activeSong2 } }
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.Songs.Should().BeEquivalentTo(new[] { activeSong1, activeSong2 }, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ForAdviseSet_FillsAdviseSetCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel { Id = new ItemId("1") }
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.AdviseSet.Should().BeSameAs(adviseSetContent);
		}

		[TestMethod]
		public void ForAdviseSetFromFavoriteAdviseGroup_FillsPlaylistTypeCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel { Id = new ItemId("1") }
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSetContent);

			// Assert

			target.AdvisedPlaylistType.Should().Be(AdvisedPlaylistType.AdviseSetFromFavoriteAdviseGroup);
		}

		[TestMethod]
		public void ForAdviseSetFromFavoriteAdviseGroup_FillsTitleCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel
				{
					Id = new ItemId("1"),
					Title = "Some Disc",
					AllSongs = new[]
					{
						new SongModel
						{
							Id = new ItemId("12345"),
							Artist = new ArtistModel { Name = "Another Artist" },
						},
					},
				}
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSetContent);

			// Assert

			target.Title.Should().Be("*** Some Artist / Some Disc");
		}

		[TestMethod]
		public void ForAdviseSetFromFavoriteAdviseGroup_IfAdviseSetContainsDeletedSongs_FillsPlaylistOnlyWithActiveSongs()
		{
			// Arrange

			var deletedSong = new SongModel { Id = new ItemId("1"), DeleteDate = new DateTime(2017, 10, 06) };
			var activeSong1 = new SongModel { Id = new ItemId("2") };
			var activeSong2 = new SongModel { Id = new ItemId("3") };

			var adviseSetContent = new DiscModel
				{
					Id = new ItemId("1"),
					AllSongs = new[] { deletedSong, activeSong1, activeSong2 },
				}
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSetContent);

			// Assert

			target.Songs.Should().BeEquivalentTo(new[] { activeSong1, activeSong2 }, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ForAdviseSetFromFavoriteAdviseGroup_FillsAdviseSetCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel
				{
					Id = new ItemId("1"),
					AllSongs = new List<SongModel>(),
				}
				.AddToFolder(new FolderModel { Name = "Some Artist" })
				.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSetContent);

			// Assert

			target.AdviseSet.Should().BeSameAs(adviseSetContent);
		}

		[TestMethod]
		public void ForHighlyRatedSongs_FillsPlaylistTypeCorrectly()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			// Assert

			target.AdvisedPlaylistType.Should().Be(AdvisedPlaylistType.HighlyRatedSongs);
		}

		[TestMethod]
		public void ForHighlyRatedSongs_FillsTitleCorrectly()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			// Assert

			target.Title.Should().Be("Highly Rated Songs");
		}

		[TestMethod]
		public void ForHighlyRatedSongs_FillsSongsCorrectly()
		{
			// Arrange

			var song1 = new SongModel();
			var song2 = new SongModel();

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(new[] { song1, song2 });

			// Assert

			target.Songs.Should().BeEquivalentTo(new[] { song1, song2 }, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ForHighlyRatedSongs_DoesNotFillAdviseSet()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			// Assert

			target.AdviseSet.Should().BeNull();
		}
	}
}
