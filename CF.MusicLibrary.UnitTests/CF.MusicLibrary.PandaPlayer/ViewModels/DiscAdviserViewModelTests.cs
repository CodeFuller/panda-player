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
			Assert.Throws<ArgumentNullException>(() => new DiscAdviserViewModel(null, Substitute.For<IDiscAdviser>()));
		}

		[Test]
		public void Constructor_IfDiscAdviserArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscAdviserViewModel(new DiscLibrary(Enumerable.Empty<Disc>()), null));
		}

		[Test]
		public void Load_FillsAdvisedDiscs()
		{
			//	Arrange

			var disc = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc });

			var target = new DiscAdviserViewModel(library, discAdviserStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Assert

			Assert.AreSame(disc, target.CurrentDisc);
		}

		[Test]
		public void CurrentDiscGetter_IfNoDiscsAreAdvised_ReturnsNull()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(Enumerable.Empty<Disc>()), Substitute.For<IDiscAdviser>());

			//	Act & Assert

			Assert.IsNull(target.CurrentDisc);
		}

		[Test]
		public void CurrentDiscAnnouncementGetter_IfNoDiscsAreAdvised_DoesNotThrow()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(Enumerable.Empty<Disc>()), Substitute.For<IDiscAdviser>());

			//	Act & Assert

			string currDiscAnnouncement;
			Assert.DoesNotThrow(() => currDiscAnnouncement =  target.CurrentDiscAnnouncement);
		}

		[Test]
		public void CurrentDiscAnnouncementGetter_IfDiscWithoutArtistIsAdvised_ReturnsCorrectDiscAnnouncement()
		{
			//	Arrange

			var disc = new Disc
			{
				Title = "Some Disc Title",
			};
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			string discAnnouncement = target.CurrentDiscAnnouncement;

			//	Assert

			Assert.AreEqual("Some Disc Title", discAnnouncement);
		}

		[Test]
		public void CurrentDiscAnnouncementGetter_IfDiscWithArtistIsAdvised_ReturnsCorrectDiscAnnouncement()
		{
			//	Arrange

			var disc = new Disc
			{
				Title = "Some Disc Title",
				SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Some Artist" } } },
			};
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			string discAnnouncement = target.CurrentDiscAnnouncement;

			//	Assert

			Assert.AreEqual("Some Artist - Some Disc Title", discAnnouncement);
		}

		[Test]
		public void PlayCurrentDiscCommand_IfNoDiscsAreAdvised_ReturnsWithNoAction()
		{
			//	Arrange

			var target = new DiscAdviserViewModel(new DiscLibrary(Enumerable.Empty<Disc>()), Substitute.For<IDiscAdviser>());

			bool receivedEvent = false;
			Messenger.Default.Register<PlayDiscEventArgs>(this, e => receivedEvent = true);

			//	Act

			target.PlayCurrentDisc();

			//	Assert

			Assert.IsFalse(receivedEvent);
			//	Avoiding uncovered lambda code (receivedEvent = true)
			Messenger.Default.Send(new PlayDiscEventArgs(null));
		}

		[Test]
		public void PlayCurrentDiscCommand_IfSomeDiscsAreAdvised_SendsPlayDiscEventForNextAdvisedDisc()
		{
			//	Arrange

			var disc = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			PlayDiscEventArgs receivedEvent = null;
			Messenger.Default.Register<PlayDiscEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.PlayCurrentDisc();

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(disc, receivedEvent.Disc);
		}

		[Test]
		public void SwitchToNextDiscCommand_IfCurrentAdvisedDiscIsNotLast_SwitchesToNextDiscInAdviseList()
		{
			//	Arrange

			var disc1 = new Disc();
			var disc2 = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc1, disc2 });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			target.SwitchToNextDisc();

			//	Assert

			Assert.AreSame(disc2, target.CurrentDisc);
		}

		[Test]
		public void SwitchToNextDiscCommand_IfCurrentAdvisedDiscIsLast_LoadsNewAdvisedDiscs()
		{
			//	Arrange

			var disc1 = new Disc();
			var disc2 = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc1 }, new Collection<Disc> { disc2 });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			target.SwitchToNextDisc();

			//	Assert

			Assert.AreSame(disc2, target.CurrentDisc);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfFinshedDiscIsCurrentAdvisedDisc_SwitchesToNextDiscInAdviseList()
		{
			//	Arrange

			var disc1 = new Disc();
			var disc2 = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.PlayedDisc.Returns(disc1);

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc1, disc2 });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(disc2, target.CurrentDisc);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfCurrentAdvisedDiscIsLast_LoadsNewAdvisedDiscs()
		{
			//	Arrange

			var disc1 = new Disc();
			var disc2 = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.PlayedDisc.Returns(disc1);

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc1 }, new Collection<Disc> { disc2 });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(disc2, target.CurrentDisc);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfFinshedDiscIsNotCurrentAdvisedDisc_DoesNotSwitchesFromCurrentDisc()
		{
			//	Arrange

			var finishedDisc = new Disc();
			var currDisc = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.PlayedDisc.Returns(finishedDisc);

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { currDisc }, new Collection<Disc> { new Disc() });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(currDisc, target.CurrentDisc);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfFinshedDiscPresentsInFutureAdvisedDisc_RemovesThisDiscFromAdviseList()
		{
			//	Arrange

			var currDisc = new Disc();
			var finishedDisc = new Disc();
			var lastDisc = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.PlayedDisc.Returns(finishedDisc);

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { currDisc, finishedDisc, lastDisc });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(currDisc, target.CurrentDisc);
			target.SwitchToNextDisc();
			Assert.AreSame(lastDisc, target.CurrentDisc);
		}

		[Test]
		public void PlaylistFinishedEventHandler_IfFinshedDiscIsNull_ReturnsWithNoAction()
		{
			//	Arrange

			var disc = new Disc();
			var library = new DiscLibrary(Enumerable.Empty<Disc>());

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.PlayedDisc.Returns((Disc)null);

			IDiscAdviser discAdviserStub = Substitute.For<IDiscAdviser>();
			discAdviserStub.AdviseDiscs(library).Returns(new Collection<Disc> { disc });

			var target = new DiscAdviserViewModel(library, discAdviserStub);
			Messenger.Default.Send(new LibraryLoadedEventArgs(library));

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));

			//	Assert

			Assert.AreSame(disc, target.CurrentDisc);
		}
	}
}
