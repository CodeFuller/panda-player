using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class SongListViewModelTests
	{
		private sealed class TestSongListViewModel : SongListViewModel
		{
			public override bool DisplayTrackNumbers => false;

			public override IEnumerable<BasicMenuItem> ContextMenuItems => Enumerable.Empty<BasicMenuItem>();

			public TestSongListViewModel(ISongsService songsService, IViewNavigator viewNavigator)
				: base(songsService, viewNavigator)
			{
			}

			public Task InvokeEditSongsProperties(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
			{
				return EditSongsProperties(songs, cancellationToken);
			}
		}

		[TestMethod]
		public void ListInfoProperties_ForEmptySongList_ReturnCorrectValues()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			// Act

			var hasSongs = target.HasSongs;
			var songsNumber = target.SongsNumber;
			var totalSongsFileSize = target.TotalSongsFileSize;
			var totalSongsDuration = target.TotalSongsDuration;

			// Assert

			hasSongs.Should().BeFalse();
			songsNumber.Should().Be(0);
			totalSongsFileSize.Should().Be("N/A");
			totalSongsDuration.Should().Be(TimeSpan.Zero);
		}

		[TestMethod]
		public void ListInfoProperties_ForNonEmptySongList_ReturnCorrectValues()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Size = 123,
					Duration = new TimeSpan(0, 3, 28),
				},

				new SongModel
				{
					Id = new ItemId("1"),
					Size = 12345,
					Duration = new TimeSpan(0, 4, 11),
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			// Act

			var hasSongs = target.HasSongs;
			var songsNumber = target.SongsNumber;
			var totalSongsFileSize = target.TotalSongsFileSize;
			var totalSongsDuration = target.TotalSongsDuration;

			// Assert

			hasSongs.Should().BeTrue();
			songsNumber.Should().Be(2);
			totalSongsFileSize.Should().Be("12.2 KB");
			totalSongsDuration.Should().Be(new TimeSpan(0, 7, 39));
		}

		[TestMethod]
		public void ListInfoProperties_WhenAllSongsAreDeleted_ReturnCorrectValues()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Size = null,
					Duration = new TimeSpan(0, 3, 28),
					DeleteDate = new DateTime(2021, 10, 25),
				},

				new SongModel
				{
					Id = new ItemId("1"),
					Size = null,
					Duration = new TimeSpan(0, 4, 11),
					DeleteDate = new DateTime(2021, 10, 25),
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			// Act

			var hasSongs = target.HasSongs;
			var songsNumber = target.SongsNumber;
			var totalSongsFileSize = target.TotalSongsFileSize;
			var totalSongsDuration = target.TotalSongsDuration;

			// Assert

			hasSongs.Should().BeTrue();
			songsNumber.Should().Be(2);
			totalSongsFileSize.Should().Be("N/A");
			totalSongsDuration.Should().Be(new TimeSpan(0, 7, 39));
		}

		[TestMethod]
		public void SetSongs_ForEmptySongList_FillsListWithSongsCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SetSongs(songs);

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());

			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public void SetSongs_ForNonEmptySongList_DeletesPreviousSongs()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var newSongs = new[]
			{
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(oldSongs);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SetSongs(newSongs);

			// Assert

			target.Songs.Should().BeEquivalentTo(newSongs, x => x.WithStrictOrdering());

			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task EditSongsProperties_ShowsSongPropertiesView()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			// Act

			await target.InvokeEditSongsProperties(songs, CancellationToken.None);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowSongPropertiesView(songs, It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
