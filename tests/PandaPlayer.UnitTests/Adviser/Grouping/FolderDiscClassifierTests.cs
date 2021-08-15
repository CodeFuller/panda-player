﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Adviser.Grouping
{
	[TestClass]
	public class FolderDiscClassifierTests
	{
		[TestMethod]
		public async Task GroupLibraryDiscs_AssignsAllDiscsFromOneFolderToOneGroup()
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

			var groups = await target.GroupLibraryDiscs(new[] { disc11, disc21, disc12 }, CancellationToken.None);

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
