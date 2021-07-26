using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
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
	}
}
