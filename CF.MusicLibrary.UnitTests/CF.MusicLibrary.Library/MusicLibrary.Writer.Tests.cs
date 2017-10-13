using System;
using System.Collections.Generic;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Library;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.Library
{
	[TestFixture]
	public class RepositoryAndStorageMusicLibraryTests
	{
		[Test]
		public void ChangeDiscUri_ChangesDiscUriInStorageCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
			};

			Uri oldDiscUri = null;
			IMusicLibraryStorage musicLibraryStorageMock = Substitute.For<IMusicLibraryStorage>();
			musicLibraryStorageMock.ChangeDiscUri(Arg.Do<Disc>(arg => oldDiscUri = arg.Uri), Arg.Any<Uri>());
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), musicLibraryStorageMock, Substitute.For<ILibraryStructurer>());

			//	Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			//	Assert

			musicLibraryStorageMock.Received(1).ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative));
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative), oldDiscUri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesDiscUriPropertyCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
			};

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(), Substitute.For<ILibraryStructurer>());

			//	Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			//	Assert

			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), disc.Uri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesDiscInRepositoryCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
			};

			Uri updatedUri = null;
			IMusicLibraryRepository musicLibraryRepositoryMock = Substitute.For<IMusicLibraryRepository>();
			musicLibraryRepositoryMock.UpdateDisc(Arg.Do<Disc>(arg => updatedUri = arg.Uri));
			var target = new RepositoryAndStorageMusicLibrary(musicLibraryRepositoryMock, Substitute.For<IMusicLibraryStorage>(), Substitute.For<ILibraryStructurer>());

			//	Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			//	Assert

			musicLibraryRepositoryMock.Received(1).UpdateDisc(disc);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), updatedUri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesSongsUriPropertyCorrectly()
		{
			//	Arrange

			var song1 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song1", UriKind.Relative) };
			var song2 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song2", UriKind.Relative) };
			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				SongsUnordered = new List<Song> { song1, song2, },
			};

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song1.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative));
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song2.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative));
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(), libraryStructurerStub);

			//	Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			//	Assert

			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative), song1.Uri);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative), song2.Uri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesSongsInRepositoryCorrectly()
		{
			//	Arrange

			var song1 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song1", UriKind.Relative) };
			var song2 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song2", UriKind.Relative) };
			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				SongsUnordered = new List<Song> { song1, song2, },
			};

			List<Uri> updatedUris = new List<Uri>();
			IMusicLibraryRepository musicLibraryRepositoryMock = Substitute.For<IMusicLibraryRepository>();
			musicLibraryRepositoryMock.UpdateSong(Arg.Do<Song>(arg => updatedUris.Add(arg.Uri)));

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song1.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative));
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song2.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative));
			var target = new RepositoryAndStorageMusicLibrary(musicLibraryRepositoryMock, Substitute.For<IMusicLibraryStorage>(), libraryStructurerStub);

			//	Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			//	Assert

			musicLibraryRepositoryMock.Received(1).UpdateSong(song1);
			musicLibraryRepositoryMock.Received(1).UpdateSong(song2);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative), updatedUris[0]);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative), updatedUris[1]);
		}

		[Test]
		public void UpdateDisc_IfSomeTaggedPropertiesAreUpdated_UpdatesSongTagData()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var disc = new Disc
			{
				SongsUnordered = new List<Song> {song1, song2},
			};

			IMusicLibraryStorage musicLibraryStorageMock = Substitute.For<IMusicLibraryStorage>();
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), musicLibraryStorageMock, Substitute.For<ILibraryStructurer>());

			//	Act

			target.UpdateDisc(disc, UpdatedSongProperties.Album).Wait();

			//	Assert

			musicLibraryStorageMock.Received(1).UpdateSongTagData(song1, UpdatedSongProperties.Album);
			musicLibraryStorageMock.Received(1).UpdateSongTagData(song2, UpdatedSongProperties.Album);
		}

		[Test]
		public void UpdateDisc_IfNoTaggedPropertiesAreUpdated_DoesNotUpdateSongTagData()
		{
			//	Arrange

			IMusicLibraryStorage musicLibraryStorageMock = Substitute.For<IMusicLibraryStorage>();
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), musicLibraryStorageMock, Substitute.For<ILibraryStructurer>());

			//	Act

			target.UpdateDisc(new Disc { SongsUnordered = new List<Song> { new Song() } }, UpdatedSongProperties.None).Wait();

			//	Assert

			musicLibraryStorageMock.DidNotReceive().UpdateSongTagData(Arg.Any<Song>(), Arg.Any<UpdatedSongProperties>());
		}

		[Test]
		public void UpdateDisc_UpdatesDiscInRepositoryCorrectly()
		{
			//	Arrange

			var disc = new Disc();

			IMusicLibraryRepository musicLibraryRepositoryMock = Substitute.For<IMusicLibraryRepository>();
			var target = new RepositoryAndStorageMusicLibrary(musicLibraryRepositoryMock, Substitute.For<IMusicLibraryStorage>(), Substitute.For<ILibraryStructurer>());

			//	Act

			target.UpdateDisc(disc, UpdatedSongProperties.None).Wait();

			//	Assert

			musicLibraryRepositoryMock.Received(1).UpdateDisc(disc);
		}
	}
}
