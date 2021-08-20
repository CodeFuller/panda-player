using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.LibraryExplorerItems;

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

			var folder = new ShallowFolderModel
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

			var folder = new ShallowFolderModel
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
		public void IconKindGetter_ForFolderWithAdviseGroup_ReturnsFolderStarIconKind()
		{
			// Arrange

			var folder = new ShallowFolderModel
			{
				AdviseGroup = new AdviseGroupModel { Id = new ItemId("1"), Name = "Some Advise Group" },
			};

			var target = new FolderExplorerItem(folder);

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.FolderStar);
		}

		[TestMethod]
		public void FolderExplorerItem_WhenFolderAdviseGroupIsChanged_SendsPropertyChangedEventForIconKind()
		{
			// Arrange

			var folder = new ShallowFolderModel
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
	}
}
