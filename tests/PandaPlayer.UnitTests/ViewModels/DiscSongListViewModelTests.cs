using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class DiscSongListViewModelTests
	{
		[TestMethod]
		public void DisplayTrackNumbersGetter_ReturnsTrue()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			// Act

			var displayTrackNumbers = target.DisplayTrackNumbers;

			// Assert

			displayTrackNumbers.Should().BeTrue();
		}

		[TestMethod]
		public void ContextMenuItems_IfNoSongsSelected_ReturnsEmptyCollection()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(songs);

			// Act

			var menuItems = target.ContextMenuItems;

			// Assert

			menuItems.Should().BeEmpty();
		}

		[TestMethod]
		public void ContextMenuItems_IfActiveAndDeletedSongsSelected_ReturnsEmptyCollection()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1"), DeleteDate = new DateTime(2021, 10, 25) },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(songs);
			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[0],
				target.SongItems[1],
			};

			// Act

			var menuItems = target.ContextMenuItems;

			// Assert

			menuItems.Should().BeEmpty();
		}

		[TestMethod]
		public void ContextMenuItems_IfActiveSongsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1"), DeleteDate = new DateTime(2021, 10, 25) },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(songs);
			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			var menuItems = target.ContextMenuItems;

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new CommandMenuItem(() => { }) { Header = "Play Next", IconKind = PackIconKind.PlaylistAdd },
				new CommandMenuItem(() => { }) { Header = "Play Last", IconKind = PackIconKind.PlaylistAdd },
				new ExpandableMenuItem
				{
					Header = "Set Rating",
					IconKind = PackIconKind.Star,
					Items = new[]
					{
						new SetRatingMenuItem(RatingModel.R10, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R9, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R8, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R7, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R6, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R5, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R4, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R3, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R2, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R1, () => Task.CompletedTask),
					},
				},
				new CommandMenuItem(() => { }) { Header = "Update Content", IconKind = PackIconKind.Refresh },
				new CommandMenuItem(() => { }) { Header = "Delete From Disc", IconKind = PackIconKind.DeleteForever },
				new CommandMenuItem(() => { }) { Header = "Properties", IconKind = PackIconKind.Pencil },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().PreferringRuntimeMemberTypes());
		}

		[TestMethod]
		public void ContextMenuItems_IfDeletedSongsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0"), DeleteDate = new DateTime(2021, 10, 25) },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2"), DeleteDate = new DateTime(2021, 10, 25) },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(songs);
			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			var menuItems = target.ContextMenuItems;

			// Assert

			var expectedMenuItems = new[]
			{
				new CommandMenuItem(() => { }) { Header = "Properties", IconKind = PackIconKind.Pencil },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void DeleteSongsFromDisc_InvokesDeleteDiscSongsView()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(songs);

			// Act

			target.DeleteSongsFromDisc(new[] { songs[0], songs[2] });

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowDeleteDiscSongsView(new[] { songs[0], songs[2] }), Times.Once);
		}

		[TestMethod]
		public void DeleteSongsFromDisc_IfSongsAreDeleted_SongItemsAreRemovedFromList()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("3") },
			};

			var disc = new DiscModel();
			disc.AddSongs(songs);

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			mocker.GetMock<IViewNavigator>().Setup(x => x.ShowDeleteDiscSongsView(It.IsAny<IReadOnlyCollection<SongModel>>()))
				.Callback<IReadOnlyCollection<SongModel>>(deletedSongs =>
				{
					foreach (var song in deletedSongs)
					{
						song.MarkAsDeleted(new DateTime(2021, 07, 11), "Test deletion");
					}
				})
				.Returns(true);

			mocker.SendMessage(new LibraryExplorerDiscChangedEventArgs(disc, deletedContentIsShown: false));

			// Act

			target.DeleteSongsFromDisc(new[] { songs[0], songs[2] });

			// Assert

			var expectedSongs = new[]
			{
				songs[1],
				songs[3],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void DeleteSongsFromDisc_IfSongsAreNotDeleted_DoesNotReloadSongList()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("3") },
			};

			var disc = new DiscModel();
			disc.AddSongs(songs);

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			mocker.GetMock<IViewNavigator>().Setup(x => x.ShowDeleteDiscSongsView(It.IsAny<IReadOnlyCollection<SongModel>>()))
				.Callback<IReadOnlyCollection<SongModel>>(deletedSongs =>
				{
					foreach (var song in deletedSongs)
					{
						// In real life when ShowDeleteDiscSongsView returns false, the songs are not actually deleted.
						// This is just a convenient way to check that song list is not reloaded.
						song.MarkAsDeleted(new DateTime(2021, 07, 11), "Test deletion");
					}
				})
				.Returns(false);

			mocker.SendMessage(new LibraryExplorerDiscChangedEventArgs(disc, deletedContentIsShown: false));

			// Act

			target.DeleteSongsFromDisc(new[] { songs[0], songs[2] });

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void LibraryExplorerDiscChangedEventHandler_IfDeletedContentIsNotShown_FillsListWithActiveDiscSongs()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("Old 0") },
				new SongModel { Id = new ItemId("Old 1") },
			};

			var newSongs = new[]
			{
				new SongModel { Id = new ItemId("New 0") },
				new SongModel { Id = new ItemId("New 1"), DeleteDate = new DateTime(2021, 07, 25) },
				new SongModel { Id = new ItemId("New 2") },
			};

			var newDisc = new DiscModel();
			newDisc.AddSongs(newSongs);

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(oldSongs);

			// Act

			mocker.SendMessage(new LibraryExplorerDiscChangedEventArgs(newDisc, deletedContentIsShown: false));

			// Assert

			var expectedSongs = new[]
			{
				newSongs[0],
				newSongs[2],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void LibraryExplorerDiscChangedEventHandler_IfDeletedContentIsShown_FillsListWithActiveDiscSongs()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("Old 0") },
				new SongModel { Id = new ItemId("Old 1") },
			};

			var newSongs = new[]
			{
				new SongModel { Id = new ItemId("New 0") },
				new SongModel { Id = new ItemId("New 1"), DeleteDate = new DateTime(2021, 07, 25) },
				new SongModel { Id = new ItemId("New 2") },
			};

			var newDisc = new DiscModel();
			newDisc.AddSongs(newSongs);

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(oldSongs);

			// Act

			mocker.SendMessage(new LibraryExplorerDiscChangedEventArgs(newDisc, deletedContentIsShown: true));

			// Assert

			target.Songs.Should().BeEquivalentTo(newSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void LibraryExplorerDiscChangedEventHandler_IfDiscIsNull_ClearsSongList()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("Old 0") },
				new SongModel { Id = new ItemId("Old 1") },
			};

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(oldSongs);

			// Act

			mocker.SendMessage(new LibraryExplorerDiscChangedEventArgs(null, deletedContentIsShown: false));

			// Assert

			target.Songs.Should().BeEmpty();
		}
	}
}
