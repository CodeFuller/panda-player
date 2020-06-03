using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.ViewModels;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class EditSongPropertiesViewModelTests
	{
		[TestMethod]
		public async Task Load_ForSingleSong_LoadsScalarPropertiesCorrectly()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel
			{
				Title = "Some Title",
				TreeTitle = "Some Tree Title",
				TrackNumber = 7,
			};

			// Act

			await target.Load(new[] { song }, CancellationToken.None);

			// Assert

			Assert.AreEqual("Some Title", target.Title);
			Assert.AreEqual("Some Tree Title", target.TreeTitle);
			Assert.AreEqual((short)7, target.TrackNumber);
		}

		[TestMethod]
		public async Task Load_ForMultipleSongs_LoadsScalarPropertiesCorrectly()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song1 = new SongModel
			{
				Title = "Some Title 1",
				TreeTitle = "Some Tree Title 1",
				TrackNumber = 1,
			};

			var song2 = new SongModel
			{
				Title = "Some Title 2",
				TreeTitle = "Some Tree Title 2",
				TrackNumber = 2,
			};

			// Act

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Assert

			Assert.IsNull(target.Title);
			Assert.IsNull(target.TreeTitle);
			Assert.IsNull(target.TrackNumber);
		}

		[TestMethod]
		public async Task Load_ForSingleSongWithSomeArtist_FillsArtistDataCorrectly()
		{
			// Arrange

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Another Artist" };

			var mocker = CreateMocker(artists: new[] { artist1, artist2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel { Artist = artist2 };

			// Act

			await target.Load(new[] { song }, CancellationToken.None);

			// Assert

			var availableArtistsList = target.AvailableArtists.ToList();
			Assert.AreEqual(3, availableArtistsList.Count);

			// '<blank>' item
			Assert.IsTrue(availableArtistsList[0].HasBlankValue);

			// 'Some Artist' item
			Assert.IsTrue(availableArtistsList[1].HasValue);
			Assert.AreSame(artist1, availableArtistsList[1].Value);

			// 'Another Artist' item
			Assert.IsTrue(availableArtistsList[2].HasValue);
			Assert.AreSame(artist2, availableArtistsList[2].Value);

			// 'Some Artist' item must be selected
			Assert.AreSame(availableArtistsList[2], target.Artist);
		}

		[TestMethod]
		public async Task Load_ForSingleSongWithoutArtist_FillsArtistDataCorrectly()
		{
			// Arrange

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Another Artist" };

			var mocker = CreateMocker(artists: new[] { artist1, artist2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel { Artist = null };

			// Act

			await target.Load(new[] { song }, CancellationToken.None);

			// Assert

			var availableArtistsList = target.AvailableArtists.ToList();
			Assert.AreEqual(3, availableArtistsList.Count);

			// '<blank>' item
			Assert.IsTrue(availableArtistsList[0].HasBlankValue);

			// 'Some Artist' item
			Assert.IsTrue(availableArtistsList[1].HasValue);
			Assert.AreSame(artist1, availableArtistsList[1].Value);

			// 'Another Artist' item
			Assert.IsTrue(availableArtistsList[2].HasValue);
			Assert.AreSame(artist2, availableArtistsList[2].Value);

			// '<blank>' item must be selected
			Assert.AreSame(availableArtistsList[0], target.Artist);
		}

		[TestMethod]
		public async Task Load_ForMultipleSongsWithSameArtist_FillsArtistDataCorrectly()
		{
			// Arrange

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Another Artist" };

			var song1 = new SongModel { Artist = artist2 };
			var song2 = new SongModel { Artist = artist2 };

			var mocker = CreateMocker(artists: new[] { artist1, artist2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Assert

			var availableArtistsList = target.AvailableArtists.ToList();
			Assert.AreEqual(3, availableArtistsList.Count);

			// '<blank>' item
			Assert.IsTrue(availableArtistsList[0].HasBlankValue);

			// 'Some Artist' item
			Assert.IsTrue(availableArtistsList[1].HasValue);
			Assert.AreSame(artist1, availableArtistsList[1].Value);

			// 'Another Artist' item
			Assert.IsTrue(availableArtistsList[2].HasValue);
			Assert.AreSame(artist2, availableArtistsList[2].Value);

			// 'Some Artist' item must be selected
			Assert.AreSame(availableArtistsList[2], target.Artist);
		}

		[TestMethod]
		public async Task Load_ForMultipleSongsWithoutArtist_FillsArtistDataCorrectly()
		{
			// Arrange

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Another Artist" };

			var song1 = new SongModel { Artist = null };
			var song2 = new SongModel { Artist = null };

			var mocker = CreateMocker(artists: new[] { artist1, artist2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Assert

			var availableArtistsList = target.AvailableArtists.ToList();
			Assert.AreEqual(3, availableArtistsList.Count);

			// '<blank>' item
			Assert.IsTrue(availableArtistsList[0].HasBlankValue);

			// 'Some Artist' item
			Assert.IsTrue(availableArtistsList[1].HasValue);
			Assert.AreSame(artist1, availableArtistsList[1].Value);

			// 'Another Artist' item
			Assert.IsTrue(availableArtistsList[2].HasValue);
			Assert.AreSame(artist2, availableArtistsList[2].Value);

			// '<blank>' item must be selected
			Assert.AreSame(availableArtistsList[0], target.Artist);
		}

		[TestMethod]
		public async Task Load_ForMultipleSongsWithDifferentArtists_FillsArtistDataCorrectly()
		{
			// Arrange

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Another Artist" };

			var song1 = new SongModel { Artist = artist2 };
			var song2 = new SongModel { Artist = artist1 };

			var mocker = CreateMocker(artists: new[] { artist1, artist2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Assert

			var availableArtistsList = target.AvailableArtists.ToList();
			Assert.AreEqual(4, availableArtistsList.Count);

			// '<keep>' item
			Assert.IsFalse(availableArtistsList[0].HasValue);

			// '<blank>' item
			Assert.IsTrue(availableArtistsList[1].HasBlankValue);

			// 'Some Artist' item
			Assert.IsTrue(availableArtistsList[2].HasValue);
			Assert.AreSame(artist1, availableArtistsList[2].Value);

			// 'Another Artist' item
			Assert.IsTrue(availableArtistsList[3].HasValue);
			Assert.AreSame(artist2, availableArtistsList[3].Value);

			// '<keep>' item must be selected
			Assert.AreSame(availableArtistsList[0], target.Artist);
		}

		[TestMethod]
		public async Task Load_ForSingleSongWithSomeGenre_FillsGenreDataCorrectly()
		{
			// Arrange

			var genre1 = new GenreModel { Id = new ItemId("1"), Name = "Some Genre" };
			var genre2 = new GenreModel { Id = new ItemId("2"), Name = "Another Genre" };

			var mocker = CreateMocker(genres: new[] { genre1, genre2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel { Genre = genre2 };

			// Act

			await target.Load(new[] { song }, CancellationToken.None);

			// Assert

			var availableGenresList = target.AvailableGenres.ToList();
			Assert.AreEqual(3, availableGenresList.Count);

			// '<blank>' item
			Assert.IsTrue(availableGenresList[0].HasBlankValue);

			// 'Some Genre' item
			Assert.IsTrue(availableGenresList[1].HasValue);
			Assert.AreSame(genre1, availableGenresList[1].Value);

			// 'Another Genre' item
			Assert.IsTrue(availableGenresList[2].HasValue);
			Assert.AreSame(genre2, availableGenresList[2].Value);

			// 'Some Genre' item must be selected
			Assert.AreSame(availableGenresList[2], target.Genre);
		}

		[TestMethod]
		public async Task Load_ForSingleSongWithoutGenre_FillsGenreDataCorrectly()
		{
			// Arrange

			var genre1 = new GenreModel { Id = new ItemId("1"), Name = "Some Genre" };
			var genre2 = new GenreModel { Id = new ItemId("2"), Name = "Another Genre" };

			var mocker = CreateMocker(genres: new[] { genre1, genre2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel { Genre = null };

			// Act

			await target.Load(new[] { song }, CancellationToken.None);

			// Assert

			var availableGenresList = target.AvailableGenres.ToList();
			Assert.AreEqual(3, availableGenresList.Count);

			// '<blank>' item
			Assert.IsTrue(availableGenresList[0].HasBlankValue);

			// 'Some Genre' item
			Assert.IsTrue(availableGenresList[1].HasValue);
			Assert.AreSame(genre1, availableGenresList[1].Value);

			// 'Another Genre' item
			Assert.IsTrue(availableGenresList[2].HasValue);
			Assert.AreSame(genre2, availableGenresList[2].Value);

			// '<blank>' item must be selected
			Assert.AreSame(availableGenresList[0], target.Genre);
		}

		[TestMethod]
		public async Task Load_ForMultipleSongsWithSameGenre_FillsGenreDataCorrectly()
		{
			// Arrange

			var genre1 = new GenreModel { Id = new ItemId("1"), Name = "Some Genre" };
			var genre2 = new GenreModel { Id = new ItemId("2"), Name = "Another Genre" };

			var song1 = new SongModel { Genre = genre2 };
			var song2 = new SongModel { Genre = genre2 };

			var mocker = CreateMocker(genres: new[] { genre1, genre2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Assert

			var availableGenresList = target.AvailableGenres.ToList();
			Assert.AreEqual(3, availableGenresList.Count);

			// '<blank>' item
			Assert.IsTrue(availableGenresList[0].HasBlankValue);

			// 'Some Genre' item
			Assert.IsTrue(availableGenresList[1].HasValue);
			Assert.AreSame(genre1, availableGenresList[1].Value);

			// 'Another Genre' item
			Assert.IsTrue(availableGenresList[2].HasValue);
			Assert.AreSame(genre2, availableGenresList[2].Value);

			// 'Some Genre' item must be selected
			Assert.AreSame(availableGenresList[2], target.Genre);
		}

		[TestMethod]
		public async Task Load_ForMultipleSongsWithoutGenre_FillsGenreDataCorrectly()
		{
			// Arrange

			var genre1 = new GenreModel { Id = new ItemId("1"), Name = "Some Genre" };
			var genre2 = new GenreModel { Id = new ItemId("2"), Name = "Another Genre" };

			var song1 = new SongModel { Genre = null };
			var song2 = new SongModel { Genre = null };

			var mocker = CreateMocker(genres: new[] { genre1, genre2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Assert

			var availableGenresList = target.AvailableGenres.ToList();
			Assert.AreEqual(3, availableGenresList.Count);

			// '<blank>' item
			Assert.IsTrue(availableGenresList[0].HasBlankValue);

			// 'Some Genre' item
			Assert.IsTrue(availableGenresList[1].HasValue);
			Assert.AreSame(genre1, availableGenresList[1].Value);

			// 'Another Genre' item
			Assert.IsTrue(availableGenresList[2].HasValue);
			Assert.AreSame(genre2, availableGenresList[2].Value);

			// '<blank>' item must be selected
			Assert.AreSame(availableGenresList[0], target.Genre);
		}

		[TestMethod]
		public async Task Load_ForMultipleSongsWithDifferentGenres_FillsGenreDataCorrectly()
		{
			// Arrange

			var genre1 = new GenreModel { Id = new ItemId("1"), Name = "Some Genre" };
			var genre2 = new GenreModel { Id = new ItemId("2"), Name = "Another Genre" };

			var song1 = new SongModel { Genre = genre2 };
			var song2 = new SongModel { Genre = genre1 };

			var mocker = CreateMocker(genres: new[] { genre1, genre2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Assert

			var availableGenresList = target.AvailableGenres.ToList();
			Assert.AreEqual(4, availableGenresList.Count);

			// '<keep>' item
			Assert.IsFalse(availableGenresList[0].HasValue);

			// '<blank>' item
			Assert.IsTrue(availableGenresList[1].HasBlankValue);

			// 'Some Genre' item
			Assert.IsTrue(availableGenresList[2].HasValue);
			Assert.AreSame(genre1, availableGenresList[2].Value);

			// 'Another Genre' item
			Assert.IsTrue(availableGenresList[3].HasValue);
			Assert.AreSame(genre2, availableGenresList[3].Value);

			// '<keep>' item must be selected
			Assert.AreSame(availableGenresList[0], target.Genre);
		}

		[TestMethod]
		public async Task Load_ForEmptySongList_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			Task Call() => target.Load(Enumerable.Empty<SongModel>(), CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<InvalidOperationException>(Call);
		}

		[TestMethod]
		public async Task TreeTitleSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), new SongModel(), }, CancellationToken.None);

			// Act

			void Call() => target.TreeTitle = "New Tree Title";

			// Assert

			Assert.ThrowsException<InvalidOperationException>(Call);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataRow(" ")]
		[DataTestMethod]
		public async Task TreeTitleSetter_ForMissingValue_ThrowsInvalidOperationException(string newTreeTitle)
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), }, CancellationToken.None);

			// Act

			void Call() => target.TreeTitle = newTreeTitle;

			// Assert

			Assert.ThrowsException<InvalidOperationException>(Call);
		}

		[TestMethod]
		public async Task TitleSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), new SongModel(), }, CancellationToken.None);

			// Act

			void Call() => target.Title = "New Title";

			// Assert

			Assert.ThrowsException<InvalidOperationException>(Call);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataRow(" ")]
		[DataTestMethod]
		public async Task TitleSetter_ForMissingValue_ThrowsInvalidOperationException(string newTitle)
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), }, CancellationToken.None);

			// Act

			void Call() => target.Title = newTitle;

			// Assert

			Assert.ThrowsException<InvalidOperationException>(Call);
		}

		[TestMethod]
		public async Task TrackNumberSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), new SongModel(), }, CancellationToken.None);

			// Act

			void Call() => target.TrackNumber = 1;

			// Assert

			Assert.ThrowsException<InvalidOperationException>(Call);
		}

		[TestMethod]
		public async Task Save_ForSingleSongWhenPropertiesAreUpdated_UpdatesSongCorrectly()
		{
			// Arrange

			var oldArtist = new ArtistModel { Id = new ItemId("1"), Name = "Old Artist" };
			var newArtist = new ArtistModel { Id = new ItemId("2"), Name = "New Artist" };

			var oldGenre = new GenreModel { Id = new ItemId("1"), Name = "Old Genre" };
			var newGenre = new GenreModel { Id = new ItemId("2"), Name = "New Genre" };

			var mocker = CreateMocker(artists: new[] { oldArtist, newArtist }, genres: new[] { oldGenre, newGenre, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel
			{
				Title = "Old Title",
				TreeTitle = "Old Tree Title",
				Artist = oldArtist,
				Genre = oldGenre,
				TrackNumber = 1,
			};

			await target.Load(new[] { song }, CancellationToken.None);

			// Act

			target.Title = "New Title";
			target.TreeTitle = "New Tree Title";
			target.Artist = target.AvailableArtists.Last();
			target.Genre = target.AvailableGenres.Last();
			target.TrackNumber = 2;

			await target.Save(CancellationToken.None);

			// Assert

			var songServiceMock = mocker.GetMock<ISongsService>();

			Func<SongModel, bool> checkSong = x => x.Title == "New Title" && x.TreeTitle == "New Tree Title" && x.TrackNumber == 2 &&
			                                       Object.ReferenceEquals(x.Artist, newArtist) && Object.ReferenceEquals(x.Genre, newGenre);
			songServiceMock.Verify(x => x.UpdateSong(It.Is<SongModel>(y => checkSong(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForSingleSongWhenPropertiesAreCleared_UpdatesSongCorrectly()
		{
			// Arrange

			var oldArtist = new ArtistModel { Id = new ItemId("1"), Name = "Old Artist" };
			var newArtist = new ArtistModel { Id = new ItemId("2"), Name = "New Artist" };

			var oldGenre = new GenreModel { Id = new ItemId("1"), Name = "Old Genre" };
			var newGenre = new GenreModel { Id = new ItemId("2"), Name = "New Genre" };

			var mocker = CreateMocker(artists: new[] { oldArtist, newArtist }, genres: new[] { oldGenre, newGenre, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel
			{
				Title = "Some Title",
				TreeTitle = "Some Tree Title",
				Artist = oldArtist,
				Genre = oldGenre,
				TrackNumber = 1,
			};

			await target.Load(new[] { song }, CancellationToken.None);

			// Act

			target.Artist = target.AvailableArtists.Single(x => x.HasBlankValue);
			target.Genre = target.AvailableGenres.Single(x => x.HasBlankValue);
			target.TrackNumber = null;

			await target.Save(CancellationToken.None);

			// Assert

			var songServiceMock = mocker.GetMock<ISongsService>();

			Func<SongModel, bool> checkSong = x => x.Title == "Some Title" && x.TreeTitle == "Some Tree Title" && x.TrackNumber == null &&
			                                       x.Artist == null && x.Genre == null;
			songServiceMock.Verify(x => x.UpdateSong(It.Is<SongModel>(y => checkSong(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForMultipleSongsWhenAllPropertiesAreUpdated_UpdatesSongCorrectly()
		{
			// Arrange

			var oldArtist = new ArtistModel { Id = new ItemId("1"), Name = "Old Artist" };
			var newArtist = new ArtistModel { Id = new ItemId("2"), Name = "New Artist" };

			var oldGenre = new GenreModel { Id = new ItemId("1"), Name = "Old Genre" };
			var newGenre = new GenreModel { Id = new ItemId("2"), Name = "New Genre" };

			var mocker = CreateMocker(artists: new[] { oldArtist, newArtist }, genres: new[] { oldGenre, newGenre, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song1 = new SongModel
			{
				Title = "Old Title 1",
				TreeTitle = "Old Tree Title 1",
				Artist = oldArtist,
				Genre = oldGenre,
				TrackNumber = 1,
			};

			var song2 = new SongModel
			{
				Title = "Old Title 2",
				TreeTitle = "Old Tree Title 2",
				Artist = oldArtist,
				Genre = oldGenre,
				TrackNumber = 2,
			};

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Act

			target.Artist = target.AvailableArtists.Last();
			target.Genre = target.AvailableGenres.Last();

			await target.Save(CancellationToken.None);

			// Assert

			var songServiceMock = mocker.GetMock<ISongsService>();

			Func<SongModel, bool> checkSong1 = x => x.Title == "Old Title 1" && x.TreeTitle == "Old Tree Title 1" && x.TrackNumber == 1 &&
			                                       Object.ReferenceEquals(x.Artist, newArtist) && Object.ReferenceEquals(x.Genre, newGenre);
			songServiceMock.Verify(x => x.UpdateSong(It.Is<SongModel>(y => checkSong1(y)), It.IsAny<CancellationToken>()), Times.Once);

			Func<SongModel, bool> checkSong2 = x => x.Title == "Old Title 2" && x.TreeTitle == "Old Tree Title 2" && x.TrackNumber == 2 &&
			                                        Object.ReferenceEquals(x.Artist, newArtist) && Object.ReferenceEquals(x.Genre, newGenre);
			songServiceMock.Verify(x => x.UpdateSong(It.Is<SongModel>(y => checkSong2(y)), It.IsAny<CancellationToken>()), Times.Once);

			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
		}

		[TestMethod]
		public async Task Save_ForMultipleSongsWhenPropertiesAreKept_UpdatesSongCorrectly()
		{
			// Arrange

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Artist 1" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Artist 2" };

			var genre1 = new GenreModel { Id = new ItemId("1"), Name = "Genre 1" };
			var genre2 = new GenreModel { Id = new ItemId("2"), Name = "Genre 2" };

			var mocker = CreateMocker(artists: new[] { artist1, artist2 }, genres: new[] { genre1, genre2, });
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song1 = new SongModel
			{
				Title = "Old Title 1",
				TreeTitle = "Old Tree Title 1",
				Artist = artist1,
				Genre = genre1,
				TrackNumber = 1,
			};

			var song2 = new SongModel
			{
				Title = "Old Title 2",
				TreeTitle = "Old Tree Title 2",
				Artist = artist2,
				Genre = genre2,
				TrackNumber = 2,
			};

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Act

			await target.Save(CancellationToken.None);

			// Assert

			var songServiceMock = mocker.GetMock<ISongsService>();

			Func<SongModel, bool> checkSong1 = x => x.Title == "Old Title 1" && x.TreeTitle == "Old Tree Title 1" && x.TrackNumber == 1 &&
												   Object.ReferenceEquals(x.Artist, artist1) && Object.ReferenceEquals(x.Genre, genre1);
			songServiceMock.Verify(x => x.UpdateSong(It.Is<SongModel>(y => checkSong1(y)), It.IsAny<CancellationToken>()), Times.Once);

			Func<SongModel, bool> checkSong2 = x => x.Title == "Old Title 2" && x.TreeTitle == "Old Tree Title 2" && x.TrackNumber == 2 &&
													Object.ReferenceEquals(x.Artist, artist2) && Object.ReferenceEquals(x.Genre, genre2);
			songServiceMock.Verify(x => x.UpdateSong(It.Is<SongModel>(y => checkSong2(y)), It.IsAny<CancellationToken>()), Times.Once);

			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
		}

		private static AutoMocker CreateMocker(IReadOnlyCollection<ArtistModel> artists = null, IReadOnlyCollection<GenreModel> genres = null)
		{
			var mocker = new AutoMocker();

			var artistsStub = mocker.GetMock<IArtistsService>();
			artistsStub.Setup(x => x.GetAllArtists(It.IsAny<CancellationToken>())).ReturnsAsync(artists ?? new List<ArtistModel>());

			var genresStub = mocker.GetMock<IGenresService>();
			genresStub.Setup(x => x.GetAllGenres(It.IsAny<CancellationToken>())).ReturnsAsync(genres ?? new List<GenreModel>());

			return mocker;
		}
	}
}
