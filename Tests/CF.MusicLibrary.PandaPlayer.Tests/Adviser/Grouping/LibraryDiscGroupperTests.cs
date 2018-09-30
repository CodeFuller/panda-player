using System;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser.Grouping;
using CF.MusicLibrary.Tests;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.Adviser.Grouping
{
	[TestFixture]
	public class LibraryDiscGroupperTests
	{
		[Test]
		public void GroupLibraryDiscs_WhenDiscUrlIsMatchedBySomeGroup_AssignsDiscToThisGroup()
		{
			// Arrange

			var disc = new Disc { DiscUri = "/DiscUri" };

			var nonMatchedGroupStub = Substitute.For<GroupPattern>();
			var matchedGroupStub = Substitute.For<GroupPattern>();
			matchedGroupStub.Matches(new Uri("/DiscUri", UriKind.Relative), out string _)
				.Returns(x =>
				{
					x[1] = "SomeGroupId";
					return true;
				});

			var settings = new GroupingSettings { nonMatchedGroupStub, matchedGroupStub, };

			var target = new LibraryDiscGroupper(settings.StubOptions());

			// Act

			var groups = target.GroupLibraryDiscs(new DiscLibrary(new[] { disc }));

			// Assert

			var group = groups.Single();
			Assert.AreEqual("SomeGroupId", group.Id);
			Assert.AreSame(disc, group.Discs.Single());
		}

		[Test]
		public void GroupLibraryDiscs_IfDiscUrlIsMatchedByMultipleGroups_AssignsDiscToFirstMatchedGroup()
		{
			// Arrange

			var disc = new Disc { DiscUri = "/DiscUri" };

			var firstMatchedGroup = Substitute.For<GroupPattern>();
			firstMatchedGroup.Matches(new Uri("/DiscUri", UriKind.Relative), out string _)
				.Returns(x =>
				{
					x[1] = "FirstGroupId";
					return true;
				});

			var secondMatchedGroup = Substitute.For<GroupPattern>();
			secondMatchedGroup.Matches(new Uri("/DiscUri", UriKind.Relative), out string _)
				.Returns(x =>
				{
					x[1] = "SecondGroupId";
					return true;
				});

			var settings = new GroupingSettings { firstMatchedGroup, secondMatchedGroup, };

			var target = new LibraryDiscGroupper(settings.StubOptions());

			// Act

			var groups = target.GroupLibraryDiscs(new DiscLibrary(new[] { disc }));

			// Assert

			var group = groups.Single();
			Assert.AreEqual("FirstGroupId", group.Id);
			Assert.AreSame(disc, group.Discs.Single());
		}

		[Test]
		public void GroupLibraryDiscs_WhenMultipleDiscsAreMatchedBySomeGroup_AssignsAllMatchedDiscsToTheGroup()
		{
			// Arrange

			var disc1 = new Disc { DiscUri = "/DiscUri1" };
			var disc2 = new Disc { DiscUri = "/DiscUri2" };

			var groupStub = Substitute.For<GroupPattern>();
			groupStub.Matches(Arg.Any<Uri>(), out _).ReturnsForAnyArgs(x =>
			{
				x[1] = "SomeGroupId";
				return true;
			});

			var target = new LibraryDiscGroupper(new GroupingSettings { groupStub, }.StubOptions());

			// Act

			var groups = target.GroupLibraryDiscs(new DiscLibrary(new[] { disc1, disc2 }));

			// Assert

			var group = groups.Single();
			CollectionAssert.AreEqual(new[] { disc1, disc2 }, group.Discs);
		}

		[Test]
		public void GroupLibraryDiscs_IfDiscUrlIsNotMatchedByAnyGroup_Throws()
		{
			// Arrange

			var disc = new Disc { DiscUri = "/DiscUri" };

			var settings = new GroupingSettings { Substitute.For<GroupPattern>(), };

			var target = new LibraryDiscGroupper(settings.StubOptions());

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.GroupLibraryDiscs(new DiscLibrary(new[] { disc })));
		}
	}
}
