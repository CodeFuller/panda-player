using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.UnitTests.ViewModels.LibraryExplorerItems
{
	[TestClass]
	public class DiscExplorerItemTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

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
		public void DiscExplorerItem_WhenDiscPropertyIsChanged_SendsDiscChangedEventForThisProperty()
		{
			// Arrange

			var disc = new DiscModel
			{
				AlbumTitle = "Old Album Title",
			};

			var target = new DiscExplorerItem(disc);

			DiscChangedEventArgs discChangedEventArgs = null;
			Messenger.Default.Register<DiscChangedEventArgs>(this, e => e.RegisterEvent(ref discChangedEventArgs));

			// Act

			disc.AlbumTitle = "New Album Title";

			// Assert

			discChangedEventArgs.Should().NotBeNull();
			discChangedEventArgs.Disc.Should().Be(disc);
			discChangedEventArgs.PropertyName.Should().Be(nameof(DiscModel.AlbumTitle));
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
	}
}
