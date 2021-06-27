using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Grouping;

namespace MusicLibrary.PandaPlayer.UnitTests.Adviser.Grouping
{
	[TestClass]
	public class FolderDiscClassifierTests
	{
		[TestMethod]
		public void GroupLibraryDiscs_AssignsAllDiscsFromOneFolderToOneGroup()
		{
			// Arrange

			var folder1 = new FolderModel { Id = new ItemId("Folder 1") };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2") };

			var disc11 = new DiscModel { Folder = folder1 };
			var disc12 = new DiscModel { Folder = folder1 };
			var disc21 = new DiscModel { Folder = folder2 };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<FolderDiscClassifier>();

			// Act

			var groups = target.GroupLibraryDiscs(new[] { disc11, disc21, disc12 }).ToList();

			// Assert

			var sortedGroups = groups.OrderBy(x => x.Id).ToList();
			Assert.AreEqual(2, sortedGroups.Count);

			Assert.AreEqual("Folder 1", sortedGroups[0].Id);
			CollectionAssert.AreEqual(new[] { disc11, disc12 }, sortedGroups[0].Discs.ToList());

			Assert.AreEqual("Folder 2", sortedGroups[1].Id);
			CollectionAssert.AreEqual(new[] { disc21 }, sortedGroups[1].Discs.ToList());
		}
	}
}
