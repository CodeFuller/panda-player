using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class ApplicationViewModelTests
	{
		[TestMethod]
		public void Load_SendsApplicationLoadedEvent()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			target.LoadCommand.Execute(null);

			// Assert

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<ApplicationLoadedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Once);
		}

		[TestMethod]
		public void ReversePlayingCommand_ReversesPlayingOnPlaylistPlayer()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			target.ReversePlayingCommand.Execute(null);

			// Assert

			mocker.GetMock<IPlaylistPlayerViewModel>().Verify(x => x.ReversePlaying(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void ShowDiscAdderCommand_ShowsDiscAdderOnViewNavigator()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			target.ShowDiscAdderCommand.Execute(null);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowDiscAdderView(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void ShowLibraryCheckerCommand_ShowsLibraryCheckerViewOnViewNavigator()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			target.ShowLibraryCheckerCommand.Execute(null);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowLibraryCheckerView(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void ShowLibraryStatisticsCommand_ShowsLibraryStatisticsViewOnViewNavigator()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			target.ShowLibraryStatisticsCommand.Execute(null);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowLibraryStatisticsView(It.IsAny<CancellationToken>()), Times.Once);
		}

		// We have single test for PlaylistLoadedEvent and all other tests for PlaylistChangedEvent.
		// Both events are handled by the same code.
		// And there is no trivial way to share tests for both events. Messenger will not invoke handler if compile-time type of event is BasicPlaylistEventArgs.
		// Approach with generic test case turns ugly.
		[TestMethod]
		public void PlaylistLoadedEventHandler_IfCurrentSongHasArtist_ReturnsCorrectTitle()
		{
			// Arrange

			var song = new SongModel
			{
				Title = "Some Song",
				Artist = new ArtistModel
				{
					Name = "Some Artist",
				},
			};

			var playlistLoadedEvent = new PlaylistChangedEventArgs(new[] { song }, song, 0);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			mocker.SendMessage(playlistLoadedEvent);

			// Assert

			target.Title.Should().Be("1/1 - Some Artist - Some Song");
		}

		[TestMethod]
		public void PlaylistChangedEventHandler_WhenTitleIsSet_SendsPropertyChangedEvent()
		{
			// Arrange

			var song = new SongModel { Title = "Some Song" };
			var playlistChangedEvent = new PlaylistChangedEventArgs(new[] { song }, song, 0);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			mocker.SendMessage(playlistChangedEvent);

			// Assert

			var expectedProperties = new[]
			{
				nameof(ApplicationViewModel.Title),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void PlaylistChangedEventHandler_IfCurrentSongIsNull_ReturnsDefaultTitle()
		{
			// Arrange

			var song = new SongModel { Title = "Some Song" };
			var playlistChangedEventWithCurrentSong = new PlaylistChangedEventArgs(new[] { song }, song, 0);
			var playlistChangedEventWithNoCurrentSong = new PlaylistChangedEventArgs(new[] { song }, null, null);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			mocker.SendMessage(playlistChangedEventWithCurrentSong);
			target.Title.Should().NotBe("Panda Player");

			// Act

			mocker.SendMessage(playlistChangedEventWithNoCurrentSong);

			// Assert

			target.Title.Should().Be("Panda Player");
		}

		[TestMethod]
		public void PlaylistChangedEventHandler_IfCurrentSongHasNoArtist_ReturnsCorrectTitle()
		{
			// Arrange

			var song = new SongModel { Title = "Some Song" };
			var playlistChangedEvent = new PlaylistChangedEventArgs(new[] { song }, song, 0);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			mocker.SendMessage(playlistChangedEvent);

			// Assert

			target.Title.Should().Be("1/1 - Some Song");
		}

		[TestMethod]
		public void PlaylistChangedEventHandler_IfCurrentSongHasArtist_ReturnsCorrectTitle()
		{
			// Arrange

			var song = new SongModel
			{
				Title = "Some Song",
				Artist = new ArtistModel
				{
					Name = "Some Artist",
				},
			};

			var playlistChangedEvent = new PlaylistChangedEventArgs(new[] { song }, song, 0);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			mocker.SendMessage(playlistChangedEvent);

			// Assert

			target.Title.Should().Be("1/1 - Some Artist - Some Song");
		}

		[TestMethod]
		public void PlaylistChangedEventHandler_PlaylistHasMultipleSongs_SetsTrackNumberInfoCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Title = "First Song" },
				new SongModel { Title = "Second Song" },
				new SongModel { Title = "Third Song" },
			};

			var playlistChangedEvent = new PlaylistChangedEventArgs(songs, songs[1], 1);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			mocker.SendMessage(playlistChangedEvent);

			// Assert

			target.Title.Should().Be("2/3 - Second Song");
		}

		[TestMethod]
		public void PlaylistFinishedEventHandler_SomeSongsHaveNoRatingSet_ShowsRatePlaylistSongsView()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Rating = RatingModel.R5 },
				new SongModel { Rating = null },
				new SongModel { Rating = RatingModel.R8 },
			};

			var playlistFinishedEvent = new PlaylistFinishedEventArgs(songs);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			mocker.SendMessage(playlistFinishedEvent);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowRatePlaylistSongsView(songs), Times.Once);
		}

		[TestMethod]
		public void PlaylistFinishedEventHandler_AllSongsHaveRatingSet_DoesNotShowRatePlaylistSongsView()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Rating = RatingModel.R5 },
				new SongModel { Rating = RatingModel.R8 },
			};

			var playlistFinishedEvent = new PlaylistFinishedEventArgs(songs);

			var mocker = new AutoMocker();
			mocker.Use(Enumerable.Empty<IApplicationInitializer>());
			mocker.StubMessenger();
			var target = mocker.CreateInstance<ApplicationViewModel>();

			// Act

			mocker.SendMessage(playlistFinishedEvent);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowRatePlaylistSongsView(It.IsAny<IEnumerable<SongModel>>()), Times.Never);
		}
	}
}
