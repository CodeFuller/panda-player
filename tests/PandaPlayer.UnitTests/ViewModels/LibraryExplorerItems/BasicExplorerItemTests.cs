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
	}
}
