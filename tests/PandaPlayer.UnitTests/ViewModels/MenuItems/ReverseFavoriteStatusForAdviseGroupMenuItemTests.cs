using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels.MenuItems
{
	[TestClass]
	public class ReverseFavoriteStatusForAdviseGroupMenuItemTests
	{
		[TestMethod]
		public void HeaderGetter_ForNonFavoriteAdviseGroup_ReturnsCorrectValue()
		{
			// Arrange

			var adviseGroup = new AdviseGroupModel
			{
				Name = "Some Group",
				IsFavorite = false,
			};

			var target = new ReverseFavoriteStatusForAdviseGroupMenuItem(adviseGroup, (_, _) => Task.CompletedTask);

			// Act

			var result = target.Header;

			// Assert

			result.Should().Be("Mark 'Some Group' as favorite");
		}

		[TestMethod]
		public void HeaderGetter_ForFavoriteAdviseGroup_ReturnsCorrectValue()
		{
			// Arrange

			var adviseGroup = new AdviseGroupModel
			{
				Name = "Some Group",
				IsFavorite = true,
			};

			var target = new ReverseFavoriteStatusForAdviseGroupMenuItem(adviseGroup, (_, _) => Task.CompletedTask);

			// Act

			var result = target.Header;

			// Assert

			result.Should().Be("Unmark 'Some Group' as favorite");
		}
	}
}
