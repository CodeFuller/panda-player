using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class EditDiscPropertiesViewModelTests
	{
		[TestMethod]
		public void Load_ForActiveDisc_LoadsPropertiesCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Some Title",
				TreeTitle = "Some Tree Title",
				AlbumTitle = "Some Album Title",
				Year = 2021,
				AllSongs = new[]
				{
					new SongModel(),
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring" },
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			// Act

			target.Load(disc);

			// Assert

			target.IsDeleted.Should().BeFalse();
			target.Title.Should().Be("Some Title");
			target.TreeTitle.Should().Be("Some Tree Title");
			target.AlbumTitle.Should().Be("Some Album Title");
			target.Year.Should().Be(2021);
			target.DeleteComment.Should().BeNull();
		}

		[TestMethod]
		public void Load_ForDeletedDiscWithSameDeleteCommentForAllSongs_LoadsPropertiesCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Some Title",
				TreeTitle = "Some Tree Title",
				AlbumTitle = "Some Album Title",
				Year = 2021,
				AllSongs = new[]
				{
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring" },
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring" },
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			// Act

			target.Load(disc);

			// Assert

			target.IsDeleted.Should().BeTrue();
			target.Title.Should().Be("Some Title");
			target.TreeTitle.Should().Be("Some Tree Title");
			target.AlbumTitle.Should().Be("Some Album Title");
			target.Year.Should().Be(2021);
			target.DeleteComment.Should().Be("Boring");
		}

		[TestMethod]
		public void Load_ForDeletedDiscWithVariousDeleteCommentsForSongs_LoadsDeleteCommentCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Some Title",
				TreeTitle = "Some Tree Title",
				AllSongs = new[]
				{
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring" },
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring too" },
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			// Act

			target.Load(disc);

			// Assert

			target.IsDeleted.Should().BeTrue();
			target.DeleteComment.Should().Be("<Songs have various delete comments>");
		}

		[TestMethod]
		public async Task Save_ForActiveDisc_UpdatesDiscCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Old Title",
				TreeTitle = "Old Tree Title",
				AlbumTitle = "Old Album Title",
				Year = 2020,
				AllSongs = new[]
				{
					new SongModel(),
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring" },
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			target.Load(disc);

			// Act

			target.Title = "New Title";
			target.TreeTitle = "New Tree Title";
			target.AlbumTitle = "New Album Title";
			target.Year = 2021;

			await target.Save(CancellationToken.None);

			// Assert

			Func<Action<DiscModel>, bool> verifyDiscUpdate = updateAction =>
			{
				updateAction(disc);
				return disc.Title == "New Title" && disc.TreeTitle == "New Tree Title" && disc.AlbumTitle == "New Album Title" && disc.Year == 2021;
			};

			var discServiceMock = mocker.GetMock<IDiscsService>();
			discServiceMock.Verify(x => x.UpdateDisc(disc, It.Is<Action<DiscModel>>(y => verifyDiscUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task Save_ForDeletedDiscWithSameDeleteCommentWhenDeleteCommentWasNotChanged_UpdatesDiscCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Old Title",
				TreeTitle = "Old Tree Title",
				AlbumTitle = "Old Album Title",
				Year = 2021,
				AllSongs = new[]
				{
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment" },
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment" },
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			target.Load(disc);

			// Act

			target.Title = "New Title";
			target.TreeTitle = "New Tree Title";
			target.AlbumTitle = "New Album Title";
			target.Year = 2021;

			await target.Save(CancellationToken.None);

			// Assert

			Func<Action<DiscModel>, bool> verifyDiscUpdate = updateAction =>
			{
				updateAction(disc);
				return disc.Title == "New Title" && disc.TreeTitle == "New Tree Title" && disc.AlbumTitle == "New Album Title" && disc.Year == 2021;
			};

			var discServiceMock = mocker.GetMock<IDiscsService>();
			discServiceMock.Verify(x => x.UpdateDisc(disc, It.Is<Action<DiscModel>>(y => verifyDiscUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task Save_ForDeletedDiscWithSameDeleteCommentWhenDeleteCommentWasChanged_UpdatesDiscCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Old Delete Comment" },
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Old Delete Comment" },
			};

			var disc = new DiscModel
			{
				Title = "Old Title",
				TreeTitle = "Old Tree Title",
				AlbumTitle = "Old Album Title",
				Year = 2021,
				AllSongs = songs,
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			target.Load(disc);

			// Act

			target.Title = "New Title";
			target.TreeTitle = "New Tree Title";
			target.AlbumTitle = "New Album Title";
			target.DeleteComment = "New Delete Comment";
			target.Year = 2021;

			await target.Save(CancellationToken.None);

			// Assert

			Func<Action<DiscModel>, bool> verifyDiscUpdate = updateAction =>
			{
				updateAction(disc);
				return disc.Title == "New Title" && disc.TreeTitle == "New Tree Title" && disc.AlbumTitle == "New Album Title" && disc.Year == 2021;
			};

			var discServiceMock = mocker.GetMock<IDiscsService>();
			discServiceMock.Verify(x => x.UpdateDisc(disc, It.Is<Action<DiscModel>>(y => verifyDiscUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);

			Func<SongModel, Action<SongModel>, bool> verifySongUpdate = (song, updateAction) =>
			{
				updateAction(song);
				return song.DeleteComment == "New Delete Comment";
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songServiceMock.Verify(x => x.UpdateSong(songs[0], It.Is<Action<SongModel>>(y => verifySongUpdate(songs[0], y)), It.IsAny<CancellationToken>()), Times.Once);
			songServiceMock.Verify(x => x.UpdateSong(songs[1], It.Is<Action<SongModel>>(y => verifySongUpdate(songs[1], y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForDeletedDiscWithVariousDeleteCommentWhenDeleteCommentWasNotChanged_UpdatesDiscCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Old Title",
				TreeTitle = "Old Tree Title",
				AlbumTitle = "Old Album Title",
				Year = 2021,
				AllSongs = new[]
				{
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment 1" },
					new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment 2" },
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			target.Load(disc);

			// Act

			target.Title = "New Title";
			target.TreeTitle = "New Tree Title";
			target.AlbumTitle = "New Album Title";
			target.Year = 2021;

			await target.Save(CancellationToken.None);

			// Assert

			Func<Action<DiscModel>, bool> verifyDiscUpdate = updateAction =>
			{
				updateAction(disc);
				return disc.Title == "New Title" && disc.TreeTitle == "New Tree Title" && disc.AlbumTitle == "New Album Title" && disc.Year == 2021;
			};

			var discServiceMock = mocker.GetMock<IDiscsService>();
			discServiceMock.Verify(x => x.UpdateDisc(disc, It.Is<Action<DiscModel>>(y => verifyDiscUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task Save_ForDeletedDiscWithVariousDeleteCommentWhenDeleteCommentWasChanged_UpdatesDiscCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Old Delete Comment 1" },
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Old Delete Comment 2" },
			};

			var disc = new DiscModel
			{
				Title = "Old Title",
				TreeTitle = "Old Tree Title",
				AlbumTitle = "Old Album Title",
				Year = 2021,
				AllSongs = songs,
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<EditDiscPropertiesViewModel>();

			target.Load(disc);

			// Act

			target.Title = "New Title";
			target.TreeTitle = "New Tree Title";
			target.AlbumTitle = "New Album Title";
			target.DeleteComment = "New Delete Comment";
			target.Year = 2021;

			await target.Save(CancellationToken.None);

			// Assert

			Func<Action<DiscModel>, bool> verifyDiscUpdate = updateAction =>
			{
				updateAction(disc);
				return disc.Title == "New Title" && disc.TreeTitle == "New Tree Title" && disc.AlbumTitle == "New Album Title" && disc.Year == 2021;
			};

			var discServiceMock = mocker.GetMock<IDiscsService>();
			discServiceMock.Verify(x => x.UpdateDisc(disc, It.Is<Action<DiscModel>>(y => verifyDiscUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);

			Func<SongModel, Action<SongModel>, bool> verifySongUpdate = (song, updateAction) =>
			{
				updateAction(song);
				return song.DeleteComment == "New Delete Comment";
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songServiceMock.Verify(x => x.UpdateSong(songs[0], It.Is<Action<SongModel>>(y => verifySongUpdate(songs[0], y)), It.IsAny<CancellationToken>()), Times.Once);
			songServiceMock.Verify(x => x.UpdateSong(songs[1], It.Is<Action<SongModel>>(y => verifySongUpdate(songs[1], y)), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
