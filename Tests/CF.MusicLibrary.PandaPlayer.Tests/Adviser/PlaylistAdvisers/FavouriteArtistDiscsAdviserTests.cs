using System;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.Adviser.PlaylistAdvisers
{
	[TestFixture]
	public class FavouriteArtistDiscsAdviserTests
	{
		[Test]
		public void Constructor_IfDiscAdviserArgumentIsNull_ThrowsArgumentIsNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new FavouriteArtistDiscsAdviser(null));
		}

		[Test]
		public void AdviseDiscs_AdvisesOnlyDiscsOfFavouriteArtists()
		{
			//	Arrange

			var favouriteArtist1 = new Artist { IsFavourite = true };
			var favouriteArtist2 = new Artist { IsFavourite = true };
			var usualArtist = new Artist { IsFavourite = false };

			var favouriteDisc1 = new Disc { SongsUnordered = new[] { new Song { Artist = favouriteArtist1 } } };
			var favouriteDisc2 = new Disc { SongsUnordered = new[] { new Song { Artist = favouriteArtist1 } } };
			var favouriteDisc3 = new Disc { SongsUnordered = new[] { new Song { Artist = favouriteArtist2 } } };

			var favouriteDiscAdvise1 = AdvisedPlaylist.ForDisc(favouriteDisc1);
			var favouriteDiscAdvise2 = AdvisedPlaylist.ForDisc(favouriteDisc2);
			var favouriteDiscAdvise3 = AdvisedPlaylist.ForDisc(favouriteDisc3);
			var usualDiscAdvise = AdvisedPlaylist.ForDisc(new Disc { SongsUnordered = new[] { new Song { Artist = usualArtist } } });

			var library = new DiscLibrary();

			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(new[] { favouriteDiscAdvise1, favouriteDiscAdvise2, favouriteDiscAdvise3, usualDiscAdvise });

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub);

			//	Act

			var advisedDiscs = target.Advise(library);

			//	Assert

			CollectionAssert.AreEqual(new[] { favouriteDisc1, favouriteDisc2, favouriteDisc3 }, advisedDiscs.Select(a => a.Disc));
		}

		[Test]
		public void AdviseDiscs_ConvertsFromDiscAdviseToFavouriteArtistDiscAdviseCorrectly()
		{
			//	Arrange

			var library = new DiscLibrary();

			var disc = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { IsFavourite = true } } } };
			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(new[] { AdvisedPlaylist.ForDisc(disc) });

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub);

			//	Act

			var advisedDiscs = target.Advise(library);

			//	Assert

			var advise = advisedDiscs.Single();
			Assert.AreEqual(AdvisedPlaylistType.FavouriteArtistDisc, advise.AdvisedPlaylistType);
			Assert.AreSame(disc, advise.Disc);
		}
	}
}
