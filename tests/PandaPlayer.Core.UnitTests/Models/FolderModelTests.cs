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

			var target = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel { Name = "Subfolder 1" },
					new FolderModel { Name = "Subfolder 2" },
					new FolderModel { Name = "Subfolder 3" },
				},
			};

			var subfolder = new FolderModel
			{
				Name = "Subfolder 2",
			};

			// Act

			var call = () => target.AddSubfolder(subfolder);

			// Assert

			call.Should().Throw<InvalidOperationException>().WithMessage("Cannot add subfolder with duplicated name: 'Subfolder 2'");
		}
	}
}
