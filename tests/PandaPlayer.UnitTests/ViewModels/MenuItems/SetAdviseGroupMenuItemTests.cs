using System.Threading.Tasks;
using FluentAssertions;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels.MenuItems
{
	[TestClass]
	public class SetAdviseGroupMenuItemTests
	{
		[TestMethod]
		public void IconKindGetter_ForAssignedAdviseGroup_ReturnsCheckIconKind()
		{
			// Arrange

			var adviseGroup = new AdviseGroupModel
			{
				Name = "Some Group",
				IsFavorite = true,
			};

			var target = new SetAdviseGroupMenuItem(adviseGroup, true, _ => Task.CompletedTask);

			// Act

			var result = target.IconKind;

			// Assert

			result.Should().Be(PackIconKind.Check);
		}

		[TestMethod]
		public void IconKindGetter_ForNonAssignedFavoriteAdviseGroup_ReturnsHeartIconKind()
		{
			// Arrange

			var adviseGroup = new AdviseGroupModel
			{
				Name = "Some Group",
				IsFavorite = true,
			};

			var target = new SetAdviseGroupMenuItem(adviseGroup, false, _ => Task.CompletedTask);

			// Act

			var result = target.IconKind;

			// Assert

			result.Should().Be(PackIconKind.Heart);
		}

		[TestMethod]
		public void IconKindGetter_ForNonAssignedNonFavoriteAdviseGroup_ReturnsNull()
		{
			// Arrange

			var adviseGroup = new AdviseGroupModel
			{
				Name = "Some Group",
				IsFavorite = false,
			};

			var target = new SetAdviseGroupMenuItem(adviseGroup, false, _ => Task.CompletedTask);

			// Act

			var result = target.IconKind;

			// Assert

			result.Should().BeNull();
		}
	}
}
