using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.LibraryExplorerItems;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels.LibraryExplorerItems
{
	[TestClass]
	public class DiscExplorerItemTests
	{
		[TestMethod]
		public void TitleGetter_ReturnsDiscTreeTitle()
		{
			// Arrange

			var disc = new DiscModel
			{
				TreeTitle = "Disc Tree Title",
			};

			var target = new DiscExplorerItem(disc);

			// Act

			var title = target.Title;

			// Assert

			title.Should().Be("Disc Tree Title");
		}

		[TestMethod]
		public void IconKindGetter_ForDiscWithoutAdviseGroupAndAdviseSet_ReturnsAlbumIconKind()
		{
			// Arrange

			var disc = new DiscModel
			{
				AdviseGroup = null,
				AdviseSetInfo = null,
			};

			var target = new DiscExplorerItem(disc);

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.Album);
		}

		[TestMethod]
		public void IconKindGetter_ForDiscWithAdviseGroup_ReturnsDiscAlertIconKind()
		{
			// Arrange

			var disc = new DiscModel
			{
				AdviseGroup = new AdviseGroupModel { Id = new ItemId("1"), Name = "Some Advise Group" },
			};

			var target = new DiscExplorerItem(disc);

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.DiscAlert);
		}

		[TestMethod]
		public void IconKindGetter_ForDiscWithAdviseSet_ReturnsDiscAlertIconKind()
		{
			// Arrange

			var disc = new DiscModel
			{
				AdviseSetInfo = new AdviseSetInfo(new AdviseSetModel(), 1),
			};

			var target = new DiscExplorerItem(disc);

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.DiscAlert);
		}

		[TestMethod]
		public void IsDeletedGetter_ForActiveDisc_ReturnsFalse()
		{
			// Arrange

			var disc = new DiscModel().MakeActive();

			var target = new DiscExplorerItem(disc);

			// Act

			var isDeleted = target.IsDeleted;

			// Assert

			isDeleted.Should().BeFalse();
		}

		[TestMethod]
		public void IsDeletedGetter_ForDeletedDisc_ReturnsTrue()
		{
			// Arrange

			var disc = new DiscModel().MakeDeleted();

			var target = new DiscExplorerItem(disc);

			// Act

			var isDeleted = target.IsDeleted;

			// Assert

			isDeleted.Should().BeTrue();
		}

		[TestMethod]
		public void ToolTipGetter_ForActiveDisc_ReturnsNull()
		{
			// Arrange

			var disc = new DiscModel().MakeActive();

			var target = new DiscExplorerItem(disc);

			// Act

			var toolTip = target.ToolTip;

			// Assert

			toolTip.Should().BeNull();
		}

		[TestMethod]
		public void ToolTipGetter_ForDeletedDiscWithSameDeleteDateAndComment_ReturnsCorrectValue()
		{
			// Arrange

			var disc = new DiscModel();
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = "Boring" });
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = "Boring" });

			var target = new DiscExplorerItem(disc);

			// Act

			var toolTip = target.ToolTip;

			// Assert

			toolTip.Should().Be("The disc was deleted on 2021.10.26 with the comment 'Boring'");
		}

		[TestMethod]
		public void ToolTipGetter_ForDeletedDiscWithSameDeleteDateAndWithoutComment_ReturnsCorrectValue()
		{
			// Arrange

			var disc = new DiscModel();
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = null });
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = null });

			var target = new DiscExplorerItem(disc);

			// Act

			var toolTip = target.ToolTip;

			// Assert

			toolTip.Should().Be("The disc was deleted on 2021.10.26 without comment");
		}

		[TestMethod]
		public void ToolTipGetter_ForDeletedDiscWithSameDeleteDateAndVariousComments_ReturnsCorrectValue()
		{
			// Arrange

			var disc = new DiscModel();
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = "Boring 1" });
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = "Boring 2" });

			var target = new DiscExplorerItem(disc);

			// Act

			var toolTip = target.ToolTip;

			// Assert

			toolTip.Should().Be("The disc was deleted on 2021.10.26 with various comments");
		}

		[TestMethod]
		public void ToolTipGetter_ForDeletedDiscWithVariousDeleteDateAndSameComment_ReturnsCorrectValue()
		{
			// Arrange

			var disc = new DiscModel();
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring" });
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = "Boring" });

			var target = new DiscExplorerItem(disc);

			// Act

			var toolTip = target.ToolTip;

			// Assert

			toolTip.Should().Be("The disc was deleted with the comment 'Boring'");
		}

		[TestMethod]
		public void ToolTipGetter_ForDeletedDiscWithVariousDeleteDateAndWithoutComment_ReturnsCorrectValue()
		{
			// Arrange

			var disc = new DiscModel();
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = null });
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = null });

			var target = new DiscExplorerItem(disc);

			// Act

			var toolTip = target.ToolTip;

			// Assert

			toolTip.Should().Be("The disc was deleted without comment");
		}

		[TestMethod]
		public void ToolTipGetter_ForDeletedDiscWithVariousDeleteDateAndComments_ReturnsCorrectValue()
		{
			// Arrange

			var disc = new DiscModel();
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 25), DeleteComment = "Boring 1" });
			disc.AddSongs(new SongModel { DeleteDate = new DateTime(2021, 10, 26), DeleteComment = "Boring 2" });

			var target = new DiscExplorerItem(disc);

			// Act

			var toolTip = target.ToolTip;

			// Assert

			toolTip.Should().Be("The disc was deleted with various comments");
		}

		[TestMethod]
		public void DiscExplorerItem_WhenDiscTreeTitleIsChanged_SendsPropertyChangedEventForTitle()
		{
			// Arrange

			var disc = new DiscModel
			{
				TreeTitle = "Old Tree Title",
			};

			var target = new DiscExplorerItem(disc);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			disc.TreeTitle = "New Tree Title";

			// Assert

			var expectedProperties = new[]
			{
				nameof(DiscExplorerItem.Title),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void DiscExplorerItem_WhenDiscAdviseGroupIsChanged_SendsPropertyChangedEventForIconKind()
		{
			// Arrange

			var disc = new DiscModel
			{
				AdviseGroup = null,
			};

			var target = new DiscExplorerItem(disc);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			disc.AdviseGroup = new AdviseGroupModel { Id = new ItemId("1"), Name = "New Advise Group" };

			// Assert

			var expectedProperties = new[]
			{
				nameof(DiscExplorerItem.IconKind),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void GetContextMenuItems_ForActiveDisc_ReturnsCorrectMenuItems()
		{
			// Arrange

			var disc = new DiscModel().MakeActive();

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var target = new DiscExplorerItem(disc);

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
				new CommandMenuItem(() => { }) { Header = "Play Disc", IconKind = PackIconKind.Play },
				new CommandMenuItem(() => { }) { Header = "Add To Playlist", IconKind = PackIconKind.PlaylistPlus },
				new CommandMenuItem(() => { }) { Header = "Delete Disc", IconKind = PackIconKind.DeleteForever },
				new CommandMenuItem(() => { }) { Header = "Properties", IconKind = PackIconKind.Pencil },
			};

			// Nested items for advise group are covered by UT for BasicExplorerItem.
			menuItems[0].Should().BeEquivalentTo(expectedSetAdviseGroupItem, x => x.Excluding(y => y.Items));
			menuItems[1..].Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void GetContextMenuItems_ForDeletedDisc_ReturnsCorrectMenuItems()
		{
			// Arrange

			var disc = new DiscModel().MakeDeleted();

			var target = new DiscExplorerItem(disc);

			// Act

			var menuItems = target.GetContextMenuItems(Mock.Of<ILibraryExplorerViewModel>(), Mock.Of<IAdviseGroupHelper>());

			// Assert

			var expectedMenuItems = new[]
			{
				new CommandMenuItem(() => { }) { Header = "Properties", IconKind = PackIconKind.Pencil },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering());
		}
	}
}
