﻿using System.Linq;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.Adviser;
using MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using MusicLibrary.Tests;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.PandaPlayer.Tests.Adviser.PlaylistAdvisers
{
	[TestFixture]
	public class CompositePlaylistAdviserTests
	{
		[Test]
		public void Advise_IfHighlyRatedSongsAdvisesProvided_AdvisesHighlyRatedSongsPlaylistsEveryTenthTime()
		{
			// Arrange

			var library = new DiscLibrary();

			var highlyRatedSongsAdvise1 = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });
			var highlyRatedSongsAdvise2 = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });

			IPlaylistAdviser usualDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			usualDiscsAdviserStub.Advise(library).Returns(Enumerable.Range(0, 20).Select(i => AdvisedPlaylist.ForDisc(new Disc())));

			IPlaylistAdviser highlyRatedSongsAdviserStub = Substitute.For<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Advise(library).Returns(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			var settings = new AdviserSettings
			{
				HighlyRatedSongsAdviser = new HighlyRatedSongsAdviserSettings { PlaybacksBetweenHighlyRatedSongs = 10 },
			};

			var target = new CompositePlaylistAdviser(usualDiscsAdviserStub, highlyRatedSongsAdviserStub, Substitute.For<IPlaylistAdviser>(),
				Substitute.For<IGenericDataRepository<PlaylistAdviserMemo>>(), settings.StubOptions());

			// Act

			var advises = target.Advise(library).ToList();

			// Assert

			Assert.AreEqual(AdvisedPlaylistType.HighlyRatedSongs, advises[0].AdvisedPlaylistType);
			Assert.AreEqual(AdvisedPlaylistType.HighlyRatedSongs, advises[10].AdvisedPlaylistType);
		}

		[Test]
		public void Advise_IfMemoIsLoaded_ConsidersPreviousPlaybacksSinceHighlyRatedSongs()
		{
			// Arrange

			var library = new DiscLibrary();

			var highlyRatedSongsAdvise = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });

			IPlaylistAdviser usualDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			usualDiscsAdviserStub.Advise(library).Returns(Enumerable.Range(0, 20).Select(i => AdvisedPlaylist.ForDisc(new Disc())));

			IPlaylistAdviser highlyRatedSongsAdviserStub = Substitute.For<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Advise(library).Returns(new[] { highlyRatedSongsAdvise });

			var memoRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistAdviserMemo>>();
			memoRepositoryStub.Load().Returns(new PlaylistAdviserMemo
			{
				PlaybacksSinceHighlyRatedSongsPlaylist = 2,
			});

			var settings = new AdviserSettings
			{
				HighlyRatedSongsAdviser = new HighlyRatedSongsAdviserSettings { PlaybacksBetweenHighlyRatedSongs = 10 },
			};

			var target = new CompositePlaylistAdviser(usualDiscsAdviserStub, highlyRatedSongsAdviserStub,
				Substitute.For<IPlaylistAdviser>(), memoRepositoryStub, settings.StubOptions());

			// Act

			var advises = target.Advise(library).ToList();

			// Assert

			Assert.AreEqual(AdvisedPlaylistType.HighlyRatedSongs, advises[7].AdvisedPlaylistType);
		}

		[Test]
		public void Advise_IfFavouriteArtistDiscsAdvisesProvided_AdvisesFavouriteArtistDiscsInConfiguredIntervals()
		{
			// Arrange

			var library = new DiscLibrary();

			var favouriteArtistDiscAdvise1 = AdvisedPlaylist.ForFavouriteArtistDisc(new Disc());
			var favouriteArtistDiscAdvise2 = AdvisedPlaylist.ForFavouriteArtistDisc(new Disc());

			IPlaylistAdviser usualDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			usualDiscsAdviserStub.Advise(library).Returns(Enumerable.Range(0, 20).Select(i => AdvisedPlaylist.ForDisc(new Disc())));

			IPlaylistAdviser favouriteArtistDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			favouriteArtistDiscsAdviserStub.Advise(library).Returns(new[] { favouriteArtistDiscAdvise1, favouriteArtistDiscAdvise2 });

			var settings = new AdviserSettings
			{
				FavouriteArtistsAdviser = new FavouriteArtistsAdviserSettings { PlaybacksBetweenFavouriteArtistDiscs = 5 },
			};

			var target = new CompositePlaylistAdviser(usualDiscsAdviserStub, Substitute.For<IPlaylistAdviser>(), favouriteArtistDiscsAdviserStub,
				Substitute.For<IGenericDataRepository<PlaylistAdviserMemo>>(), settings.StubOptions());

			// Act

			var advises = target.Advise(library).ToList();

			// Assert

			Assert.AreSame(favouriteArtistDiscAdvise1, advises[0]);
			Assert.AreSame(favouriteArtistDiscAdvise2, advises[5]);
		}

		[Test]
		public void Advise_IfMemoIsLoaded_ConsidersPreviousPlaybacksSinceFavouriteArtistDisc()
		{
			// Arrange

			var library = new DiscLibrary();

			var favouriteArtistDiscAdvise = AdvisedPlaylist.ForFavouriteArtistDisc(new Disc());

			IPlaylistAdviser usualDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			usualDiscsAdviserStub.Advise(library).Returns(Enumerable.Range(0, 20).Select(i => AdvisedPlaylist.ForDisc(new Disc())));

			IPlaylistAdviser favouriteArtistDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			favouriteArtistDiscsAdviserStub.Advise(library).Returns(new[] { favouriteArtistDiscAdvise });

			var settings = new AdviserSettings
			{
				FavouriteArtistsAdviser = new FavouriteArtistsAdviserSettings { PlaybacksBetweenFavouriteArtistDiscs = 6 },
			};

			var memoRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistAdviserMemo>>();
			memoRepositoryStub.Load().Returns(new PlaylistAdviserMemo
			{
				PlaybacksSinceFavouriteArtistDisc = 2,
			});

			var target = new CompositePlaylistAdviser(usualDiscsAdviserStub, Substitute.For<IPlaylistAdviser>(),
				favouriteArtistDiscsAdviserStub, memoRepositoryStub, settings.StubOptions());

			// Act

			var advises = target.Advise(library).ToList();

			// Assert

			Assert.AreSame(favouriteArtistDiscAdvise, advises[3]);
		}

		[Test]
		public void Advise_SkipsDuplicatedDiscsBetweenDiscAdviserAndFavouriteArtistDiscs()
		{
			// Arrange

			var library = new DiscLibrary();

			var disc1 = new Disc();
			var disc2 = new Disc();

			var favouriteArtistDiscAdvise1 = AdvisedPlaylist.ForFavouriteArtistDisc(disc1);
			var favouriteArtistDiscAdvise2 = AdvisedPlaylist.ForFavouriteArtistDisc(disc2);

			var usualDiscAdvises = Enumerable.Range(0, 10).Select(i => AdvisedPlaylist.ForDisc(new Disc()))
				.Concat(new[] { AdvisedPlaylist.ForDisc(disc1) })
				.Concat(Enumerable.Range(0, 10).Select(i => AdvisedPlaylist.ForDisc(new Disc())))
				.Concat(new[] { AdvisedPlaylist.ForDisc(disc2) });

			IPlaylistAdviser usualDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			usualDiscsAdviserStub.Advise(library).Returns(usualDiscAdvises);

			IPlaylistAdviser favouriteArtistDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			favouriteArtistDiscsAdviserStub.Advise(library).Returns(new[] { favouriteArtistDiscAdvise1, favouriteArtistDiscAdvise2 });

			var target = new CompositePlaylistAdviser(usualDiscsAdviserStub, Substitute.For<IPlaylistAdviser>(), favouriteArtistDiscsAdviserStub,
				Substitute.For<IGenericDataRepository<PlaylistAdviserMemo>>(), new AdviserSettings().StubOptions());

			// Act

			var advises = target.Advise(library).ToList();

			// Assert

			Assert.AreEqual(22, advises.Count);
			Assert.AreEqual(1, advises.Count(a => a.Disc == disc1));
			Assert.AreEqual(1, advises.Count(a => a.Disc == disc2));
		}

		[Test]
		public void Advise_MixesDifferentAdvisesTypesCorrectly()
		{
			// Arrange

			var library = new DiscLibrary();

			var highlyRatedSongsAdvise1 = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });
			var highlyRatedSongsAdvise2 = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });

			var favouriteArtistDiscAdvise1 = AdvisedPlaylist.ForFavouriteArtistDisc(new Disc());
			var favouriteArtistDiscAdvise2 = AdvisedPlaylist.ForFavouriteArtistDisc(new Disc());

			var usualDiscAdvises = Enumerable.Range(1, 10).Select(i => AdvisedPlaylist.ForDisc(new Disc())).ToList();

			IPlaylistAdviser usualDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			usualDiscsAdviserStub.Advise(library).Returns(usualDiscAdvises);

			IPlaylistAdviser highlyRatedSongsAdviserStub = Substitute.For<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Advise(library).Returns(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			IPlaylistAdviser favouriteArtistDiscsAdviserStub = Substitute.For<IPlaylistAdviser>();
			favouriteArtistDiscsAdviserStub.Advise(library).Returns(new[] { favouriteArtistDiscAdvise1, favouriteArtistDiscAdvise2 });

			var settings = new AdviserSettings
			{
				FavouriteArtistsAdviser = new FavouriteArtistsAdviserSettings { PlaybacksBetweenFavouriteArtistDiscs = 5 },
				HighlyRatedSongsAdviser = new HighlyRatedSongsAdviserSettings { PlaybacksBetweenHighlyRatedSongs = 10 },
			};

			var target = new CompositePlaylistAdviser(usualDiscsAdviserStub, highlyRatedSongsAdviserStub, favouriteArtistDiscsAdviserStub,
				Substitute.For<IGenericDataRepository<PlaylistAdviserMemo>>(), settings.StubOptions());

			// Act

			var advises = target.Advise(library).ToList();

			// Assert

			var expectedAdvises = new[]
			{
				highlyRatedSongsAdvise1,
				favouriteArtistDiscAdvise1,
				usualDiscAdvises[0], usualDiscAdvises[1], usualDiscAdvises[2], usualDiscAdvises[3],
				favouriteArtistDiscAdvise2,
				usualDiscAdvises[4], usualDiscAdvises[5], usualDiscAdvises[6],
				highlyRatedSongsAdvise2,
				usualDiscAdvises[7], usualDiscAdvises[8], usualDiscAdvises[9],
			};

			CollectionAssert.AreEqual(expectedAdvises, advises);
		}

		[Test]
		public void RegisterAdvicePlayback_SavesUpdatedMemoInRepository()
		{
			// Arrange

			PlaylistAdviserMemo savedMemo = null;
			IGenericDataRepository<PlaylistAdviserMemo> memoRepositoryMock = Substitute.For<IGenericDataRepository<PlaylistAdviserMemo>>();
			memoRepositoryMock.Save(Arg.Do<PlaylistAdviserMemo>(m => savedMemo = (PlaylistAdviserMemo)m.Clone()));

			memoRepositoryMock.Load().Returns(new PlaylistAdviserMemo
			{
				PlaybacksSinceHighlyRatedSongsPlaylist = 3,
				PlaybacksSinceFavouriteArtistDisc = 5,
			});

			var target = new CompositePlaylistAdviser(Substitute.For<IPlaylistAdviser>(), Substitute.For<IPlaylistAdviser>(),
				Substitute.For<IPlaylistAdviser>(), memoRepositoryMock, new AdviserSettings().StubOptions());

			// Act

			target.RegisterAdvicePlayback(AdvisedPlaylist.ForDisc(new Disc()));

			// Assert

			Assert.IsNotNull(savedMemo);
			Assert.AreEqual(4, savedMemo.PlaybacksSinceHighlyRatedSongsPlaylist);
			Assert.AreEqual(6, savedMemo.PlaybacksSinceFavouriteArtistDisc);
		}
	}
}