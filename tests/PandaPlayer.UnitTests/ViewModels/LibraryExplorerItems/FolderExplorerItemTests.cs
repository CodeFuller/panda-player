using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.LibraryExplorerItems;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels.LibraryExplorerItems
{
	[TestClass]
	public class FolderExplorerItemTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void TitleGetter_ReturnsFolderName()
		{
			// Arrange

			var folder = new FolderModel
			{
				Name = "Folder Name",
			};

			var target = new FolderExplorerItem(folder);

			// Act

			var title = target.Title;

			// Assert

			title.Should().Be("Folder Name");
		}

		[TestMethod]
		public void IconKindGetter_ForFolderWithoutAdviseGroup_ReturnsFolderIconKind()
		{
			// Arrange

			var folder = new FolderModel
			{
				AdviseGroup = null,
			};

			var target = new FolderExplorerItem(folder);

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.Folder);
		}

		[TestMethod]
		public void IconKindGetter_ForFolderWithNonFavoriteAdviseGroup_ReturnsFolderStarIconKind()
		{
			// Arrange

			var folder = new FolderModel
			{
				AdviseGroup = new AdviseGroupModel
				{
					Id = new ItemId("1"),
					Name = "Some Advise Group",
					IsFavorite = false,
				},
			};

			var target = new FolderExplorerItem(folder);

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.FolderStar);
		}

		[TestMethod]
		public void IconKindGetter_ForFolderWithFavoriteAdviseGroup_ReturnsFolderHeartIconKind()
		{
			// Arrange

			var folder = new FolderModel
			{
				AdviseGroup = new AdviseGroupModel
				{
					Id = new ItemId("1"),
					Name = "Some Advise Group",
					IsFavorite = true,
				},
			};

			var target = new FolderExplorerItem(folder);

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.FolderHeart);
		}

		[TestMethod]
		public void IsDeletedGetter_ForActiveFolder_ReturnsFalse()
		{
			// Arrange

			var folder = new FolderModel { DeleteDate = null };

			var target = new FolderExplorerItem(folder);

			// Act

			var isDeleted = target.IsDeleted;

			// Assert

			isDeleted.Should().BeFalse();
		}

		[TestMethod]
		public void IsDeletedGetter_ForDeletedFolder_ReturnsTrue()
		{
			// Arrange

			var folder = new FolderModel { DeleteDate = new DateTime(2021, 10, 25) };

			var target = new FolderExplorerItem(folder);

			// Act

			var isDeleted = target.IsDeleted;

			// Assert

			isDeleted.Should().BeTrue();
		}

		[TestMethod]
		public void FolderExplorerItem_WhenFolderNameIsChanged_SendsPropertyChangedEventForTitle()
		{
			// Arrange

			var folder = new FolderModel
			{
				Name = "Old Name",
			};

			var target = new FolderExplorerItem(folder);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			folder.Name = "New Name";

			// Assert

			var expectedProperties = new[]
			{
				nameof(FolderExplorerItem.Title),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void FolderExplorerItem_WhenFolderAdviseGroupIsChanged_SendsPropertyChangedEventForIconKind()
		{
			// Arrange

			var folder = new FolderModel
			{
				AdviseGroup = null,
			};

			var target = new FolderExplorerItem(folder);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			folder.AdviseGroup = new AdviseGroupModel { Id = new ItemId("1"), Name = "New Advise Group" };

			// Assert

			var expectedProperties = new[]
			{
				nameof(FolderExplorerItem.IconKind),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void GetContextMenuItems_ForActiveFolder_ReturnsCorrectMenuItems()
		{
			// Arrange

			var folder = new FolderModel { DeleteDate = null };

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var target = new FolderExplorerItem(folder);

			// Act

			var menuItems = target.GetContextMenuItems(Mock.Of<ILibraryExplorerViewModel>(), adviseGroupHelperStub.Object).ToArray();

			// Assert

			var expectedSetAdviseGroupItem = new ExpandableMenuItem
			{
				Header = "Set Advise Group",
				IconKind = PackIconKind.FolderStar,
			};

			var expectedMenuItems = new[]
			{
				new CommandMenuItem(() => { }, false) { Header = "Rename Folder", IconKind = PackIconKind.Pencil },
				new CommandMenuItem(() => { }, false) { Header = "Delete Folder", IconKind = PackIconKind.DeleteForever },
			};

			// Nested items for advise group are covered by UT for BasicExplorerItem.
			menuItems[0].Should().BeEquivalentTo(expectedSetAdviseGroupItem, x => x.Excluding(y => y.Items));
			menuItems[1..].Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void GetContextMenuItems_ForDeletedFolder_ReturnsEmptyCollection()
		{
			// Arrange

			var folder = new FolderModel { DeleteDate = new DateTime(2021, 10, 25) };

			var target = new FolderExplorerItem(folder);

			// Act

			var menuItems = target.GetContextMenuItems(Mock.Of<ILibraryExplorerViewModel>(), Mock.Of<IAdviseGroupHelper>());

			// Assert

			menuItems.Should().BeEmpty();
		}
	}
}
