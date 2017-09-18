using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using CF.MusicLibrary.Local;
using NSubstitute;
using NUnit.Framework;
using static System.FormattableString;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	[TestFixture]
	public class EditDiscsDetailsViewModelTests
	{
		private const string TestWorkshopMusicStorage = @"d:\music.test";

		[Test]
		public void SetDiscs_ForAddedArtistDisc_FillsDiscDataCorrectly()
		{
			//	Arrange

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] {})
			{
				Year = 2000,
				Title = "Some Disc",
				SourcePath = Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Disc"),
				PathWithinStorage = @"Some Artist\2000 - Some Disc",
				NameInStorage = "2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			//	Assert

			var discItem = target.Discs.Single() as ArtistDiscViewItem;
			Assert.IsNotNull(discItem);
			Assert.AreEqual(@"Some Artist\2000 - Some Disc", discItem.PathWithinWorkshopStorage);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Disc"), discItem.SourcePath);
			Assert.AreEqual("Some Artist", discItem.Artist.Name);
			Assert.AreEqual("Some Disc", discItem.Title);
			Assert.AreEqual(2000, discItem.Year);
			Assert.IsNull(discItem.Genre);
			Assert.AreEqual("2000 - Some Disc", discItem.NameInStorage);
			Assert.IsNull(discItem.DestinationUri);
		}

		[Test]
		public void SetDiscs_ForArtistDiscOfKnownArtist_SetsDestinationUriCorrectly()
		{
			//	Arrange

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				NameInStorage = "2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
			};

			ILibraryStructurer libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.GetArtistStorageUri(Arg.Any<DiscLibrary>(), Arg.Any<Artist>()).Returns(new Uri("/Some Artist", UriKind.Relative));
			libraryStructurer.BuildArtistDiscUri(new Uri("/Some Artist", UriKind.Relative), "2000 - Some Disc").Returns(new Uri("/SomeCategory/Some Artist/2000 - Some Disc", UriKind.Relative));

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, libraryStructurer, Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			//	Assert

			var discItem = target.Discs.Single();
			Assert.AreEqual(new Uri("/SomeCategory/Some Artist/2000 - Some Disc", UriKind.Relative), discItem.DestinationUri);
		}

		[Test]
		public void SetDiscs_ForArtistDiscOfKnownArtist_CopiesGenreFromLastArtistDisc()
		{
			//	Arrange

			var artist = new Artist
			{
				Name = "Some Artist",
			};

			var genre1 = new Genre { Name = "Gothic Metal" };
			var genre2 = new Genre { Name = "Symphonic Metal" };

			var disc1 = new Disc
			{
				SongsUnordered = new Collection<Song>
				{
					new Song
					{
						Year = 2005,
						Artist = artist,
						Genre = genre1,
					}
				}
			};

			var disc2 = new Disc
			{
				SongsUnordered = new Collection<Song>
				{
					new Song
					{
						Year = 2000,
						Artist = artist,
						Genre = genre2,
					}
				}
			};

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(new[] { disc1, disc2 }.Select(d => d)));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			//	Assert

			var discItem = target.Discs.Single();
			Assert.AreSame(genre1, discItem.Genre);
		}

		[Test]
		public void SetDiscs_ForAddedCompilationDiscWithArtistInfo_FillsDiscDataCorrectly()
		{
			//	Arrange

			var addedSongs = new[]
			{
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie\01 - Marilyn Manson - Rock Is Dead.mp3")),
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie\02 - Propellerheads - Spybreak! (Short One).mp3")),
			};

			var addedDisc = new AddedDiscInfo(addedSongs)
			{
				Title = "Some Movie",
				SourcePath = Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"),
				PathWithinStorage = @"Soundtracks\Some Movie",
				NameInStorage = "Some Movie",
				DiscType = DsicType.CompilationDiscWithArtistInfo,
			};

			ILibraryStructurer libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.BuildUriForWorkshopStoragePath(@"Soundtracks\Some Movie").Returns(new Uri("/Soundtracks/Some Movie", UriKind.Relative));

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, libraryStructurer, Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			//	Assert

			var discItem = target.Discs.Single() as CompilationDiscWithArtistInfoViewItem;
			Assert.IsNotNull(discItem);
			Assert.AreEqual(@"Soundtracks\Some Movie", discItem.PathWithinWorkshopStorage);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"), discItem.SourcePath);
			Assert.IsNull(discItem.Artist);
			Assert.AreEqual("Some Movie", discItem.Title);
			Assert.IsNull(discItem.Year);
			Assert.IsNull(discItem.Genre);
			Assert.AreEqual(new Uri("/Soundtracks/Some Movie", UriKind.Relative), discItem.DestinationUri);
		}

		[Test]
		public void SetDiscs_ForAddedCompilationDiscWithoutArtistInfo_FillsDiscDataCorrectly()
		{
			//	Arrange

			var addedSongs = new[]
			{
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie\01 - Half Remembered Dream.mp")),
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie\02 - We Built Our Own World.mp3")),
			};

			var addedDisc = new AddedDiscInfo(addedSongs)
			{
				Title = "Some Movie",
				SourcePath = Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"),
				PathWithinStorage = @"Soundtracks\Some Movie",
				NameInStorage = "Some Movie",
				DiscType = DsicType.CompilationDiscWithoutArtistInfo,
			};

			ILibraryStructurer libraryStructurer = Substitute.For<ILibraryStructurer>();
			libraryStructurer.BuildUriForWorkshopStoragePath(@"Soundtracks\Some Movie").Returns(new Uri("/Soundtracks/Some Movie", UriKind.Relative));

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, libraryStructurer, Substitute.For<IFileSystemFacade>());

			//	Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			//	Assert

			var discItem = target.Discs.Single() as CompilationDiscWithoutArtistInfoViewItem;
			Assert.IsNotNull(discItem);
			Assert.AreEqual(@"Soundtracks\Some Movie", discItem.PathWithinWorkshopStorage);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie"), discItem.SourcePath);
			Assert.IsNull(discItem.Artist);
			Assert.AreEqual("Some Movie", discItem.Title);
			Assert.IsNull(discItem.Year);
			Assert.IsNull(discItem.Genre);
			Assert.AreEqual(new Uri("/Soundtracks/Some Movie", UriKind.Relative), discItem.DestinationUri);
		}

		[Test]
		public void Songs_ReturnsCorrectSongsData()
		{
			//	Arrange

			AddedDiscInfo[] discs =
			{
				new AddedDiscInfo(new[]
				{
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Nightwish\2000 - Wishmaster\01 - She Is My Sin.mp3"))
					{
						Track = 1,
						Title = "She Is My Sin",
						FullTitle = "She Is My Sin",
					}
				})
				{
					Year = 2000,
					Title = "Wishmaster",
					DiscType = DsicType.ArtistDisc,
					Artist = "Nightwish",
				},

				new AddedDiscInfo(new[]
				{
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Gladiator\01 - Progeny.mp3"))
					{
						Track = 1,
						Title = "Progeny",
						FullTitle = "Progeny",
					}
				})
				{
					Title = "Gladiator",
					PathWithinStorage = @"Soundtracks\Gladiator",
					DiscType = DsicType.CompilationDiscWithoutArtistInfo,
				},

				new AddedDiscInfo(new[]
				{
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix\01 - Marilyn Manson - Rock Is Dead.mp3"))
					{
						Artist = "Marilyn Manson",
						Track = 1,
						Title = "Rock Is Dead",
						FullTitle = "Marilyn Manson - Rock Is Dead",
					},
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix\02 - Propellerheads - Spybreak! (Short One).mp3"))
					{
						Artist = "Propellerheads",
						Track = 2,
						Title = "Spybreak! (Short One)",
						FullTitle = "Propellerheads - Spybreak! (Short One)",
					}
				})
				{
					Title = "The Matrix",
					PathWithinStorage = @"Soundtracks\The Matrix",
					DiscType = DsicType.CompilationDiscWithArtistInfo,
				},
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, new MyLibraryStructurer(), Substitute.For<IFileSystemFacade>());

			target.SetDiscs(discs).Wait();

			//	Emulating editing of disc data by the user.
			target.Discs[0].DestinationUri = new Uri("/Foreign/Nightwish/2000 - Wishmaster", UriKind.Relative);
			target.Discs[0].Genre = new Genre { Name = "Gothic Metal" };
			target.Discs[1].Genre = new Genre { Name = "Soundtrack" };
			target.Discs[1].Artist = new Artist { Name = "Hans Zimmer" };
			target.Discs[1].Year = 2000;
			target.Discs[2].Genre = new Genre { Name = "Soundtrack" };

			//	Act

			var songs = target.AddedSongs.ToList();

			//	Assert

			//	Sanity check
			Assert.IsTrue(target.DataIsReady);

			Assert.AreEqual(4, songs.Count);

			var song1 = songs[0].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Nightwish\2000 - Wishmaster\01 - She Is My Sin.mp3"), songs[0].SourceFileName);
			Assert.AreEqual(new Uri("/Foreign/Nightwish/2000 - Wishmaster/01 - She Is My Sin.mp3", UriKind.Relative), song1.Uri);
			Assert.AreEqual("Nightwish", song1.Artist.Name);
			Assert.AreEqual("Wishmaster", song1.Disc.Title);
			Assert.AreEqual(2000, song1.Year);
			Assert.AreEqual("Gothic Metal", song1.Genre.Name);
			Assert.AreEqual(1, song1.TrackNumber);
			Assert.AreEqual("She Is My Sin", song1.Title);

			var song2 = songs[1].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Gladiator\01 - Progeny.mp3"), songs[1].SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/Gladiator/01 - Progeny.mp3", UriKind.Relative), song2.Uri);
			Assert.AreEqual("Hans Zimmer", song2.Artist.Name);
			Assert.AreEqual("Gladiator", song2.Disc.Title);
			Assert.AreEqual(2000, song2.Year);
			Assert.AreEqual("Soundtrack", song2.Genre.Name);
			Assert.AreEqual(1, song2.TrackNumber);
			Assert.AreEqual("Progeny", song2.Title);

			var song3 = songs[2].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix\01 - Marilyn Manson - Rock Is Dead.mp3"), songs[2].SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/The Matrix/01 - Marilyn Manson - Rock Is Dead.mp3", UriKind.Relative), song3.Uri);
			Assert.AreEqual("Marilyn Manson", song3.Artist.Name);
			Assert.AreEqual("The Matrix", song3.Disc.Title);
			Assert.IsNull(song3.Year);
			Assert.AreEqual("Soundtrack", song3.Genre.Name);
			Assert.AreEqual(1, song3.TrackNumber);
			Assert.AreEqual("Rock Is Dead", song3.Title);

			var song4 = songs[3].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix\02 - Propellerheads - Spybreak! (Short One).mp3"), songs[3].SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/The Matrix/02 - Propellerheads - Spybreak! (Short One).mp3", UriKind.Relative), song4.Uri);
			Assert.AreEqual("Propellerheads", song4.Artist.Name);
			Assert.AreEqual("The Matrix", song4.Disc.Title);
			Assert.IsNull(song4.Year);
			Assert.AreEqual("Soundtrack", song4.Genre.Name);
			Assert.AreEqual(2, song4.TrackNumber);
			Assert.AreEqual("Spybreak! (Short One)", song4.Title);
		}
	}
}
