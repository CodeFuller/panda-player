using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.LibraryExplorerItems;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels.LibraryExplorerItems
{
	[TestClass]
	public class BasicExplorerItemTests
	{
		private abstract class DerivedExplorerItem : BasicExplorerItem
		{
			public static IReadOnlyCollection<BasicMenuItem> InvokeGetAdviseGroupMenuItems(
				BasicAdviseGroupHolder adviseGroupHolder, ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper)
			{
				return GetAdviseGroupMenuItems(adviseGroupHolder, libraryExplorerViewModel, adviseGroupHelper);
			}
		}

		[TestMethod]
		public void GetAdviseGroupMenuItems_IfNoAdviseGroupsExist_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupHelperStub);

			// Act

			var menuItems = DerivedExplorerItem.InvokeGetAdviseGroupMenuItems(Mock.Of<BasicAdviseGroupHolder>(), Mock.Of<ILibraryExplorerViewModel>(), adviseGroupHelperStub.Object);

			// Assert

			var expectedMenuItems = new[]
			{
				new CommandMenuItem(() => Task.CompletedTask) { Header = "New Advise Group ...", IconKind = PackIconKind.FolderPlus },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfAdviseGroupsExistAndItemWithoutAdviseGroupIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var adviseGroupHolderStub = new Mock<BasicAdviseGroupHolder>();
			adviseGroupHolderStub.Setup(x => x.CurrentAdviseGroup).Returns<AdviseGroupModel>(null);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupHelperStub);

			// Act

			var menuItems = DerivedExplorerItem.InvokeGetAdviseGroupMenuItems(adviseGroupHolderStub.Object, Mock.Of<ILibraryExplorerViewModel>(), adviseGroupHelperStub.Object);

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new CommandMenuItem(() => Task.CompletedTask) { Header = "New Advise Group ...", IconKind = PackIconKind.FolderPlus },
				new SeparatorMenuItem(),
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 1", IconKind = null },
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 2", IconKind = null },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfAdviseGroupsExistAndItemWithNonFavoriteAdviseGroupIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
				new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
			};

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var adviseGroupHolderStub = new Mock<BasicAdviseGroupHolder>();
			adviseGroupHolderStub.Setup(x => x.CurrentAdviseGroup).Returns(adviseGroups[1]);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupHelperStub);

			// Act

			var menuItems = DerivedExplorerItem.InvokeGetAdviseGroupMenuItems(adviseGroupHolderStub.Object, Mock.Of<ILibraryExplorerViewModel>(), adviseGroupHelperStub.Object);

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new CommandMenuItem(() => Task.CompletedTask) { Header = "New Advise Group ...", IconKind = PackIconKind.FolderPlus },
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Mark 'Advise Group 2' as favorite", IconKind = PackIconKind.Heart },
				new SeparatorMenuItem(),
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 1", IconKind = null },
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 2", IconKind = PackIconKind.Check },
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 3", IconKind = null },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfAdviseGroupsExistAndItemWithFavoriteAdviseGroupIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2", IsFavorite = true },
				new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
			};

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var adviseGroupHolderStub = new Mock<BasicAdviseGroupHolder>();
			adviseGroupHolderStub.Setup(x => x.CurrentAdviseGroup).Returns(adviseGroups[1]);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupHelperStub);

			// Act

			var menuItems = DerivedExplorerItem.InvokeGetAdviseGroupMenuItems(adviseGroupHolderStub.Object, Mock.Of<ILibraryExplorerViewModel>(), adviseGroupHelperStub.Object);

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new CommandMenuItem(() => Task.CompletedTask) { Header = "New Advise Group ...", IconKind = PackIconKind.FolderPlus },
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Unmark 'Advise Group 2' as favorite", IconKind = PackIconKind.HeartBroken },
				new SeparatorMenuItem(),
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 1", IconKind = null },
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 2", IconKind = PackIconKind.Heart },
				new CommandMenuItem(() => Task.CompletedTask) { Header = "Advise Group 3", IconKind = null },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		/*
				[TestMethod]
				public void AdviseGroupMenuItems_IfDiscIsSelectedAndNoAdviseGroupsExist_ReturnsCorrectMenuItems()
				{
					// Arrange

					var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					menuItems.Should().HaveCount(1);
					menuItems.Single().Should().BeOfType(typeof(NewAdviseGroupMenuItem));
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfAdviseGroupsExistAndFolderWithoutAdviseGroupIsSelected_ReturnsCorrectMenuItems()
				{
					// Arrange

					var adviseGroups = new[]
					{
						new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
						new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
					};

					var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var expectedMenuItems = new BasicMenuItem[]
					{
						new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
						new SeparatorMenuItem(),
						new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
						new SetAdviseGroupMenuItem(adviseGroups[1], false, _ => Task.CompletedTask),
					};

					menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfAdviseGroupsExistAndFolderWithAdviseGroupIsSelected_ReturnsCorrectMenuItems()
				{
					// Arrange

					var adviseGroups = new[]
					{
						new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
						new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
						new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
					};

					var selectedFolder = new ShallowFolderModel
					{
						AdviseGroup = adviseGroups[1],
					};

					var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var expectedMenuItems = new BasicMenuItem[]
					{
						new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
						new ReverseFavoriteStatusForAdviseGroupMenuItem(adviseGroups[1], (_, _) => Task.CompletedTask),
						new SeparatorMenuItem(),
						new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
						new SetAdviseGroupMenuItem(adviseGroups[1], true, _ => Task.CompletedTask),
						new SetAdviseGroupMenuItem(adviseGroups[2], false, _ => Task.CompletedTask),
					};

					menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfFolderIsSelected_CommandForSetAdviseGroupMenuItemReversesSelectedAdviseGroup()
				{
					// Arrange

					var adviseGroups = new[]
					{
						new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
						new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
						new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
					};

					var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(adviseGroups);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperMock);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var menuItem = menuItems.OfType<SetAdviseGroupMenuItem>().ToList()[1];
					menuItem.Command.Execute(null);

					adviseGroupHelperMock.Verify(x => x.ReverseAdviseGroup(It.IsAny<FolderAdviseGroupHolder>(), adviseGroups[1], It.IsAny<CancellationToken>()), Times.Once);
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfAdviseGroupsExistAndDiscWithoutAdviseGroupIsSelected_ReturnsCorrectMenuItems()
				{
					// Arrange

					var adviseGroups = new[]
					{
						new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
						new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
					};

					var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var expectedMenuItems = new BasicMenuItem[]
					{
						new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
						new SeparatorMenuItem(),
						new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
						new SetAdviseGroupMenuItem(adviseGroups[1], false, _ => Task.CompletedTask),
					};

					menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfAdviseGroupsExistAndDiscWithAdviseGroupIsSelected_ReturnsCorrectMenuItems()
				{
					// Arrange

					var adviseGroups = new[]
					{
						new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
						new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
						new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
					};

					var selectedDisc = new DiscModel
					{
						AdviseGroup = adviseGroups[1],
					};

					var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var expectedMenuItems = new BasicMenuItem[]
					{
						new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
						new ReverseFavoriteStatusForAdviseGroupMenuItem(adviseGroups[1], (_, _) => Task.CompletedTask),
						new SeparatorMenuItem(),
						new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
						new SetAdviseGroupMenuItem(adviseGroups[1], true, _ => Task.CompletedTask),
						new SetAdviseGroupMenuItem(adviseGroups[2], false, _ => Task.CompletedTask),
					};

					menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfDiscIsSelected_CommandForSetAdviseGroupMenuItemReversesSelectedAdviseGroup()
				{
					// Arrange

					var adviseGroups = new[]
					{
						new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
						new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
						new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
					};

					var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(adviseGroups);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperMock);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var menuItem = menuItems.OfType<SetAdviseGroupMenuItem>().ToList()[1];
					menuItem.Command.Execute(null);

					adviseGroupHelperMock.Verify(x => x.ReverseAdviseGroup(It.IsAny<DiscAdviseGroupHolder>(), adviseGroups[1], It.IsAny<CancellationToken>()), Times.Once);
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfNewAdviseGroupMenuItemIsExecuted_ShowsShowCreateAdviseGroupViewCorrectly()
				{
					// Arrange

					var adviseGroups = new[]
					{
						new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
						new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
					};

					var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var menuItem = menuItems.OfType<NewAdviseGroupMenuItem>().Single();
					menuItem.Command.Execute(null);

					var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
					viewNavigatorMock.Verify(x => x.ShowCreateAdviseGroupView("Folder Name", new[] { "Advise Group 1", "Advise Group 2" }), Times.Once);
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfShowCreateAdviseGroupViewReturnsNull_DoesNotCreateAdviseGroup()
				{
					// Arrange

					var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

					var viewNavigatorStub = new Mock<IViewNavigator>();
					viewNavigatorStub.Setup(x => x.ShowCreateAdviseGroupView(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns<string>(null);

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperMock);
					mocker.Use(viewNavigatorStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var menuItem = menuItems.OfType<NewAdviseGroupMenuItem>().Single();
					menuItem.Command.Execute(null);

					adviseGroupHelperMock.Verify(x => x.CreateAdviseGroup(It.IsAny<BasicAdviseGroupHolder>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
				}

				[TestMethod]
				public void AdviseGroupMenuItems_IfShowCreateAdviseGroupViewReturnsNotNull_CreatesAdviseGroup()
				{
					// Arrange

					var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
					adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

					var viewNavigatorStub = new Mock<IViewNavigator>();
					viewNavigatorStub.Setup(x => x.ShowCreateAdviseGroupView(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns("Some New Group");

					var mocker = new AutoMocker();
					mocker.Use(adviseGroupHelperMock);
					mocker.Use(viewNavigatorStub);

					// Act

					var menuItems = target.AdviseGroupMenuItems;

					// Assert

					var menuItem = menuItems.OfType<NewAdviseGroupMenuItem>().Single();
					menuItem.Command.Execute(null);

					adviseGroupHelperMock.Verify(x => x.CreateAdviseGroup(It.IsAny<FolderAdviseGroupHolder>(), "Some New Group", It.IsAny<CancellationToken>()), Times.Once);
				}
		*/
	}
}
