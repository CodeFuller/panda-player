using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.ViewModels.DiscImages;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class DiscImageViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void EditDiscImageCommand_IfNoDiscIsSet_DoesNothing()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			// Act

			target.EditDiscImageCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowEditDiscImageView(It.IsAny<DiscModel>()), Times.Never);
		}

		[TestMethod]
		public void EditDiscImageCommand_IfActiveDiscIsSet_ShowsEditDiscImageViewForDisc()
		{
			// Arrange

			var disc = new DiscModel
			{
				AllSongs = new[] { new SongModel() },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Act

			target.EditDiscImageCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowEditDiscImageView(disc), Times.Once);
		}

		[TestMethod]
		public void EditDiscImageCommand_IfDeletedDiscIsSet_DoesNothing()
		{
			// Arrange

			var disc = new DiscModel
			{
				AllSongs = new[] { new SongModel { DeleteDate = new DateTime(2021, 07, 26) } },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Act

			target.EditDiscImageCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowEditDiscImageView(It.IsAny<DiscModel>()), Times.Never);
		}
	}
}
