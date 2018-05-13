using System;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.DiscImages;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.ViewModels.DiscImages
{
	[TestFixture]
	public class DiscImageViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void Constructor_IfMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscImageViewModel(null, Substitute.For<IViewNavigator>()));
		}

		[Test]
		public void Constructor_IfViewNavigatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscImageViewModel(Substitute.For<IMusicLibrary>(), null));
		}

		[Test]
		public void CurrImageFileName_IfNoActiveDiscSet_ReturnsNull()
		{
			// Arrange

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(Arg.Any<Disc>()).Returns("SomeCover.jpg");
			var target = new DiscImageViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			// Act

			var currImageFileName = target.CurrImageFileName;

			// Assert

			Assert.IsNull(currImageFileName);
		}

		[Test]
		public void ActiveDiscChangedEventHandler_IfNewActiveDiscHasCoverImage_SetsCurrImageFileNameToNewDiscCoverImage()
		{
			// Arrange

			var disc = new Disc();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns("SomeDiscCover.jpg");
			var target = new DiscImageViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			// Act

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Assert

			Assert.AreEqual("SomeDiscCover.jpg", target.CurrImageFileName);
		}

		[Test]
		public void ActiveDiscChangedEventHandler_IfNewActiveDiscHasNoCoverImage_SetsCurrImageFileNameToNull()
		{
			// Arrange

			var disc = new Disc();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns((string)null);
			var target = new DiscImageViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			// Act

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Assert

			Assert.IsNull(target.CurrImageFileName);
		}

		[Test]
		public void EditDiscImage_IfNoActiveDiscSet_ReturnsWithNoAction()
		{
			// Arrange

			IViewNavigator viewNavigatorMock = Substitute.For<IViewNavigator>();
			var target = new DiscImageViewModel(Substitute.For<IMusicLibrary>(), viewNavigatorMock);

			// Act

			target.EditDiscImage().Wait();

			// Assert

			viewNavigatorMock.DidNotReceive().ShowEditDiscImageView(Arg.Any<Disc>());
		}

		[Test]
		public void EditDiscImage_IfActiveDiscIsSet_ShowsEditDiscImageViewForActiveDisc()
		{
			// Arrange

			var disc = new Disc();

			IViewNavigator viewNavigatorMock = Substitute.For<IViewNavigator>();
			var target = new DiscImageViewModel(Substitute.For<IMusicLibrary>(), viewNavigatorMock);

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Act

			target.EditDiscImage().Wait();

			// Assert

			viewNavigatorMock.Received(1).ShowEditDiscImageView(disc);
		}

		[Test]
		public void DiscImageChangedEventHandler_IfChangedDiscIsActiveDisc_RaisesPropertyChangedEventForCurrImageFileName()
		{
			// Arrange

			var disc = new Disc();
			var target = new DiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IViewNavigator>());
			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			bool raisedPropertyChangedEvent = false;
			target.PropertyChanged += (sender, e) => raisedPropertyChangedEvent = e.PropertyName == nameof(DiscImageViewModel.CurrImageFileName) || raisedPropertyChangedEvent;

			// Act

			Messenger.Default.Send(new DiscImageChangedEventArgs(disc));

			// Assert

			Assert.IsTrue(raisedPropertyChangedEvent);
		}
	}
}
