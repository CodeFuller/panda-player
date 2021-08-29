﻿using System;
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

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.AdvisedPlaylistType.Should().Be(AdvisedPlaylistType.AdviseSet);
		}

		[TestMethod]
		public void ForAdviseSet_ForImplicitAdviseSet_FillsTitleCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				Title = "Some Disc",
				AllSongs = new List<SongModel>(),
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.Title.Should().Be("Some Artist / Some Disc");
		}

		[TestMethod]
		public void ForAdviseSet_ForExplicitAdviseSet_FillsTitleCorrectly()
		{
			// Arrange

			var adviseSet = new AdviseSetModel
			{
				Id = new ItemId("Advise Set Id"),
				Name = "Some Advise Set",
			};

			var disc1 = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				AdviseSet = adviseSet,
				Title = "Disc 1",
				AllSongs = new List<SongModel>(),
			};

			var disc2 = new DiscModel
			{
				Id = new ItemId("2"),
				Folder = new FolderModel { Name = "Some Artist" },
				AdviseSet = adviseSet,
				Title = "Disc 1",
				AllSongs = new List<SongModel>(),
			};

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

			var adviseSet = new AdviseSetModel
			{
				Id = new ItemId("Advise Set Id"),
				Name = "Some Advise Set",
			};

			var song1 = new SongModel { Id = new ItemId("Song 1.1") };
			var disc1 = new DiscModel
			{
				Id = new ItemId("1"),
				TreeTitle = "Disc 1",
				AdviseSet = adviseSet,
				AllSongs = new[] { song1 },
			};

			var song2 = new SongModel { Id = new ItemId("Song 2.1") };
			var disc2 = new DiscModel
			{
				Id = new ItemId("2"),
				TreeTitle = "Disc 2",
				AdviseSet = adviseSet,
				AllSongs = new[] { song2 },
			};

			var adviseSetContent = new AdviseSetContent("AdviseSetContent Id");

			// We add disc2 first on purpose.
			adviseSetContent.AddDisc(disc2);
			adviseSetContent.AddDisc(disc1);

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			var expectedSongs = new[]
			{
				song1,
				song2,
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

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new[] { deletedSong, activeSong1, activeSong2 },
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.Songs.Should().BeEquivalentTo(new[] { activeSong1, activeSong2 }, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ForAdviseSet_FillsAdviseSetCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForAdviseSet(adviseSetContent);

			// Assert

			target.AdviseSet.Should().BeSameAs(adviseSetContent);
		}

		[TestMethod]
		public void ForFavoriteArtistAdviseSet_FillsPlaylistTypeCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSetContent);

			// Assert

			target.AdvisedPlaylistType.Should().Be(AdvisedPlaylistType.FavoriteArtistAdviseSet);
		}

		[TestMethod]
		public void ForFavoriteArtistAdviseSet_FillsTitleCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				Title = "Some Disc",
				AllSongs = new[]
				{
					new SongModel
					{
						Id = new ItemId("12345"),
						Artist = new ArtistModel { Name = "Another Artist" },
					},
				},
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSetContent);

			// Assert

			target.Title.Should().Be("*** Some Artist / Some Disc");
		}

		[TestMethod]
		public void ForFavoriteArtistAdviseSet_IfAdviseSetContainsDeletedSongs_FillsPlaylistOnlyWithActiveSongs()
		{
			// Arrange

			var deletedSong = new SongModel { Id = new ItemId("1"), DeleteDate = new DateTime(2017, 10, 06) };
			var activeSong1 = new SongModel { Id = new ItemId("2") };
			var activeSong2 = new SongModel { Id = new ItemId("3") };

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new[] { deletedSong, activeSong1, activeSong2 },
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSetContent);

			// Assert

			target.Songs.Should().BeEquivalentTo(new[] { activeSong1, activeSong2 }, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ForFavoriteArtistAdviseSet_FillsAdviseSetCorrectly()
		{
			// Arrange

			var adviseSetContent = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			}.ToAdviseSet();

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSetContent);

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
