using System;
using System.Collections.Generic;
using System.Linq;
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
	public class EditSongPropertiesViewModelTests
	{
		[TestMethod]
		public async Task Load_ForSingleActiveSong_LoadsScalarPropertiesCorrectly()
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

			target.Title.Should().Be("Some Title");
			target.TreeTitle.Should().Be("Some Tree Title");
			target.TrackNumber.Should().Be(7);
			target.SongsAreDeleted.Should().BeFalse();
			target.DeleteComment.Should().BeNull();
		}

		[TestMethod]
		public async Task Load_ForMultipleActiveSongs_LoadsScalarPropertiesCorrectly()
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

			target.Title.Should().BeNull();
			target.TreeTitle.Should().BeNull();
			target.TrackNumber.Should().BeNull();
			target.SongsAreDeleted.Should().BeFalse();
			target.DeleteComment.Should().BeNull();
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

			var expectedAvailableArtists = new[]
			{
				new EditedSongProperty<ArtistModel>(null),
				new EditedSongProperty<ArtistModel>(artist1),
				new EditedSongProperty<ArtistModel>(artist2),
			};

			target.AvailableArtists.Should().BeEquivalentTo(expectedAvailableArtists, x => x.WithStrictOrdering());
			target.Artist.Should().BeEquivalentTo(expectedAvailableArtists[2]);
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

			var expectedAvailableArtists = new[]
			{
				new EditedSongProperty<ArtistModel>(null),
				new EditedSongProperty<ArtistModel>(artist1),
				new EditedSongProperty<ArtistModel>(artist2),
			};

			target.AvailableArtists.Should().BeEquivalentTo(expectedAvailableArtists, x => x.WithStrictOrdering());
			target.Artist.Should().BeEquivalentTo(expectedAvailableArtists[0]);
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

			var expectedAvailableArtists = new[]
			{
				new EditedSongProperty<ArtistModel>(null),
				new EditedSongProperty<ArtistModel>(artist1),
				new EditedSongProperty<ArtistModel>(artist2),
			};

			target.AvailableArtists.Should().BeEquivalentTo(expectedAvailableArtists, x => x.WithStrictOrdering());
			target.Artist.Should().BeEquivalentTo(expectedAvailableArtists[2]);
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

			var expectedAvailableArtists = new[]
			{
				new EditedSongProperty<ArtistModel>(null),
				new EditedSongProperty<ArtistModel>(artist1),
				new EditedSongProperty<ArtistModel>(artist2),
			};

			target.AvailableArtists.Should().BeEquivalentTo(expectedAvailableArtists, x => x.WithStrictOrdering());
			target.Artist.Should().BeEquivalentTo(expectedAvailableArtists[0]);
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

			var expectedAvailableArtists = new[]
			{
				new EditedSongProperty<ArtistModel>(),
				new EditedSongProperty<ArtistModel>(null),
				new EditedSongProperty<ArtistModel>(artist1),
				new EditedSongProperty<ArtistModel>(artist2),
			};

			target.AvailableArtists.Should().BeEquivalentTo(expectedAvailableArtists, x => x.WithStrictOrdering());
			target.Artist.Should().BeEquivalentTo(expectedAvailableArtists[0]);
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

			var expectedAvailableGenres = new[]
			{
				new EditedSongProperty<GenreModel>(null),
				new EditedSongProperty<GenreModel>(genre1),
				new EditedSongProperty<GenreModel>(genre2),
			};

			target.AvailableGenres.Should().BeEquivalentTo(expectedAvailableGenres, x => x.WithStrictOrdering());
			target.Genre.Should().BeEquivalentTo(expectedAvailableGenres[2]);
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

			var expectedAvailableGenres = new[]
			{
				new EditedSongProperty<GenreModel>(null),
				new EditedSongProperty<GenreModel>(genre1),
				new EditedSongProperty<GenreModel>(genre2),
			};

			target.AvailableGenres.Should().BeEquivalentTo(expectedAvailableGenres, x => x.WithStrictOrdering());
			target.Genre.Should().BeEquivalentTo(expectedAvailableGenres[0]);
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

			var expectedAvailableGenres = new[]
			{
				new EditedSongProperty<GenreModel>(null),
				new EditedSongProperty<GenreModel>(genre1),
				new EditedSongProperty<GenreModel>(genre2),
			};

			target.AvailableGenres.Should().BeEquivalentTo(expectedAvailableGenres, x => x.WithStrictOrdering());
			target.Genre.Should().BeEquivalentTo(expectedAvailableGenres[2]);
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

			var expectedAvailableGenres = new[]
			{
				new EditedSongProperty<GenreModel>(null),
				new EditedSongProperty<GenreModel>(genre1),
				new EditedSongProperty<GenreModel>(genre2),
			};

			target.AvailableGenres.Should().BeEquivalentTo(expectedAvailableGenres, x => x.WithStrictOrdering());
			target.Genre.Should().BeEquivalentTo(expectedAvailableGenres[0]);
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

			var expectedAvailableGenres = new[]
			{
				new EditedSongProperty<GenreModel>(),
				new EditedSongProperty<GenreModel>(null),
				new EditedSongProperty<GenreModel>(genre1),
				new EditedSongProperty<GenreModel>(genre2),
			};

			target.AvailableGenres.Should().BeEquivalentTo(expectedAvailableGenres, x => x.WithStrictOrdering());
			target.Genre.Should().BeEquivalentTo(expectedAvailableGenres[0]);
		}

		[TestMethod]
		public async Task Load_ForDeletedSongsWithSameDeleteComment_LoadsDeleteCommentCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment" },
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment" },
			};

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(songs, CancellationToken.None);

			// Assert

			target.DeleteComment.Should().Be("Some Delete Comment");
			target.SongsAreDeleted.Should().BeTrue();
		}

		[TestMethod]
		public async Task Load_ForDeletedSongsWithVariousDeleteComment_LoadsDeleteCommentCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment 1" },
				new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Some Delete Comment 2" },
			};

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			await target.Load(songs, CancellationToken.None);

			// Assert

			target.DeleteComment.Should().Be("<Songs have various delete comments>");
			target.SongsAreDeleted.Should().BeTrue();
		}

		[TestMethod]
		public async Task Load_ForEmptySongList_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			// Act

			Func<Task> call = () => target.Load(Enumerable.Empty<SongModel>(), CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>();
		}

		[TestMethod]
		public async Task TreeTitleSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), new SongModel(), }, CancellationToken.None);

			// Act

			Action call = () => target.TreeTitle = "New Tree Title";

			// Assert

			call.Should().Throw<InvalidOperationException>();
		}

		[DataRow(null)]
		[DataRow("")]
		[DataRow(" ")]
		[TestMethod]
		public async Task TreeTitleSetter_ForMissingValue_ThrowsInvalidOperationException(string newTreeTitle)
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), }, CancellationToken.None);

			// Act

			Action call = () => target.TreeTitle = newTreeTitle;

			// Assert

			call.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public async Task TitleSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), new SongModel(), }, CancellationToken.None);

			// Act

			Action call = () => target.Title = "New Title";

			// Assert

			call.Should().Throw<InvalidOperationException>();
		}

		[DataRow(null)]
		[DataRow("")]
		[DataRow(" ")]
		[TestMethod]
		public async Task TitleSetter_ForMissingValue_ThrowsInvalidOperationException(string newTitle)
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), }, CancellationToken.None);

			// Act

			Action call = () => target.Title = newTitle;

			// Assert

			call.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public async Task TrackNumberSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var mocker = CreateMocker();
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			await target.Load(new[] { new SongModel(), new SongModel(), }, CancellationToken.None);

			// Act

			Action call = () => target.TrackNumber = 1;

			// Assert

			call.Should().Throw<InvalidOperationException>();
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

			Func<Action<SongModel>, bool> verifySongUpdate = updateAction =>
			{
				updateAction(song);
				return song.Title == "New Title" && song.TreeTitle == "New Tree Title" && song.TrackNumber == 2 &&
				       Object.ReferenceEquals(song.Artist, newArtist) && Object.ReferenceEquals(song.Genre, newGenre);
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(song, It.Is<Action<SongModel>>(y => verifySongUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);
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
			target.NewArtistName = target.Artist.ToString();
			target.Genre = target.AvailableGenres.Single(x => x.HasBlankValue);
			target.TrackNumber = null;

			await target.Save(CancellationToken.None);

			// Assert

			Func<Action<SongModel>, bool> verifySongUpdate = updateAction =>
			{
				updateAction(song);
				return song.Title == "Some Title" && song.TreeTitle == "Some Tree Title" &&
				       song.TrackNumber == null && song.Artist == null && song.Genre == null;
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(song, It.Is<Action<SongModel>>(y => verifySongUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForSingleSongWhenNewArtistIsCreated_UpdatesSongCorrectly()
		{
			// Arrange

			var oldArtist = new ArtistModel { Id = new ItemId("1"), Name = "Old Artist" };

			var mocker = CreateMocker(artists: new[] { oldArtist }, genres: Array.Empty<GenreModel>());
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel
			{
				Artist = oldArtist,
			};

			await target.Load(new[] { song }, CancellationToken.None);

			// Act

			target.Artist = null;
			target.NewArtistName = "New Artist";

			await target.Save(CancellationToken.None);

			// Assert

			var artistServiceMock = mocker.GetMock<IArtistsService>();
			Func<ArtistModel, bool> checkArtist = x => x.Name == "New Artist";
			artistServiceMock.Verify(x => x.CreateArtist(It.Is<ArtistModel>(y => checkArtist(y)), It.IsAny<CancellationToken>()), Times.Once);

			Func<Action<SongModel>, bool> verifySongUpdate = updateAction =>
			{
				updateAction(song);
				return song.Artist.Name == "New Artist";
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(song, It.Is<Action<SongModel>>(y => verifySongUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForSingleSongWhenExistingArtistNameIsTyped_UpdatesSongCorrectly()
		{
			// Arrange

			var oldArtist = new ArtistModel { Id = new ItemId("1"), Name = "Old Artist" };
			var existingArtist = new ArtistModel { Id = new ItemId("2"), Name = "Existing Artist" };

			var mocker = CreateMocker(artists: new[] { oldArtist, existingArtist }, genres: Array.Empty<GenreModel>());
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel
			{
				Artist = oldArtist,
			};

			await target.Load(new[] { song }, CancellationToken.None);

			// Act

			target.Artist = target.AvailableArtists.Last();
			target.NewArtistName = "Existing Artist";

			await target.Save(CancellationToken.None);

			// Assert

			var artistServiceMock = mocker.GetMock<IArtistsService>();
			artistServiceMock.Verify(x => x.CreateArtist(It.IsAny<ArtistModel>(), It.IsAny<CancellationToken>()), Times.Never);

			Func<Action<SongModel>, bool> verifySongUpdate = updateAction =>
			{
				updateAction(song);
				return song.Artist.Name == "Existing Artist";
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(song, It.Is<Action<SongModel>>(y => verifySongUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForSingleSongWhenPartiallyMatchingArtistNameIsTyped_UpdatesSongCorrectly()
		{
			// Arrange

			var oldArtist = new ArtistModel { Id = new ItemId("1"), Name = "Old Artist" };
			var existingArtist = new ArtistModel { Id = new ItemId("2"), Name = "Metallica & Nirvana" };

			var mocker = CreateMocker(artists: new[] { oldArtist, existingArtist }, genres: Array.Empty<GenreModel>());
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song = new SongModel
			{
				Artist = oldArtist,
			};

			await target.Load(new[] { song }, CancellationToken.None);

			// Act

			target.Artist = target.AvailableArtists.Last();
			target.NewArtistName = "Metallica";

			await target.Save(CancellationToken.None);

			// Assert

			var artistServiceMock = mocker.GetMock<IArtistsService>();
			Func<ArtistModel, bool> checkArtist = x => x.Name == "Metallica";
			artistServiceMock.Verify(x => x.CreateArtist(It.Is<ArtistModel>(y => checkArtist(y)), It.IsAny<CancellationToken>()), Times.Once);

			Func<Action<SongModel>, bool> verifySongUpdate = updateAction =>
			{
				updateAction(song);
				return song.Artist.Name == "Metallica";
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(song, It.Is<Action<SongModel>>(y => verifySongUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);
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

			Func<Action<SongModel>, bool> verifySongUpdate1 = updateAction =>
			{
				updateAction(song1);
				return song1.Title == "Old Title 1" && song1.TreeTitle == "Old Tree Title 1" && song1.TrackNumber == 1 &&
				       Object.ReferenceEquals(song1.Artist, newArtist) && Object.ReferenceEquals(song1.Genre, newGenre);
			};

			Func<Action<SongModel>, bool> verifySongUpdate2 = updateAction =>
			{
				updateAction(song2);
				return song2.Title == "Old Title 2" && song2.TreeTitle == "Old Tree Title 2" && song2.TrackNumber == 2 &&
				       Object.ReferenceEquals(song2.Artist, newArtist) && Object.ReferenceEquals(song2.Genre, newGenre);
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songServiceMock.Verify(x => x.UpdateSong(song1, It.Is<Action<SongModel>>(y => verifySongUpdate1(y)), It.IsAny<CancellationToken>()), Times.Once);
			songServiceMock.Verify(x => x.UpdateSong(song2, It.Is<Action<SongModel>>(y => verifySongUpdate2(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForMultipleSongsWhenNewArtistIsCreated_UpdatesSongCorrectly()
		{
			// Arrange

			var oldArtist = new ArtistModel { Id = new ItemId("1"), Name = "Old Artist" };

			var mocker = CreateMocker(artists: new[] { oldArtist }, genres: Array.Empty<GenreModel>());
			var target = mocker.CreateInstance<EditSongPropertiesViewModel>();

			var song1 = new SongModel
			{
				Id = new ItemId("1"),
				Artist = oldArtist,
			};

			var song2 = new SongModel
			{
				Id = new ItemId("2"),
				Artist = oldArtist,
			};

			await target.Load(new[] { song1, song2, }, CancellationToken.None);

			// Act

			target.Artist = null;
			target.NewArtistName = "New Artist";

			await target.Save(CancellationToken.None);

			// Assert

			var artistServiceMock = mocker.GetMock<IArtistsService>();
			Func<ArtistModel, bool> checkArtist = x => x.Name == "New Artist";
			artistServiceMock.Verify(x => x.CreateArtist(It.Is<ArtistModel>(y => checkArtist(y)), It.IsAny<CancellationToken>()), Times.Once);

			Func<SongModel, Action<SongModel>, bool> verifySongUpdate = (song, updateAction) =>
			{
				updateAction(song);
				return song.Artist.Name == "New Artist";
			};
			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songServiceMock.Verify(x => x.UpdateSong(song1, It.Is<Action<SongModel>>(y => verifySongUpdate(song1, y)), It.IsAny<CancellationToken>()), Times.Once);
			songServiceMock.Verify(x => x.UpdateSong(song2, It.Is<Action<SongModel>>(y => verifySongUpdate(song2, y)), It.IsAny<CancellationToken>()), Times.Once);
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

			Func<Action<SongModel>, bool> verifySongUpdate1 = updateAction =>
			{
				updateAction(song1);
				return song1.Title == "Old Title 1" && song1.TreeTitle == "Old Tree Title 1" && song1.TrackNumber == 1 &&
				       Object.ReferenceEquals(song1.Artist, artist1) && Object.ReferenceEquals(song1.Genre, genre1);
			};

			Func<Action<SongModel>, bool> verifySongUpdate2 = updateAction =>
			{
				updateAction(song2);
				return song2.Title == "Old Title 2" && song2.TreeTitle == "Old Tree Title 2" && song2.TrackNumber == 2 &&
				       Object.ReferenceEquals(song2.Artist, artist2) && Object.ReferenceEquals(song2.Genre, genre2);
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			songServiceMock.Verify(x => x.UpdateSong(song1, It.Is<Action<SongModel>>(y => verifySongUpdate1(y)), It.IsAny<CancellationToken>()), Times.Once);
			songServiceMock.Verify(x => x.UpdateSong(song2, It.Is<Action<SongModel>>(y => verifySongUpdate2(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Save_ForSingleDeletedSongWhenDeleteCommentIsUpdated_UpdatesSongCorrectly()
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
				DeleteDate = new DateTime(2021, 10, 26),
				DeleteComment = "Old Delete Comment",
			};

			await target.Load(new[] { song }, CancellationToken.None);

			// Act

			target.Title = "New Title";
			target.TreeTitle = "New Tree Title";
			target.Artist = target.AvailableArtists.Last();
			target.Genre = target.AvailableGenres.Last();
			target.TrackNumber = 2;
			target.DeleteComment = "New Delete Comment";

			await target.Save(CancellationToken.None);

			// Assert

			Func<Action<SongModel>, bool> verifySongUpdate = updateAction =>
			{
				updateAction(song);
				return song.Title == "New Title" && song.TreeTitle == "New Tree Title" && song.TrackNumber == 2 &&
				       Object.ReferenceEquals(song.Artist, newArtist) && Object.ReferenceEquals(song.Genre, newGenre) &&
				       song.DeleteComment == "New Delete Comment";
			};

			var songServiceMock = mocker.GetMock<ISongsService>();
			songServiceMock.Verify(x => x.UpdateSong(It.IsAny<SongModel>(), It.IsAny<Action<SongModel>>(), It.IsAny<CancellationToken>()), Times.Once);
			songServiceMock.Verify(x => x.UpdateSong(song, It.Is<Action<SongModel>>(y => verifySongUpdate(y)), It.IsAny<CancellationToken>()), Times.Once);
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
