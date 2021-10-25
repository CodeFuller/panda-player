using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class SongListViewModelTests
	{
		private class TestSongListViewModel : SongListViewModel
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

		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
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

		[TestMethod]
		public void SongChangedEventHandler_SongDoesNotPresentsInList_DoesNothing()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Title = "Old Title 0",
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Title = "Old Title 1",
				},
			};

			var changedSong = new SongModel
			{
				Id = new ItemId("2"),
				Title = "New Title 2",
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new SongChangedEventArgs(changedSong, nameof(SongModel.Title)));

			// Assert

			var songList = target.Songs.ToList();
			songList[0].Title.Should().Be("Old Title 0");
			songList[1].Title.Should().Be("Old Title 1");

			songList.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public void SongChangedEventHandler_SongPresentsInList_UpdatesAllInstancesOfSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Title = "Old Title 0",
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Title = "Old Title 1",
				},
				new SongModel
				{
					Id = new ItemId("0"),
					Title = "Old Title 0",
				},
			};

			var changedSong = new SongModel
			{
				Id = new ItemId("0"),
				Title = "New Title 0",
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new SongChangedEventArgs(changedSong, nameof(SongModel.Title)));

			// Assert

			var songList = target.Songs.ToList();
			songList[0].Title.Should().Be("New Title 0");
			songList[1].Title.Should().Be("Old Title 1");
			songList[2].Title.Should().Be("New Title 0");

			songList.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public void SongChangedEventHandler_ChangedPropertyIsDeleteDateAndSongIsDeleted_DeletesAllOccurrencesOfSongFromList()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("1") },
			};

			var changedSong = new SongModel
			{
				Id = new ItemId("1"),
				DeleteDate = new DateTime(2021, 07, 11),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new SongChangedEventArgs(changedSong, nameof(SongModel.DeleteDate)));

			// Assert

			var expectedSongs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("2") },
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public void SongChangedEventHandler_ChangedPropertyIsDeleteDateButSongIsNotDeleted_DoesNotDeleteSongsFromList()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
			};

			var changedSong = new SongModel
			{
				Id = new ItemId("0"),
				DeleteDate = null,
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new SongChangedEventArgs(changedSong, nameof(SongModel.DeleteDate)));

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public void SongChangedEventHandler_ChangedPropertyIsNotDeleteDate_DoesNotDeleteSongsFromList()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
			};

			var changedSong = new SongModel
			{
				Id = new ItemId("0"),
				DeleteDate = new DateTime(2021, 07, 11),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new SongChangedEventArgs(changedSong, nameof(SongModel.Title)));

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public void DiscChangedEventHandler_UpdatesAllOccurrencesOfSameDisc()
		{
			// Arrange

			var disc1 = new DiscModel
			{
				Id = new ItemId("0"),
				Title = "Old Title 0",
			};

			var disc2 = new DiscModel
			{
				Id = new ItemId("1"),
				Title = "Old Title 1",
			};

			var disc3 = new DiscModel
			{
				Id = new ItemId("0"),
				Title = "Old Title 0",
			};

			var changedDisc = new DiscModel
			{
				Id = new ItemId("0"),
				Title = "New Title 0",
			};

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("2"),
					Disc = disc2,
				},
				new SongModel
				{
					Id = new ItemId("3"),
					Disc = disc3,
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			// Act

			Messenger.Default.Send(new DiscChangedEventArgs(changedDisc, nameof(DiscModel.Title)));

			// Assert

			disc1.Title.Should().Be("New Title 0");
			disc2.Title.Should().Be("Old Title 1");
			disc3.Title.Should().Be("New Title 0");
		}

		[TestMethod]
		public void DiscImageChangedEventHandler_UpdatesImagesForAllOccurrencesOfSameDisc()
		{
			// Arrange

			var images1 = new[] { new DiscImageModel { Id = new ItemId("1") } };
			var images2 = new[] { new DiscImageModel { Id = new ItemId("2") } };
			var images3 = new[] { new DiscImageModel { Id = new ItemId("3") } };
			var newImages = new[] { new DiscImageModel { Id = new ItemId("4") } };

			var disc1 = new DiscModel
			{
				Id = new ItemId("1"),
				Images = images1,
			};

			var disc2 = new DiscModel
			{
				Id = new ItemId("2"),
				Images = images2,
			};

			var disc3 = new DiscModel
			{
				// Same id as disc1.
				Id = new ItemId("1"),
				Images = images3,
			};

			var changedDisc = new DiscModel
			{
				Id = new ItemId("1"),
				Images = newImages,
			};

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("2"),
					Disc = disc2,
				},
				new SongModel
				{
					Id = new ItemId("3"),
					Disc = disc3,
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TestSongListViewModel>();

			target.SetSongs(songs);

			// Act

			Messenger.Default.Send(new DiscImageChangedEventArgs(changedDisc));

			// Assert

			disc1.Images.Should().BeEquivalentTo(newImages, x => x.WithStrictOrdering());
			disc2.Images.Should().BeEquivalentTo(images2, x => x.WithStrictOrdering());
			disc3.Images.Should().BeEquivalentTo(newImages, x => x.WithStrictOrdering());
		}
	}
}
