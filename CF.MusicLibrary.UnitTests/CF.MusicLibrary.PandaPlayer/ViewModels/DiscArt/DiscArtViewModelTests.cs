using System;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt
{
	[TestFixture]
	public class DiscArtViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void Constructor_IfMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtViewModel(null, Substitute.For<IViewNavigator>()));
		}

		[Test]
		public void Constructor_IfViewNavigatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtViewModel(Substitute.For<IMusicLibrary>(), null));
		}

		[Test]
		public void CurrImageFileName_IfNoActiveDiscSet_ReturnsNull()
		{
			//	Arrange

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(Arg.Any<Disc>()).Returns("SomeCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.IsNull(currImageFileName);
		}

		[Test]
		public void ActiveDiscChangedEventHandler_IfNewActiveDiscHasCoverImage_SetsCurrImageFileNameToNewDiscCoverImage()
		{
			//	Arrange

			var disc = new Disc();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns("SomeDiscCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			//	Assert

			Assert.AreEqual("SomeDiscCover.jpg", target.CurrImageFileName);
		}

		[Test]
		public void ActiveDiscChangedEventHandler_IfNewActiveDiscHasNoCoverImage_SetsCurrImageFileNameToNull()
		{
			//	Arrange

			var disc = new Disc();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns((string)null);
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			//	Assert

			Assert.IsNull(target.CurrImageFileName);
		}

		[Test]
		public void EditDiscArt_IfNoActiveDiscSet_ReturnsWithNoAction()
		{
			//	Arrange

			IViewNavigator viewNavigatorMock = Substitute.For<IViewNavigator>();
			var target = new DiscArtViewModel(Substitute.For<IMusicLibrary>(), viewNavigatorMock);

			//	Act

			target.EditDiscArt().Wait();

			//	Assert

			viewNavigatorMock.DidNotReceive().ShowEditDiscArtView(Arg.Any<Disc>());
		}

		[Test]
		public void EditDiscArt_IfActiveDiscIsSet_ShowEditDiscArtViewForExplorerDisc()
		{
			//	Arrange

			var disc = new Disc();

			IViewNavigator viewNavigatorMock = Substitute.For<IViewNavigator>();
			var target = new DiscArtViewModel(Substitute.For<IMusicLibrary>(), viewNavigatorMock);

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			//	Act

			target.EditDiscArt().Wait();

			//	Assert

			viewNavigatorMock.Received(1).ShowEditDiscArtView(disc);
		}

		[Test]
		public void DiscArtChangedEventHandler_IfChangedDiscIsActiveDisc_RaisesPropertyChangedEventForCurrImageFileName()
		{
			//	Arrange

			var disc = new Disc();
			var target = new DiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IViewNavigator>());
			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			bool raisedPropertyChangedEvent = false;
			target.PropertyChanged += (sender, e) => raisedPropertyChangedEvent = (e.PropertyName == nameof(DiscArtViewModel.CurrImageFileName) || raisedPropertyChangedEvent);

			//	Act

			Messenger.Default.Send(new DiscArtChangedEventArgs(disc));

			//	Assert

			Assert.IsTrue(raisedPropertyChangedEvent);
		}
	}
}
