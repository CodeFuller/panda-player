using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using CF.MusicLibrary.Library;
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
			// Arrange

			var addedDisc = new AddedDiscInfo(Array.Empty<AddedSongInfo>())
			{
				Year = 2000,
				DiscTitle = "Some Album (CD 1)",
				AlbumTitle = "Some Album",
				SourcePath = Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Album (CD 1)"),
				UriWithinStorage = new Uri("/Foreign/Some Artist/2000 - Some Album (CD 1)", UriKind.Relative),
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>());

			// Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			// Assert

			var discItem = target.Discs.Single() as ArtistDiscViewItem;
			Assert.IsNotNull(discItem);
			Assert.AreEqual(new Uri("/Foreign/Some Artist/2000 - Some Album (CD 1)", UriKind.Relative), discItem.DestinationUri);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Album (CD 1)"), discItem.SourcePath);
			Assert.AreEqual("Some Artist", discItem.Artist.Name);
			Assert.AreEqual("Some Album (CD 1)", discItem.DiscTitle);
			Assert.AreEqual("Some Album", discItem.AlbumTitle);
			Assert.AreEqual(2000, discItem.Year);
			Assert.IsNull(discItem.Genre);
		}

		[Test]
		public void SetDiscs_ForArtistDiscOfKnownArtist_CopiesGenreFromLastArtistDisc()
		{
			// Arrange

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
					},
				},
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
					},
				},
			};

			var addedDisc = new AddedDiscInfo(Array.Empty<AddedSongInfo>())
			{
				DiscTitle = "Some Title",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
				SourcePath = "DiscSourcePath",
				UriWithinStorage = new Uri("/Some/Disc/Uri", UriKind.Relative),
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(new[] { disc1, disc2 }.Select(d => d)));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>());

			// Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			// Assert

			var discItem = target.Discs.Single();
			Assert.AreSame(genre1, discItem.Genre);
		}

		[Test]
		public void SetDiscs_ForAddedCompilationDiscWithArtistInfo_FillsDiscDataCorrectly()
		{
			// Arrange

			var addedSongs = new[]
			{
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)\01 - Marilyn Manson - Rock Is Dead.mp3")),
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)\02 - Propellerheads - Spybreak! (Short One).mp3")),
			};

			var addedDisc = new AddedDiscInfo(addedSongs)
			{
				DiscTitle = "Some Movie (CD 1)",
				AlbumTitle = "Some Movie",
				SourcePath = Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)"),
				UriWithinStorage = new Uri("/Soundtracks/Some Movie (CD 1)", UriKind.Relative),
				DiscType = DsicType.CompilationDiscWithArtistInfo,
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>());

			// Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			// Assert

			var discItem = target.Discs.Single() as CompilationDiscWithArtistInfoViewItem;
			Assert.IsNotNull(discItem);
			Assert.AreEqual(new Uri("/Soundtracks/Some Movie (CD 1)", UriKind.Relative), discItem.DestinationUri);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)"), discItem.SourcePath);
			Assert.IsNull(discItem.Artist);
			Assert.AreEqual("Some Movie (CD 1)", discItem.DiscTitle);
			Assert.AreEqual("Some Movie", discItem.AlbumTitle);
			Assert.IsNull(discItem.Year);
			Assert.IsNull(discItem.Genre);
		}

		[Test]
		public void SetDiscs_ForAddedCompilationDiscWithoutArtistInfo_FillsDiscDataCorrectly()
		{
			// Arrange

			var addedSongs = new[]
			{
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)\01 - Half Remembered Dream.mp")),
				new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)\02 - We Built Our Own World.mp3")),
			};

			var addedDisc = new AddedDiscInfo(addedSongs)
			{
				DiscTitle = "Some Movie (CD 1)",
				AlbumTitle = "Some Movie",
				SourcePath = Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)"),
				UriWithinStorage = new Uri("/Soundtracks/Some Movie (CD 1)", UriKind.Relative),
				DiscType = DsicType.CompilationDiscWithoutArtistInfo,
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>());

			// Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			// Assert

			var discItem = target.Discs.Single() as CompilationDiscWithoutArtistInfoViewItem;
			Assert.IsNotNull(discItem);
			Assert.AreEqual(new Uri("/Soundtracks/Some Movie (CD 1)", UriKind.Relative), discItem.DestinationUri);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Some Movie (CD 1)"), discItem.SourcePath);
			Assert.IsNull(discItem.Artist);
			Assert.AreEqual("Some Movie (CD 1)", discItem.DiscTitle);
			Assert.AreEqual("Some Movie", discItem.AlbumTitle);
			Assert.IsNull(discItem.Year);
			Assert.IsNull(discItem.Genre);
		}

		[Test]
		public void SetDiscs_ForExistingDisc_FillsDiscDataCorrectly()
		{
			// Arrange

			var existingArtist = new Artist();
			var existingGenre = new Genre();
			var existingDisc = new Disc
			{
				Title = "Existing Disc Title",
				AlbumTitle = "Existing Album Title",
				Uri = new Uri("/Foreign/Some Artist/2000 - Some Disc", UriKind.Relative),
				SongsUnordered = new[]
				{
					new Song
					{
						Year = 1995,
						Artist = existingArtist,
						Genre = existingGenre,
					},
				},
			};

			var addedDisc = new AddedDiscInfo(Array.Empty<AddedSongInfo>())
			{
				Year = 2000,
				DiscTitle = "Some Disc",
				SourcePath = Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Disc"),
				UriWithinStorage = new Uri("/Foreign/Some Artist/2000 - Some Disc", UriKind.Relative),
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(new[] { existingDisc }.AsEnumerable()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>());

			// Act

			target.SetDiscs(Enumerable.Repeat(addedDisc, 1)).Wait();

			// Assert

			var discItem = target.Discs.Single() as ExistingDiscViewItem;
			Assert.IsNotNull(discItem);
			Assert.AreEqual(new Uri("/Foreign/Some Artist/2000 - Some Disc", UriKind.Relative), discItem.DestinationUri);
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Some Artist\2000 - Some Disc"), discItem.SourcePath);
			Assert.AreSame(existingArtist, discItem.Artist);
			Assert.AreEqual("Existing Disc Title", discItem.DiscTitle);
			Assert.AreEqual("Existing Album Title", discItem.AlbumTitle);
			Assert.AreEqual(1995, discItem.Year);
			Assert.AreEqual(existingGenre, discItem.Genre);
		}

		[Test]
		public void Songs_ReturnsCorrectSongsData()
		{
			// Arrange

			var existingArtist = new Artist();
			var existingGenre = new Genre();
			var existingDisc = new Disc
			{
				Title = "Proud Like A God (CD 1)",
				AlbumTitle = "Proud Like A God",
				Uri = new Uri("/Foreign/Guano Apes/1997 - Proud Like A God (CD 1)", UriKind.Relative),
				SongsUnordered = new[]
				{
					new Song
					{
						Year = 1997,
						Artist = existingArtist,
						Genre = existingGenre,
					},
				},
			};

			AddedDiscInfo[] discs =
			{
				new AddedDiscInfo(new[]
				{
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Foreign\Nightwish\2000 - Wishmaster (CD 1)\01 - She Is My Sin.mp3"))
					{
						Track = 1,
						Title = "She Is My Sin",
						FullTitle = "She Is My Sin",
					},
				})
				{
					Year = 2000,
					DiscTitle = "Wishmaster (CD 1)",
					AlbumTitle = "Wishmaster",
					UriWithinStorage = new Uri("/Foreign/Nightwish/2000 - Wishmaster (CD 1)", UriKind.Relative),
					DiscType = DsicType.ArtistDisc,
					Artist = "Nightwish",
					SourcePath = "DiscSourcePath1",
				},

				new AddedDiscInfo(new[]
				{
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Gladiator (CD 1)\01 - Progeny.mp3"))
					{
						Track = 1,
						Title = "Progeny",
						FullTitle = "Progeny",
					},
				})
				{
					DiscTitle = "Gladiator (CD 1)",
					AlbumTitle = "Gladiator",
					UriWithinStorage = new Uri("/Soundtracks/Gladiator (CD 1)", UriKind.Relative),
					DiscType = DsicType.CompilationDiscWithoutArtistInfo,
					SourcePath = "DiscSourcePath2",
				},

				new AddedDiscInfo(new[]
				{
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix (CD 1)\01 - Marilyn Manson - Rock Is Dead.mp3"))
					{
						Artist = "Marilyn Manson",
						Track = 1,
						Title = "Rock Is Dead",
						FullTitle = "Marilyn Manson - Rock Is Dead",
					},
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix (CD 1)\02 - Propellerheads - Spybreak! (Short One).mp3"))
					{
						Artist = "Propellerheads",
						Track = 2,
						Title = "Spybreak! (Short One)",
						FullTitle = "Propellerheads - Spybreak! (Short One)",
					},
				})
				{
					DiscTitle = "The Matrix (CD 1)",
					AlbumTitle = "The Matrix",
					UriWithinStorage = new Uri("/Soundtracks/The Matrix (CD 1)", UriKind.Relative),
					DiscType = DsicType.CompilationDiscWithArtistInfo,
					SourcePath = "DiscSourcePath3",
				},

				new AddedDiscInfo(new[]
				{
					new AddedSongInfo(Invariant($@"{TestWorkshopMusicStorage}\Foreign\Guano Apes\1997 - Proud Like A God (CD 1)\14 - Cool Song.mp3"))
					{
						Track = 14,
						Title = "Cool Song",
						FullTitle = "Cool Song",
					},
				})
				{
					Year = 1997,
					DiscTitle = "Proud Like A God (CD 1)",
					AlbumTitle = "Proud Like A God",
					UriWithinStorage = new Uri("/Foreign/Guano Apes/1997 - Proud Like A God (CD 1)", UriKind.Relative),
					DiscType = DsicType.ArtistDisc,
					Artist = "Guano Apes",
					SourcePath = "DiscSourcePath4",
				},
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(new[] { existingDisc }.AsEnumerable()));
			var target = new EditDiscsDetailsViewModel(discLibrary, new LibraryStructurer());

			target.SetDiscs(discs).Wait();

			// Emulating editing of disc data by the user.
			target.Discs[0].Genre = new Genre { Name = "Gothic Metal" };
			target.Discs[1].Genre = new Genre { Name = "Soundtrack" };
			target.Discs[1].Artist = new Artist { Name = "Hans Zimmer" };
			target.Discs[1].Year = 2000;
			target.Discs[2].Genre = new Genre { Name = "Soundtrack" };

			// Act

			var songs = target.AddedSongs.ToList();

			// Assert

			// Sanity check
			Assert.IsTrue(target.DataIsReady);

			Assert.AreEqual(5, songs.Count);

			var song1 = songs[0].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Foreign\Nightwish\2000 - Wishmaster (CD 1)\01 - She Is My Sin.mp3"), songs[0].SourceFileName);
			Assert.AreEqual(new Uri("/Foreign/Nightwish/2000 - Wishmaster (CD 1)/01 - She Is My Sin.mp3", UriKind.Relative), song1.Uri);
			Assert.AreEqual("Nightwish", song1.Artist.Name);
			Assert.AreEqual("Wishmaster (CD 1)", song1.Disc.Title);
			Assert.AreEqual("Wishmaster", song1.Disc.AlbumTitle);
			Assert.AreEqual(2000, song1.Year);
			Assert.AreEqual("Gothic Metal", song1.Genre.Name);
			Assert.AreEqual(1, song1.TrackNumber);
			Assert.AreEqual("She Is My Sin", song1.Title);

			var song2 = songs[1].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\Gladiator (CD 1)\01 - Progeny.mp3"), songs[1].SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/Gladiator (CD 1)/01 - Progeny.mp3", UriKind.Relative), song2.Uri);
			Assert.AreEqual("Hans Zimmer", song2.Artist.Name);
			Assert.AreEqual("Gladiator (CD 1)", song2.Disc.Title);
			Assert.AreEqual("Gladiator", song2.Disc.AlbumTitle);
			Assert.AreEqual(2000, song2.Year);
			Assert.AreEqual("Soundtrack", song2.Genre.Name);
			Assert.AreEqual(1, song2.TrackNumber);
			Assert.AreEqual("Progeny", song2.Title);

			var song3 = songs[2].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix (CD 1)\01 - Marilyn Manson - Rock Is Dead.mp3"), songs[2].SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/The Matrix (CD 1)/01 - Marilyn Manson - Rock Is Dead.mp3", UriKind.Relative), song3.Uri);
			Assert.AreEqual("Marilyn Manson", song3.Artist.Name);
			Assert.AreEqual("The Matrix (CD 1)", song3.Disc.Title);
			Assert.AreEqual("The Matrix", song3.Disc.AlbumTitle);
			Assert.IsNull(song3.Year);
			Assert.AreEqual("Soundtrack", song3.Genre.Name);
			Assert.AreEqual(1, song3.TrackNumber);
			Assert.AreEqual("Rock Is Dead", song3.Title);

			var song4 = songs[3].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Soundtracks\The Matrix (CD 1)\02 - Propellerheads - Spybreak! (Short One).mp3"), songs[3].SourceFileName);
			Assert.AreEqual(new Uri("/Soundtracks/The Matrix (CD 1)/02 - Propellerheads - Spybreak! (Short One).mp3", UriKind.Relative), song4.Uri);
			Assert.AreEqual("Propellerheads", song4.Artist.Name);
			Assert.AreEqual("The Matrix (CD 1)", song4.Disc.Title);
			Assert.AreEqual("The Matrix", song4.Disc.AlbumTitle);
			Assert.IsNull(song4.Year);
			Assert.AreEqual("Soundtrack", song4.Genre.Name);
			Assert.AreEqual(2, song4.TrackNumber);
			Assert.AreEqual("Spybreak! (Short One)", song4.Title);

			var song5 = songs[4].Song;
			Assert.AreEqual(Invariant($@"{TestWorkshopMusicStorage}\Foreign\Guano Apes\1997 - Proud Like A God (CD 1)\14 - Cool Song.mp3"), songs[4].SourceFileName);
			Assert.AreEqual(new Uri("/Foreign/Guano Apes/1997 - Proud Like A God (CD 1)/14 - Cool Song.mp3", UriKind.Relative), song5.Uri);
			Assert.AreSame(existingArtist, song5.Artist);
			Assert.AreEqual("Proud Like A God (CD 1)", song5.Disc.Title);
			Assert.AreEqual("Proud Like A God", song5.Disc.AlbumTitle);
			Assert.AreEqual(1997, song5.Year);
			Assert.AreEqual(existingGenre, song5.Genre);
			Assert.AreEqual(14, song5.TrackNumber);
			Assert.AreEqual("Cool Song", song5.Title);
		}
	}
}
