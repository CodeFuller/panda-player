using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.ViewModels
{
	[TestFixture]
	public class EditSongPropertiesViewModelTests
	{
		[Test]
		public void Load_FillsCollectionOfAvailableArtistsCorrectly()
		{
			// Arrange

			var artist1 = new Artist { Name = "Some Artist" };
			var artist2 = new Artist { Name = "Another Artist" };

			var discs = new List<Disc>()
			{
				new Disc { SongsUnordered = new[] { new Song { Artist = artist1 } } },
				new Disc { SongsUnordered = new[] { new Song { Artist = artist2 } } },
			};
			var library = new DiscLibrary(discs);

			var target = new EditSongPropertiesViewModel(library, Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());

			// Act

			target.Load(new[] { new Song() });

			// Assert

			var availableArtistsList = target.AvailableArtists.ToList();
			Assert.AreEqual(4, availableArtistsList.Count);

			// 'Keep value' entry
			Assert.IsFalse(availableArtistsList[0].HasValue);

			// 'Blank' entry
			Assert.IsTrue(availableArtistsList[1].HasValue);
			Assert.IsNull(availableArtistsList[1].Value);

			// 'Another Artist' entry
			Assert.IsTrue(availableArtistsList[2].HasValue);
			Assert.AreSame(artist2, availableArtistsList[2].Value);

			// 'Some Artist' entry
			Assert.IsTrue(availableArtistsList[3].HasValue);
			Assert.AreSame(artist1, availableArtistsList[3].Value);
		}

		[Test]
		public void Load_FillsCollectionOfAvailableGenresCorrectly()
		{
			// Arrange

			var genre1 = new Genre { Name = "Some Genre" };
			var genre2 = new Genre { Name = "Another Genre" };

			var discs = new List<Disc>
			{
				new Disc { SongsUnordered = new[] { new Song { Genre = genre1 } } },
				new Disc { SongsUnordered = new[] { new Song { Genre = genre2 } } },
			};
			var library = new DiscLibrary(discs);

			var target = new EditSongPropertiesViewModel(library, Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());

			// Act

			target.Load(new[] { new Song() });

			// Assert

			var availableGenresList = target.AvailableGenres.ToList();
			Assert.AreEqual(4, availableGenresList.Count);

			// 'Keep value' entry
			Assert.IsFalse(availableGenresList[0].HasValue);

			// 'Blank' entry
			Assert.IsTrue(availableGenresList[1].HasValue);
			Assert.IsNull(availableGenresList[1].Value);

			// 'Another Genre' entry
			Assert.IsTrue(availableGenresList[2].HasValue);
			Assert.AreSame(genre2, availableGenresList[2].Value);

			// 'Some Genre' entry
			Assert.IsTrue(availableGenresList[3].HasValue);
			Assert.AreSame(genre1, availableGenresList[3].Value);
		}

		[Test]
		public void Load_WhenSongListIsEmpty_ThrowsInvalidOperationException()
		{
			// Arrange

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());

			// Act & Arrange

			Assert.Throws<InvalidOperationException>(() => target.Load(Enumerable.Empty<Song>()));
		}

		[Test]
		public void SongPropertiesGetters_ForSingleSong_ReturnCorrectValues()
		{
			// Arrange

			var artist = new Artist();
			var genre = new Genre();

			var libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.GetFileNameFromUri(new Uri("/SomeDisc/Uri/SomeFileName.mp3", UriKind.Relative)).Returns("SomeFileName.mp3");

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), libraryStructurer, Substitute.For<ILibraryContentUpdater>());
			target.Load(new[]
			{
				new Song
				{
					Uri = new Uri("/SomeDisc/Uri/SomeFileName.mp3", UriKind.Relative),
					Title = "Some song",
					Artist = artist,
					Genre = genre,
					Year = 2017,
					TrackNumber = 7,
				}
			});

			// Act & Assert

			Assert.AreEqual("SomeFileName.mp3", target.FileName);
			Assert.AreEqual("Some song", target.Title);
			Assert.AreSame(artist, target.Artist.Value);
			Assert.AreSame(genre, target.Genre.Value);
			Assert.AreEqual(2017, target.Year.Value);
			Assert.AreEqual(7, target.TrackNumber.Value);
		}

		[Test]
		public void SongPropertiesGetters_ForMultipleSongsWithEqualProperties_ReturnCorrectValues()
		{
			// Arrange

			var artist = new Artist();
			var genre = new Genre();

			var libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.GetFileNameFromUri(new Uri("/SomeDisc/Uri/SomeFileName.mp3", UriKind.Relative)).Returns("SomeFileName.mp3");

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), libraryStructurer, Substitute.For<ILibraryContentUpdater>());
			target.Load(new[]
			{
				new Song
				{
					Uri = new Uri("/SomeDisc/Uri/SomeFileName.mp3", UriKind.Relative),
					Title = "Some song",
					Artist = artist,
					Genre = genre,
					Year = 2017,
					TrackNumber = 7,
				},

				new Song
				{
					Uri = new Uri("/SomeDisc/Uri/SomeFileName.mp3", UriKind.Relative),
					Title = "Some song",
					Artist = artist,
					Genre = genre,
					Year = 2017,
					TrackNumber = 7,
				}
			});

			// Act & Assert

			Assert.IsNull(target.FileName);
			Assert.IsNull(target.Title);
			Assert.AreSame(artist, target.Artist.Value);
			Assert.AreSame(genre, target.Genre.Value);
			Assert.AreEqual(2017, target.Year.Value);
			Assert.AreEqual(7, target.TrackNumber.Value);
		}

		[Test]
		public void SongPropertiesGetters_ForMultipleSongsWithDifferentProperties_ReturnCorrectValues()
		{
			// Arrange

			var libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.GetFileNameFromUri(new Uri("/SomeDisc/Uri/SomeFileName.mp3", UriKind.Relative)).Returns("SomeFileName.mp3");

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), libraryStructurer, Substitute.For<ILibraryContentUpdater>());
			target.Load(new[]
			{
				new Song
				{
					Artist = new Artist { Id = 1 },
					Genre = new Genre { Id = 1 },
					Year = 2017,
					TrackNumber = 7,
				},

				new Song
				{
					Artist = new Artist { Id = 2 },
					Genre = new Genre { Id = 2 },
					Year = 2018,
					TrackNumber = 8,
				}
			});

			// Act & Assert

			Assert.IsFalse(target.Artist.HasValue);
			Assert.IsFalse(target.Genre.HasValue);
			Assert.IsFalse(target.Year.HasValue);
			Assert.IsFalse(target.TrackNumber.HasValue);
		}

		[Test]
		public void Save_ForSingleSongWhenNoPropertiesAreUpdated_UpdatesSongsCorrectly()
		{
			// Arrange

			var artist = new Artist();
			var genre = new Genre();

			List<Song> songsUpdate = null;
			UpdatedSongProperties? updatedProperties = null;
			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			libraryContentUpdaterMock.UpdateSongs(Arg.Do<IEnumerable<Song>>(arg => songsUpdate = arg.ToList()), Arg.Do<UpdatedSongProperties>(arg => updatedProperties = arg));

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), libraryContentUpdaterMock);
			target.Load(new[]
			{
				new Song
				{
					Title = "Some song",
					Artist = artist,
					Genre = genre,
					Year = 2017,
					TrackNumber = 7,
				}
			});

			// Act

			target.Save().Wait();

			// Assert

			Assert.AreEqual(1, songsUpdate.Count);
			var song = songsUpdate.Single();
			Assert.AreEqual("Some song", song.Title);
			Assert.AreSame(artist, song.Artist);
			Assert.AreSame(genre, song.Genre);
			Assert.AreEqual(2017, song.Year);
			Assert.AreEqual(7, song.TrackNumber);
			Assert.AreEqual(UpdatedSongProperties.ForceTagUpdate, updatedProperties);
		}

		[Test]
		public void Save_ForSingleSongWhenPropertiesAreUpdated_UpdatesSongsCorrectly()
		{
			// Arrange

			var updatedArtist = new Artist();
			var updatedGenre = new Genre();

			List<Song> songsUpdate = null;
			UpdatedSongProperties? updatedProperties = null;
			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			libraryContentUpdaterMock.UpdateSongs(Arg.Do<IEnumerable<Song>>(arg => songsUpdate = arg.ToList()), Arg.Do<UpdatedSongProperties>(arg => updatedProperties = arg));

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), libraryContentUpdaterMock);
			target.Load(new[]
			{
				new Song
				{
					Title = "Some song",
					Artist = new Artist(),
					Genre = new Genre(),
					Year = 2017,
					TrackNumber = 7,
				}
			});

			// Act

			target.Title = "Updated title";
			target.Artist = new EditedSongProperty<Artist>(updatedArtist);
			target.Genre = new EditedSongProperty<Genre>(updatedGenre);
			target.Year = new EditedSongProperty<short?>(2018);
			target.TrackNumber = new EditedSongProperty<short?>(1);

			target.Save().Wait();

			// Assert

			Assert.AreEqual(1, songsUpdate.Count);
			var song = songsUpdate.Single();
			Assert.AreEqual("Updated title", song.Title);
			Assert.AreSame(updatedArtist, song.Artist);
			Assert.AreSame(updatedGenre, song.Genre);
			Assert.AreEqual(2018, song.Year);
			Assert.AreEqual(1, song.TrackNumber);
			Assert.AreEqual(UpdatedSongProperties.ForceTagUpdate, updatedProperties);
		}

		[Test]
		public void Save_ForSingleSongWhenPropertiesAreCleared_UpdatesSongsCorrectly()
		{
			// Arrange

			List<Song> songsUpdate = null;
			UpdatedSongProperties? updatedProperties = null;
			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			libraryContentUpdaterMock.UpdateSongs(Arg.Do<IEnumerable<Song>>(arg => songsUpdate = arg.ToList()), Arg.Do<UpdatedSongProperties>(arg => updatedProperties = arg));

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), libraryContentUpdaterMock);
			target.Load(new[]
			{
				new Song
				{
					Title = "Some song",
					Artist = new Artist(),
					Genre = new Genre(),
					Year = 2017,
					TrackNumber = 7,
				}
			});

			// Act

			target.Artist = new EditedSongProperty<Artist>(null);
			target.Genre = new EditedSongProperty<Genre>(null);
			target.Year = new EditedSongProperty<short?>(null);
			target.TrackNumber = new EditedSongProperty<short?>(null);

			target.Save().Wait();

			// Assert

			Assert.AreEqual(1, songsUpdate.Count);
			var song = songsUpdate.Single();
			Assert.IsNull(song.Artist);
			Assert.IsNull(song.Genre);
			Assert.IsNull(song.Year);
			Assert.IsNull(song.TrackNumber);
			Assert.AreEqual(UpdatedSongProperties.ForceTagUpdate, updatedProperties);
		}

		[Test]
		public void Save_ForMultipleSongsWhenPropertiesAreUpdated_UpdatesSongsCorrectly()
		{
			// Arrange

			var updatedArtist = new Artist();
			var updatedGenre = new Genre();

			List<Song> songsUpdate = null;
			UpdatedSongProperties? updatedProperties = null;
			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			libraryContentUpdaterMock.UpdateSongs(Arg.Do<IEnumerable<Song>>(arg => songsUpdate = arg.ToList()), Arg.Do<UpdatedSongProperties>(arg => updatedProperties = arg));

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), libraryContentUpdaterMock);
			target.Load(new[]
			{
				new Song
				{
					Title = "Some song",
					Artist = new Artist(),
					Genre = new Genre(),
					Year = 2017,
					TrackNumber = 7,
				},

				new Song
				{
					Title = "Some song 2",
					Artist = new Artist(),
					Genre = new Genre(),
					Year = 2018,
					TrackNumber = 8,
				},
			});

			// Act

			target.Artist = new EditedSongProperty<Artist>(updatedArtist);
			target.Genre = new EditedSongProperty<Genre>(updatedGenre);
			target.Year = new EditedSongProperty<short?>(2019);
			target.TrackNumber = new EditedSongProperty<short?>(null);

			target.Save().Wait();

			// Assert

			Assert.AreEqual(2, songsUpdate.Count);
			Assert.AreSame(updatedArtist, songsUpdate[0].Artist);
			Assert.AreSame(updatedGenre, songsUpdate[0].Genre);
			Assert.AreEqual(2019, songsUpdate[0].Year);
			Assert.IsNull(songsUpdate[0].TrackNumber);
			Assert.AreSame(updatedArtist, songsUpdate[1].Artist);
			Assert.AreSame(updatedGenre, songsUpdate[1].Genre);
			Assert.AreEqual(2019, songsUpdate[1].Year);
			Assert.IsNull(songsUpdate[1].TrackNumber);
			Assert.AreEqual(UpdatedSongProperties.ForceTagUpdate, updatedProperties);
		}

		[Test]
		public void Save_ForSingleSongWhenFileNameWasNotChanged_DoesNotChangeSongUri()
		{
			// Arrange

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();

			var libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.BuildSongUri(Arg.Any<Uri>(), "SomeFileName.mp3").Returns(new Uri("SomeFileName.mp3", UriKind.Relative));
			var target = new EditSongPropertiesViewModel(new DiscLibrary(), libraryStructurer, libraryContentUpdaterMock);
			target.Load(new[]
			{
				new Song { Uri = new Uri("SomeFileName.mp3", UriKind.Relative) }
			});

			// Act

			target.Save().Wait();

			// Assert

			libraryContentUpdaterMock.DidNotReceive().ChangeSongUri(Arg.Any<Song>(), Arg.Any<Uri>());
		}

		[Test]
		public void Save_ForSingleSongWhenFileNameWasChanged_ChangesSongUri()
		{
			// Arrange

			var song = new Song
			{
				Disc = new Disc { Uri = new Uri("/SomeDiscUri", UriKind.Relative) },
				Uri = new Uri("SomeFileName.mp3", UriKind.Relative)
			};

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();

			var libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.BuildSongUri(new Uri("/SomeDiscUri", UriKind.Relative), "NewFileName.mp3").Returns(new Uri("/SomeDiscUri/NewFileName.mp3", UriKind.Relative));
			var target = new EditSongPropertiesViewModel(new DiscLibrary(), libraryStructurer, libraryContentUpdaterMock);
			target.Load(new[] { song });

			// Act

			target.FileName = "NewFileName.mp3";
			target.Save().Wait();

			// Assert

			libraryContentUpdaterMock.Received(1).ChangeSongUri(song, new Uri("/SomeDiscUri/NewFileName.mp3", UriKind.Relative));
		}

		[Test]
		public void Save_ForMultipleSongs_DoesNotChangeSongsUri()
		{
			// Arrange

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();

			var libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.BuildSongUri(new Uri("/SomeDiscUri", UriKind.Relative), "NewFileName.mp3").Returns(new Uri("/SomeDiscUri/NewFileName.mp3", UriKind.Relative));
			var target = new EditSongPropertiesViewModel(new DiscLibrary(), libraryStructurer, libraryContentUpdaterMock);
			target.Load(new[]
			{
				new Song
				{
					Disc = new Disc { Uri = new Uri("/SomeDiscUri", UriKind.Relative) },
					Uri = new Uri("SomeFileName1.mp3", UriKind.Relative)
				},
				new Song
				{
					Disc = new Disc { Uri = new Uri("/SomeDiscUri", UriKind.Relative) },
					Uri = new Uri("SomeFileName2.mp3", UriKind.Relative)
				},
			});

			// Act

			target.Save().Wait();

			// Assert

			libraryContentUpdaterMock.DidNotReceive().ChangeSongUri(Arg.Any<Song>(), Arg.Any<Uri>());
		}

		[Test]
		public void FileNameSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());
			target.Load(new[]
			{
				new Song(),
				new Song(),
			});

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.FileName = "SomeFileName");
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void FileNameSetter_WhenNewFileNameIsEmpty_ThrowsInvalidOperationException(string newFileName)
		{
			// Arrange

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());
			target.Load(new[] { new Song() });

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.FileName = newFileName);
		}

		[Test]
		public void TitleSetter_ForMultipleSongs_ThrowsInvalidOperationException()
		{
			// Arrange

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());
			target.Load(new[]
			{
				new Song(),
				new Song(),
			});

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Title = "Some Title");
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void TitleSetter_WhenNewTitleIsEmpty_ThrowsInvalidOperationException(string newTitle)
		{
			// Arrange

			var target = new EditSongPropertiesViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());
			target.Load(new[] { new Song() });

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Title = newTitle);
		}
	}
}
