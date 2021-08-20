using FluentAssertions;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.UnitTests.ViewModels.LibraryExplorerItems
{
	[TestClass]
	public class ParentFolderExplorerItemTests
	{
		[TestMethod]
		public void TitleGetter_ReturnsCorrectTitle()
		{
			// Arrange

			var target = new ParentFolderExplorerItem();

			// Act

			var title = target.Title;

			// Assert

			title.Should().Be("..");
		}

		[TestMethod]
		public void IconKindGetter_ReturnsArrowUpBoldIconKind()
		{
			// Arrange

			var target = new ParentFolderExplorerItem();

			// Act

			var iconKind = target.IconKind;

			// Assert

			iconKind.Should().Be(PackIconKind.ArrowUpBold);
		}
	}
}
