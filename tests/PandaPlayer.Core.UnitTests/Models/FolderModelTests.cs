using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.UnitTests.Models
{
	[TestClass]
	public class FolderModelTests
	{
		[TestMethod]
		public void AddSubfolder_IfSubfolderWithSameNameAlreadyExists_ThrowsInvalidOperationException()
		{
			// Arrange

			var target = new FolderModel();

			target.AddSubfolder(new FolderModel { Name = "Subfolder 1" });
			target.AddSubfolder(new FolderModel { Name = "Subfolder 2" });
			target.AddSubfolder(new FolderModel { Name = "Subfolder 3" });

			// Act

			var call = () => target.AddSubfolder(new FolderModel { Name = "Subfolder 2" });

			// Assert

			call.Should().Throw<InvalidOperationException>().WithMessage("Cannot add subfolder with duplicated name: 'Subfolder 2'");
		}
	}
}
