using System;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.DiscAdviser;
using CF.MusicLibrary.PandaPlayer.Events;
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
			Assert.Throws<ArgumentNullException>(() => new DiscAdviserViewModel(null, Substitute.For<IPlaylistAdviser>()));
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

			var advise = new AdvisedPlaylist(String.Empty, Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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

			var target = new DiscAdviserViewModel(new DiscLibrary(), Substitute.For<IPlaylistAdviser>());

			//	Act & Assert

			Assert.IsNull(target.CurrentAdvise);
		}

		[Test]
		public void CurrentAdviseAnnouncementGetter_IfCurrentAdviseIsNotSet_DoesNotThrow()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(), Substitute.For<IPlaylistAdviser>());

			//	Act & Assert

			string currAnnouncement;
			Assert.DoesNotThrow(() => currAnnouncement =  target.CurrentAdviseAnnouncement);
		}

		[Test]
		public void CurrentAdviseAnnouncementGetter_IfCurrentAdviseIsSet_ReturnsAdviseTitle()
		{
			//	Arrange

			var advise = new AdvisedPlaylist("Some Advise Title", Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
			adviserStub.Advise(library).Returns(new Collection<AdvisedPlaylist> { advise });

			var target = new DiscAdviserViewModel(library, adviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			string announcement = target.CurrentAdviseAnnouncement;

			//	Assert

			Assert.AreEqual("Some Advise Title", announcement);
		}

		[Test]
		public void PlayCurrentAdviseCommand_IfNoAdvisesAreMade_ReturnsWithNoAction()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(), Substitute.For<IPlaylistAdviser>());

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
		public void PlayCurrentAdviseCommand_IfSomeAdvisesAreMade_SendsPlaySongsListEventForCurrentAdvise()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var advise = new AdvisedPlaylist(String.Empty, new[] { song1, song2 });
			var library = new DiscLibrary();

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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

			var advise1 = new AdvisedPlaylist(String.Empty, Enumerable.Empty<Song>());
			var advise2 = new AdvisedPlaylist(String.Empty, Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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

			var advise1 = new AdvisedPlaylist(String.Empty, Enumerable.Empty<Song>());
			var advise2 = new AdvisedPlaylist(String.Empty, Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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

			var advise1 = new AdvisedPlaylist(String.Empty, new[] { song1, song2 });
			var advise2 = new AdvisedPlaylist(String.Empty, new[] { new Song() });
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song1, song2, song3 });

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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

			var advise1 = new AdvisedPlaylist(String.Empty, new[] { song1, song2 });
			var advise2 = new AdvisedPlaylist(String.Empty, Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song1, song2 });

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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

			var currAdvise = new AdvisedPlaylist(String.Empty, new[] { song1, song2 });
			var nextAdvise = new AdvisedPlaylist(String.Empty, Enumerable.Empty<Song>());
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song1 });

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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

			var currAdvise = new AdvisedPlaylist(String.Empty, new[] { new Song() });
			var coveredAdvise = new AdvisedPlaylist(String.Empty, new[] { song });
			var lastAdvise = new AdvisedPlaylist(String.Empty, new[] { new Song() });
			var library = new DiscLibrary();

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.Songs.Returns(new[] { song });

			IPlaylistAdviser adviserStub = Substitute.For<IPlaylistAdviser>();
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
