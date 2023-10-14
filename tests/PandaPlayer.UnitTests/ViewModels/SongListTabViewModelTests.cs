using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class SongListTabViewModelTests
	{
		[TestMethod]
		public void Constructor_SetsDiscSongListAsDefaultView()
		{
			// Arrange

			var mocker = new AutoMocker();

			// Act

			var target = mocker.CreateInstance<SongListTabViewModel>();

			// Assert

			target.IsDiscSongListSelected.Should().BeTrue();
			target.IsPlaylistSelected.Should().BeFalse();
		}

		[TestMethod]
		public void SwitchToDiscSongListCommand_SwitchesToDiscSongList()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToPlaylistCommand.Execute(null);

			// Act

			target.SwitchToDiscSongListCommand.Execute(null);

			// Assert

			target.IsDiscSongListSelected.Should().BeTrue();
			target.IsPlaylistSelected.Should().BeFalse();
		}

		[TestMethod]
		public void SwitchToDiscSongListCommand_IfSongListIsChanged_RaisesActiveDiscChangedEventForSelectedDisc()
		{
			// Arrange

			var selectedDisc = new DiscModel();

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerViewModel>()
				.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToPlaylistCommand.Execute(null);

			ActiveDiscChangedEventArgs activeDiscChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<ActiveDiscChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref activeDiscChangedEventArgs));

			// Act

			target.SwitchToDiscSongListCommand.Execute(null);

			// Assert

			activeDiscChangedEventArgs.Should().NotBeNull();
			activeDiscChangedEventArgs.Disc.Should().Be(selectedDisc);
		}

		[TestMethod]
		public void SwitchToPlaylistCommand_SwitchesToPlaylist()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToDiscSongListCommand.Execute(null);

			// Act

			target.SwitchToPlaylistCommand.Execute(null);

			// Assert

			target.IsPlaylistSelected.Should().BeTrue();
			target.IsDiscSongListSelected.Should().BeFalse();
		}

		[TestMethod]
		public void SwitchToPlaylistCommand_IfSongListIsChanged_RaisesPropertyChangedForAllAffectedProperties()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToDiscSongListCommand.Execute(null);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SwitchToPlaylistCommand.Execute(null);

			// Assert

			var expectedProperties = new[]
			{
				"CurrentSongListViewModel",
				nameof(SongListTabViewModel.IsDiscSongListSelected),
				nameof(SongListTabViewModel.IsPlaylistSelected),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void SwitchToPlaylistCommand_IfSongListIsChanged_RaisesActiveDiscChangedEventForCurrentPlaylistDisc()
		{
			// Arrange

			var currentDisc = new DiscModel();

			var mocker = new AutoMocker();
			mocker.GetMock<IPlaylistViewModel>()
				.Setup(x => x.CurrentDisc).Returns(currentDisc);

			mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToDiscSongListCommand.Execute(null);

			ActiveDiscChangedEventArgs activeDiscChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<ActiveDiscChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref activeDiscChangedEventArgs));

			// Act

			target.SwitchToPlaylistCommand.Execute(null);

			// Assert

			activeDiscChangedEventArgs.Should().NotBeNull();
			activeDiscChangedEventArgs.Disc.Should().Be(currentDisc);
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfPlaylistHasSomeSongs_SwitchesToPlaylist()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel(),
				new SongModel(),
			};

			var mocker = new AutoMocker();
			mocker.GetMock<IPlaylistViewModel>().Setup(x => x.Songs).Returns(songs);
			mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();
			target.IsPlaylistSelected.Should().BeFalse();

			// Act

			mocker.SendMessage(new ApplicationLoadedEventArgs());

			// Assert

			target.IsPlaylistSelected.Should().BeTrue();
			target.IsDiscSongListSelected.Should().BeFalse();
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfPlaylistHasNoSongs_LeavesDiscSongListSelected()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<IPlaylistViewModel>().Setup(x => x.Songs).Returns(Enumerable.Empty<SongModel>());
			mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();

			// Act

			mocker.SendMessage(new ApplicationLoadedEventArgs());

			// Assert

			target.IsDiscSongListSelected.Should().BeTrue();
			target.IsPlaylistSelected.Should().BeFalse();
		}

		[TestMethod]
		public void LibraryExplorerDiscChangedEventHandler_SwitchesToDiscSongList()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToPlaylistCommand.Execute(null);

			// Act

			mocker.SendMessage(new LibraryExplorerDiscChangedEventArgs(new DiscModel(), deletedContentIsShown: false));

			// Assert

			target.IsDiscSongListSelected.Should().BeTrue();
			target.IsPlaylistSelected.Should().BeFalse();
		}

		[TestMethod]
		public void PlaySongsListEventHandler_SwitchesToPlaylist()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToDiscSongListCommand.Execute(null);

			// Act

			mocker.SendMessage(new PlaySongsListEventArgs(Enumerable.Empty<SongModel>()));

			// Assert

			target.IsPlaylistSelected.Should().BeTrue();
			target.IsDiscSongListSelected.Should().BeFalse();
		}

		[TestMethod]
		public void PlaylistLoadedEventHandler_IfPlaylistIsSelected_RaisesActiveDiscChangedEventForCurrentPlaylistDisc()
		{
			// Arrange

			var currentDisc = new DiscModel();

			var mocker = new AutoMocker();
			var messenger = mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToPlaylistCommand.Execute(null);

			// We mock CurrentDisc property after switching to Playlist.
			// Otherwise value of ActiveDisc will be the same and no event will be sent.
			mocker.GetMock<IPlaylistViewModel>()
				.Setup(x => x.CurrentDisc).Returns(currentDisc);

			ActiveDiscChangedEventArgs activeDiscChangedEventArgs = null;
			messenger.Register<ActiveDiscChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref activeDiscChangedEventArgs));

			// Act

			messenger.Send(new PlaylistLoadedEventArgs(Enumerable.Empty<SongModel>(), null, null));

			// Assert

			activeDiscChangedEventArgs.Should().NotBeNull();
			activeDiscChangedEventArgs.Disc.Should().Be(currentDisc);
		}

		[TestMethod]
		public void PlaylistLoadedEventHandler_IfDiscSongListIsSelected_DoesNotRaiseActiveDiscChangedEvent()
		{
			// Arrange

			var mocker = new AutoMocker();

			// Mocking SelectedDisc for LibraryExplorer so that we could catch mistaken set of ActiveDisc to different value.
			mocker.GetMock<ILibraryExplorerViewModel>().Setup(x => x.SelectedDisc).Returns(new DiscModel());

			var messenger = mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToDiscSongListCommand.Execute(null);

			ActiveDiscChangedEventArgs activeDiscChangedEventArgs = null;
			messenger.Register<ActiveDiscChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref activeDiscChangedEventArgs));

			// Act

			messenger.Send(new PlaylistLoadedEventArgs(Enumerable.Empty<SongModel>(), null, null));

			// Assert

			activeDiscChangedEventArgs.Should().BeNull();
		}

		[TestMethod]
		public void PlaylistChangedEventHandler_IfPlaylistIsSelected_RaisesActiveDiscChangedEventForCurrentPlaylistDisc()
		{
			// Arrange

			var currentDisc = new DiscModel();

			var mocker = new AutoMocker();
			var messenger = mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToPlaylistCommand.Execute(null);

			// We mock CurrentDisc property after switching to Playlist.
			// Otherwise value of ActiveDisc will be the same and no event will be sent.
			mocker.GetMock<IPlaylistViewModel>().Setup(x => x.CurrentDisc).Returns(currentDisc);

			ActiveDiscChangedEventArgs activeDiscChangedEventArgs = null;
			messenger.Register<ActiveDiscChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref activeDiscChangedEventArgs));

			// Act

			messenger.Send(new PlaylistChangedEventArgs(Enumerable.Empty<SongModel>(), null, null));

			// Assert

			activeDiscChangedEventArgs.Should().NotBeNull();
			activeDiscChangedEventArgs.Disc.Should().Be(currentDisc);
		}

		[TestMethod]
		public void PlaylistChangedEventHandler_IfDiscSongListIsSelected_DoesNotRaiseActiveDiscChangedEvent()
		{
			// Arrange

			var mocker = new AutoMocker();

			// Mocking SelectedDisc for LibraryExplorer so that we could catch mistaken set of ActiveDisc to different value.
			mocker.GetMock<ILibraryExplorerViewModel>().Setup(x => x.SelectedDisc).Returns(new DiscModel());

			var messenger = mocker.StubMessenger();

			var target = mocker.CreateInstance<SongListTabViewModel>();

			target.SwitchToDiscSongListCommand.Execute(null);

			ActiveDiscChangedEventArgs activeDiscChangedEventArgs = null;
			messenger.Register<ActiveDiscChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref activeDiscChangedEventArgs));

			// Act

			messenger.Send(new PlaylistChangedEventArgs(Enumerable.Empty<SongModel>(), null, null));

			// Assert

			activeDiscChangedEventArgs.Should().BeNull();
		}
	}
}
