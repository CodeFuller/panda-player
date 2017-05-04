using System;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using NSubstitute;
using NUnit.Framework;
using static System.FormattableString;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	[TestFixture]
	public class EditAlbumsDetailsViewModelTests
	{
		private const string TestWorkshopMusicStorage = @"d:\music.test";

		[Test]
		public void SetAlbums_ForAddedArtistAlbum_FillsAlbumDataCorrectly()
		{
			//	Arrange

			AlbumContent albumContent = new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Album"), Enumerable.Empty<string>());
			AlbumTreeViewItem album = new AlbumTreeViewItem(albumContent);

			var musicLibrary = Substitute.For<IMusicLibrary>();
			musicLibrary.ArtistLibrary.Returns(new ArtistLibrary(Enumerable.Empty<LibraryArtist>()));

			var target = new EditAlbumsDetailsViewModel(musicLibrary, new LocalWorkshopMusicStorage(TestWorkshopMusicStorage),
				Substitute.For<IStorageUrlBuilder>(), Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetAlbums(Enumerable.Repeat(album, 1)).Wait();

			//	Assert

			var addedAlbum = target.Albums.Single() as AddedArtistAlbum;
			Assert.IsNotNull(addedAlbum);
			Assert.AreEqual(@"Some Artist\2000 - Some Album", addedAlbum.PathWithinWorkshopStorage);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Album"), addedAlbum.SourcePath);
			Assert.AreEqual(AlbumType.ArtistAlbum, addedAlbum.AlbumsType);
			Assert.AreEqual("Some Artist", addedAlbum.Artist);
			Assert.AreEqual("Some Album", addedAlbum.Title);
			Assert.AreEqual(2000, addedAlbum.Year);
			Assert.IsNull(addedAlbum.Genre);
			Assert.AreEqual("2000 - Some Album", addedAlbum.NameInStorage);
			Assert.IsNull(addedAlbum.DestinationUri);
		}

		[Test]
		public void SetAlbums_ForAddedCompilationAlbumWithArtistInfo_FillsAlbumDataCorrectly()
		{
			//	Arrange

			string[] songs =
			{
				@"01 - Marilyn Manson - Rock Is Dead.mp3",
				@"02 - Propellerheads - Spybreak! (Short One).mp3",
			};

			AlbumContent albumContent = new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"), songs);
			AlbumTreeViewItem album = new AlbumTreeViewItem(albumContent);

			var musicLibrary = Substitute.For<IMusicLibrary>();
			musicLibrary.ArtistLibrary.Returns(new ArtistLibrary(Enumerable.Empty<LibraryArtist>()));

			IStorageUrlBuilder storageUrlBuilder = Substitute.For<IStorageUrlBuilder>();
			storageUrlBuilder.MapWorkshopStoragePath(@"Soundtracks\Some Movie").Returns(new Uri("/Soundtracks/Some Movie", UriKind.Relative));

			var target = new EditAlbumsDetailsViewModel(musicLibrary, new LocalWorkshopMusicStorage(TestWorkshopMusicStorage),
				storageUrlBuilder, Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetAlbums(Enumerable.Repeat(album, 1)).Wait();

			//	Assert

			var addedAlbum = target.Albums.Single() as AddedCompilationAlbumWithArtistInfo;
			Assert.IsNotNull(addedAlbum);
			Assert.AreEqual(@"Soundtracks\Some Movie", addedAlbum.PathWithinWorkshopStorage);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"), addedAlbum.SourcePath);
			Assert.AreEqual(AlbumType.CompilationAlbumWithArtistInfo, addedAlbum.AlbumsType);
			Assert.IsNull(addedAlbum.Artist);
			Assert.AreEqual("Some Movie", addedAlbum.Title);
			Assert.IsNull(addedAlbum.Year);
			Assert.IsNull(addedAlbum.Genre);
			Assert.AreEqual(new Uri("/Soundtracks/Some Movie", UriKind.Relative), addedAlbum.DestinationUri);
		}

		[Test]
		public void SetAlbums_ForAddedCompilationAlbumWithoutArtistInfo_FillsAlbumDataCorrectly()
		{
			//	Arrange

			string[] songs =
			{
				@"01 - Half Remembered Dream.mp3",
				@"02 - We Built Our Own World.mp3",
			};

			AlbumContent albumContent = new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"), songs);
			AlbumTreeViewItem album = new AlbumTreeViewItem(albumContent);

			var musicLibrary = Substitute.For<IMusicLibrary>();
			musicLibrary.ArtistLibrary.Returns(new ArtistLibrary(Enumerable.Empty<LibraryArtist>()));

			IStorageUrlBuilder storageUrlBuilder = Substitute.For<IStorageUrlBuilder>();
			storageUrlBuilder.MapWorkshopStoragePath(@"Soundtracks\Some Movie").Returns(new Uri("/Soundtracks/Some Movie", UriKind.Relative));

			var target = new EditAlbumsDetailsViewModel(musicLibrary, new LocalWorkshopMusicStorage(TestWorkshopMusicStorage),
				storageUrlBuilder, Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetAlbums(Enumerable.Repeat(album, 1)).Wait();

			//	Assert

			var addedAlbum = target.Albums.Single() as AddedCompilationAlbumWithoutArtistInfo;
			Assert.IsNotNull(addedAlbum);
			Assert.AreEqual(@"Soundtracks\Some Movie", addedAlbum.PathWithinWorkshopStorage);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"), addedAlbum.SourcePath);
			Assert.AreEqual(AlbumType.CompilationAlbumWithoutArtistInfo, addedAlbum.AlbumsType);
			Assert.IsNull(addedAlbum.Artist);
			Assert.AreEqual("Some Movie", addedAlbum.Title);
			Assert.IsNull(addedAlbum.Year);
			Assert.IsNull(addedAlbum.Genre);
			Assert.AreEqual(new Uri("/Soundtracks/Some Movie", UriKind.Relative), addedAlbum.DestinationUri);
		}

		[Test]
		public void SetAlbums_ForKnownArtist_SetsDestinationUriCorrectly()
		{
			//	Arrange

			AlbumContent albumContent = new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Album"), Enumerable.Empty<string>());
			AlbumTreeViewItem album = new AlbumTreeViewItem(albumContent);

			LibraryArtist artist = new LibraryArtist("SomeId", "Some Artist", new Uri("/SomeCategory/Some Artist", UriKind.Relative));
			var musicLibrary = new CatalogBasedMusicLibrary(Substitute.For<IMusicCatalog>(),
				Substitute.For<IMusicStorage>(), Substitute.For<IArtistLibraryBuilder>(), Substitute.For<IStorageUrlBuilder>())
			{
				ArtistLibrary = new ArtistLibrary(Enumerable.Repeat(artist, 1))
			};

			var target = new EditAlbumsDetailsViewModel(musicLibrary, new LocalWorkshopMusicStorage(TestWorkshopMusicStorage),
				new StorageUrlBuilder(), Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetAlbums(Enumerable.Repeat(album, 1)).Wait();

			//	Assert

			var addedAlbum = target.Albums.Single();
			Assert.AreEqual(new Uri("/SomeCategory/Some Artist/2000 - Some Album", UriKind.Relative), addedAlbum.DestinationUri);
		}

		[Test]
		public void SetAlbums_ForKnownArtist_CopiesGenreFromLastAlbum()
		{
			//	Arrange

			AlbumContent albumContent = new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Album"), Enumerable.Empty<string>());
			AlbumTreeViewItem album = new AlbumTreeViewItem(albumContent);

			var genre1 = new Genre
			{
				Id = 1,
				Name = "Gothic Metal",
			};
			var genre2 = new Genre
			{
				Id = 2,
				Name = "Symphonic Metal",
			};

			LibraryArtist artist = new LibraryArtist("SomeId", "Some Artist", new Uri("/SomeCategory/Some Artist", UriKind.Relative));
			artist.AddDisc(new LibraryDisc(artist, new Disc
			{
				Songs = new Collection<Song>
				{
					new Song
					{
						Year = 2000,
						Genre = genre1,
					}
				}
			}));

			artist.AddDisc(new LibraryDisc(artist, new Disc
			{
				Songs = new Collection<Song>
				{
					new Song
					{
						Year = 2005,
						Genre = genre2,
					}
				}
			}));

			var musicLibrary = new CatalogBasedMusicLibrary(Substitute.For<IMusicCatalog>(),
				Substitute.For<IMusicStorage>(), Substitute.For<IArtistLibraryBuilder>(), Substitute.For<IStorageUrlBuilder>())
			{
				ArtistLibrary = new ArtistLibrary(Enumerable.Repeat(artist, 1))
			};

			var target = new EditAlbumsDetailsViewModel(musicLibrary, new LocalWorkshopMusicStorage(TestWorkshopMusicStorage),
				new StorageUrlBuilder(), Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetAlbums(Enumerable.Repeat(album, 1)).Wait();

			//	Assert

			var addedAlbum = target.Albums.Single();
			Assert.AreSame(genre2, addedAlbum.Genre);
		}

		[Test]
		public void Songs_ReturnsCorrectSongsData()
		{
			//	Arrange

			AlbumContent[] albums = 
			{
				new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Nightwish\2000 - Wishmaster"), new[]
				{
					"01 - She Is My Sin.mp3",
				}),

				new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Gladiator"), new[]
				{
					"01 - Progeny.mp3",
				}),

				new AlbumContent(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix"), new[]
				{
					"01 - Marilyn Manson - Rock Is Dead.mp3",
					"02 - Propellerheads - Spybreak! (Short One).mp3",
				}),
			};

			var musicLibrary = Substitute.For<IMusicLibrary>();
			musicLibrary.ArtistLibrary.Returns(new ArtistLibrary(Enumerable.Empty<LibraryArtist>()));

			var target = new EditAlbumsDetailsViewModel(musicLibrary, new LocalWorkshopMusicStorage(TestWorkshopMusicStorage),
				new StorageUrlBuilder(), Substitute.For<IFileSystemFacade>());

			target.SetAlbums(albums.Select(a => new AlbumTreeViewItem(a))).Wait();

			//	Emulating editing of album data by the user.
			target.Albums[0].DestinationUri = new Uri("/Foreign/Nightwish/2000 - Wishmaster", UriKind.Relative);
			target.Albums[0].Genre = new Genre {Name = "Gothic Metal"};
			target.Albums[1].Genre = new Genre { Name = "Soundtrack" };
			target.Albums[1].Artist = "Hans Zimmer";
			target.Albums[1].Year = 2000;
			target.Albums[2].Genre = new Genre { Name = "Soundtrack" };

			//	Act

			var songs = target.Songs.ToList();

			//	Assert

			Assert.AreEqual(4, songs.Count);

			var song1 = songs[0];
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Nightwish\2000 - Wishmaster\01 - She Is My Sin.mp3"), song1.SourceFileName);
			Assert.AreEqual(new Uri("/Foreign/Nightwish/2000 - Wishmaster/01 - She Is My Sin.mp3", UriKind.Relative), song1.StorageUri);
			Assert.AreEqual("Nightwish", song1.Artist);
			Assert.AreEqual("Wishmaster", song1.Album);
			Assert.AreEqual(2000, song1.Year);
			Assert.AreEqual("Gothic Metal", song1.Genre);
			Assert.AreEqual(1, song1.Track);
			Assert.AreEqual("She Is My Sin", song1.Title);

			var song2 = songs[1];
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Gladiator\01 - Progeny.mp3"), song2.SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/Gladiator/01 - Progeny.mp3", UriKind.Relative), song2.StorageUri);
			Assert.AreEqual("Hans Zimmer", song2.Artist);
			Assert.AreEqual("Gladiator", song2.Album);
			Assert.AreEqual(2000, song2.Year);
			Assert.AreEqual("Soundtrack", song2.Genre);
			Assert.AreEqual(1, song2.Track);
			Assert.AreEqual("Progeny", song2.Title);

			var song3 = songs[2];
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix\01 - Marilyn Manson - Rock Is Dead.mp3"), song3.SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/The Matrix/01 - Marilyn Manson - Rock Is Dead.mp3", UriKind.Relative), song3.StorageUri);
			Assert.AreEqual("Marilyn Manson", song3.Artist);
			Assert.AreEqual("The Matrix", song3.Album);
			Assert.IsNull(song3.Year);
			Assert.AreEqual("Soundtrack", song3.Genre);
			Assert.AreEqual(1, song3.Track);
			Assert.AreEqual("Rock Is Dead", song3.Title);

			var song4 = songs[3];
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix\02 - Propellerheads - Spybreak! (Short One).mp3"), song4.SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/The Matrix/02 - Propellerheads - Spybreak! (Short One).mp3", UriKind.Relative), song4.StorageUri);
			Assert.AreEqual("Propellerheads", song4.Artist);
			Assert.AreEqual("The Matrix", song4.Album);
			Assert.IsNull(song4.Year);
			Assert.AreEqual("Soundtrack", song4.Genre);
			Assert.AreEqual(2, song4.Track);
			Assert.AreEqual("Spybreak! (Short One)", song4.Title);
		}
	}
}
