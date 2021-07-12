using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Adviser;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Adviser
{
	[TestClass]
	public class AdvisedPlaylistTests
	{
		[TestMethod]
		public void ForDisc_FillsPlaylistTypeCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			};

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			Assert.AreEqual(AdvisedPlaylistType.Disc, target.AdvisedPlaylistType);
		}

		[TestMethod]
		public void ForDisc_FillsTitleCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
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
			};

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			Assert.AreEqual("Some Artist / Some Disc", target.Title);
		}

		[TestMethod]
		public void ForDisc_DiscContainsDeletedSongs_FillsPlaylistOnlyWithActiveSongs()
		{
			// Arrange

			var deletedSong = new SongModel { Id = new ItemId("1"), DeleteDate = new DateTime(2017, 10, 06) };
			var activeSong1 = new SongModel { Id = new ItemId("2") };
			var activeSong2 = new SongModel { Id = new ItemId("3") };

			var disc = new DiscModel
			{
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new[] { deletedSong, activeSong1, activeSong2 },
			};

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			CollectionAssert.AreEqual(new[] { activeSong1, activeSong2 }, target.Songs.ToList());
		}

		[TestMethod]
		public void ForDisc_FillsDiscCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			};

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			Assert.AreSame(disc, target.Disc);
		}

		[TestMethod]
		public void ForFavoriteArtistDisc_FillsPlaylistTypeCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			};

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistDisc(disc);

			// Assert

			Assert.AreEqual(AdvisedPlaylistType.FavoriteArtistDisc, target.AdvisedPlaylistType);
		}

		[TestMethod]
		public void ForFavoriteArtistDisc_FillsTitleCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
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
			};

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistDisc(disc);

			// Assert

			Assert.AreEqual("*** Some Artist / Some Disc", target.Title);
		}

		[TestMethod]
		public void ForFavoriteArtistDisc_DiscContainsDeletedSongs_FillsPlaylistOnlyWithActiveSongs()
		{
			// Arrange

			var deletedSong = new SongModel { Id = new ItemId("1"), DeleteDate = new DateTime(2017, 10, 06) };
			var activeSong1 = new SongModel { Id = new ItemId("2") };
			var activeSong2 = new SongModel { Id = new ItemId("3") };

			var disc = new DiscModel
			{
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new[] { deletedSong, activeSong1, activeSong2 },
			};

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistDisc(disc);

			// Assert

			CollectionAssert.AreEqual(new[] { activeSong1, activeSong2 }, target.Songs.ToList());
		}

		[TestMethod]
		public void ForFavoriteArtistDisc_FillsDiscCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Folder = new FolderModel { Name = "Some Artist" },
				AllSongs = new List<SongModel>(),
			};

			// Act

			var target = AdvisedPlaylist.ForFavoriteArtistDisc(disc);

			// Assert

			Assert.AreSame(disc, target.Disc);
		}

		[TestMethod]
		public void ForHighlyRatedSongs_FillsPlaylistTypeCorrectly()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			// Assert

			Assert.AreEqual(AdvisedPlaylistType.HighlyRatedSongs, target.AdvisedPlaylistType);
		}

		[TestMethod]
		public void ForHighlyRatedSongs_FillsTitleCorrectly()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			// Assert

			Assert.AreEqual("Highly Rated Songs", target.Title);
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

			CollectionAssert.AreEqual(new[] { song1, song2 }, target.Songs.ToList());
		}

		[TestMethod]
		public void ForHighlyRatedSongs_DoesNotFillDisc()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			// Assert

			Assert.IsNull(target.Disc);
		}
	}
}
