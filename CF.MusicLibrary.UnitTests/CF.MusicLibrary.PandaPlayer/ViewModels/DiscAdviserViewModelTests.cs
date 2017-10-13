using System;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
{
	[TestFixture]
	public class DiscAdviserViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void Constructor_IfDiscLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscAdviserViewModel(null, Substitute.For<ICompositePlaylistAdviser>()));
		}

		[Test]
		public void Constructor_IfPlaylistAdviserArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscAdviserViewModel(new DiscLibrary(), null));
		}

		[Test]
		public void Load_FillsAdvisedDiscs()
		{
			//	Arrange

			var advise = AdvisedPlaylist.ForDisc(new Disc());
			var library = new DiscLibrary();

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise });

			var target = new DiscAdviserViewModel(library, adviserStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Assert

			Assert.AreSame(advise, target.CurrentAdvise);
		}

		[Test]
		public void CurrentAdviseGetter_IfCurrentAdviseIsNotSet_ReturnsNull()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(), Substitute.For<ICompositePlaylistAdviser>());

			//	Act & Assert

			Assert.IsNull(target.CurrentAdvise);
		}

		[Test]
		public void CurrentAdviseAnnouncementGetter_IfCurrentAdviseIsNotSet_DoesNotThrow()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(), Substitute.For<ICompositePlaylistAdviser>());

			//	Act & Assert

			string currAnnouncement;
			Assert.DoesNotThrow(() => currAnnouncement =  target.CurrentAdviseAnnouncement);
		}

		[Test]
		public void CurrentAdviseAnnouncementGetter_IfCurrentAdviseIsSet_ReturnsAdviseTitle()
		{
			//	Arrange

			var advise = AdvisedPlaylist.ForDisc(new Disc { Title = "Some Disc" });
			var library = new DiscLibrary();

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act & Assert

			Assert.AreEqual(advise.Title, target.CurrentAdviseAnnouncement);
		}

		[Test]
		public void PlayCurrentAdviseCommand_IfNoAdvisesAreMade_ReturnsWithNoAction()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(), Substitute.For<ICompositePlaylistAdviser>());

			bool receivedEvent = false;
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => receivedEvent = true);

			//	Act

			target.PlayCurrentAdvise();

			//	Assert

			Assert.IsFalse(receivedEvent);
			//	Avoiding uncovered lambda code (receivedEvent = true)
			Messenger.Default.Send(new PlaySongsListEventArgs(Enumerable.Empty<Song>()));
		}

		[Test]
		public void PlayCurrentAdviseCommand_IfCurrentAdviseIsFilled_RegisterAdvicePlaybackAtAdviser()
		{
			//	Arrange

			var library = new DiscLibrary();
			var advise = AdvisedPlaylist.ForDisc(new Disc());

			ICompositePlaylistAdviser adviserMock = Substitute.For<ICompositePlaylistAdviser>();
			adviserMock.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise });

			var target = new DiscAdviserViewModel(library, adviserMock);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			target.PlayCurrentAdvise();

			//	Assert

			adviserMock.Received(1).RegisterAdvicePlayback(advise);
		}

		[Test]
		public void PlayCurrentAdviseCommand_IfCurrentAdviseIsFilled_SendsPlaySongsListEventForCurrentAdvise()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var advise = AdvisedPlaylist.ForHighlyRatedSongs(new[] { song1, song2 });
			var library = new DiscLibrary();

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			PlaySongsListEventArgs receivedEvent = null;
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.PlayCurrentAdvise();

			//	Assert

			Assert.IsNotNull(receivedEvent);
			CollectionAssert.AreEqual(advise.Songs, receivedEvent.Songs);
		}

		[Test]
		public void SwitchToNextAdviseCommand_IfCurrentAdviseIsNotLast_SwitchesToNextAdvise()
		{
			//	Arrange

			var advise1 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<Song>());
			var advise2 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise1, advise2 });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			target.SwitchToNextAdvise();

			//	Assert

			Assert.AreSame(advise2, target.CurrentAdvise);
		}

		[Test]
		public void SwitchToNextAdviseCommand_IfCurrentAdviseIsLast_RebuildsAdvises()
		{
			//	Arrange

			var advise1 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<Song>());
			var advise2 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise1 }, new Collection<AdvisedPlaylist> { advise2 });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			target.SwitchToNextAdvise();

			//	Assert

			Assert.AreSame(advise2, target.CurrentAdvise);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfFinshedPlaylistContainsAllSongsFromCurrentAdvise_SwitchesToNextAdvise()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var song3 = new Song();

			var advise1 = AdvisedPlaylist.ForHighlyRatedSongs(new[] { song1, song2 });
			var advise2 = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song1, song2, song3 });

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise1, advise2 });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(advise2, target.CurrentAdvise);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfCurrentAdviseIsLast_RebuildsAdvises()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();

			var advise1 = AdvisedPlaylist.ForHighlyRatedSongs(new[] { song1, song2 });
			var advise2 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song1, song2 });

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise1 }, new Collection<AdvisedPlaylist> { advise2 });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(advise2, target.CurrentAdvise);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfFinshedPlaylistContainsNotAllSongsFromCurrentAdvise_DoesNotSwitchesFromCurrentAdvise()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();

			var currAdvise = AdvisedPlaylist.ForHighlyRatedSongs(new[] { song1, song2 });
			var nextAdvise = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song1 });

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { currAdvise }, new Collection<AdvisedPlaylist> { nextAdvise });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(currAdvise, target.CurrentAdvise);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfFinshedPlaylistCoversSomeFutureAdvises_RemovesThisAdvisesFromList()
		{
			//	Arrange

			var song = new Song();

			var currAdvise = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });
			var coveredAdvise = AdvisedPlaylist.ForHighlyRatedSongs(new[] { song });
			var lastAdvise = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song });

			ICompositePlaylistAdviser adviserStub = Substitute.For<ICompositePlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { currAdvise, coveredAdvise, lastAdvise });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(currAdvise, target.CurrentAdvise);
			target.SwitchToNextAdvise();
			Assert.AreSame(lastAdvise, target.CurrentAdvise);
		}
	}
}
