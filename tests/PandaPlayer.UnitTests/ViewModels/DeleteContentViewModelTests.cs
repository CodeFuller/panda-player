using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class DeleteContentViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void LoadForSongs_SetsCorrectConfirmationMessage()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel(),
				new SongModel(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DeleteContentViewModel>();

			// Act

			target.LoadForSongs(songs);

			// Assert

			target.ConfirmationMessage.Should().Be("Do you really want to delete 2 selected song(s)?");
		}

		[TestMethod]
		public void LoadForSongs_ForSubsequentCall_ClearsPreviousDeleteComment()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel(),
				new SongModel(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DeleteContentViewModel>();

			// Act

			target.LoadForSongs(songs);

			target.DeleteComment = "Some Delete Comment";

			target.LoadForSongs(songs);

			// Assert

			target.DeleteComment.Should().BeNull();
		}

		[TestMethod]
		public void LoadForDisc_ForSubsequentCall_ClearsPreviousDeleteComment()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Some Disc",
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DeleteContentViewModel>();

			// Act

			target.LoadForDisc(disc);

			target.DeleteComment = "Some Delete Comment";

			target.LoadForDisc(disc);

			// Assert

			target.DeleteComment.Should().BeNull();
		}

		[TestMethod]
		public void LoadForDisc_SetsCorrectConfirmationMessage()
		{
			// Arrange

			var disc = new DiscModel
			{
				Title = "Some Disc",
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DeleteContentViewModel>();

			// Act

			target.LoadForDisc(disc);

			// Assert

			target.ConfirmationMessage.Should().Be("Do you really want to delete the selected disc 'Some Disc'?");
		}

		[TestMethod]
		public async Task Delete_ForSongs_DeletesSongsCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel(),
				new SongModel(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DeleteContentViewModel>();

			target.LoadForSongs(songs);

			// Act

			target.DeleteComment = "Some Delete Comment";
			await target.Delete(CancellationToken.None);

			// Assert

			var songServiceMock = mocker.GetMock<ISongsService>();

			songServiceMock.Verify(x => x.DeleteSong(It.IsAny<SongModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songServiceMock.Verify(x => x.DeleteSong(songs[0], "Some Delete Comment", It.IsAny<CancellationToken>()), Times.Once);
			songServiceMock.Verify(x => x.DeleteSong(songs[1], "Some Delete Comment", It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Delete_ForDisc_DeletesDiscCorrectly()
		{
			// Arrange

			var disc = new DiscModel
			{
				Id = new ItemId("Disc Id"),
				Title = "Some Disc",
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DeleteContentViewModel>();

			target.LoadForDisc(disc);

			LibraryExplorerDiscChangedEventArgs libraryExplorerDiscChangedEvent = null;
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => e.RegisterEvent(ref libraryExplorerDiscChangedEvent));

			// Act

			target.DeleteComment = "Some Delete Comment";
			await target.Delete(CancellationToken.None);

			// Assert

			libraryExplorerDiscChangedEvent.Should().NotBeNull();
			libraryExplorerDiscChangedEvent.Disc.Should().BeNull();

			var discServiceMock = mocker.GetMock<IDiscsService>();

			discServiceMock.Verify(x => x.DeleteDisc(It.IsAny<ItemId>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
			discServiceMock.Verify(x => x.DeleteDisc(new ItemId("Disc Id"), "Some Delete Comment", It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
