using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels.DiscImages;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class DiscImageViewModelTests
	{
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

			var disc = new DiscModel().MakeActive();

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			mocker.SendMessage(new ActiveDiscChangedEventArgs(disc));

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

			var disc = new DiscModel().MakeDeleted();

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			mocker.SendMessage(new ActiveDiscChangedEventArgs(disc));

			// Act

			target.EditDiscImageCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowEditDiscImageView(It.IsAny<DiscModel>()), Times.Never);
		}
	}
}
