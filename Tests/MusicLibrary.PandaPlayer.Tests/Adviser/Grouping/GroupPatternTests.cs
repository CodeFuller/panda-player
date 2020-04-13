using System;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using NUnit.Framework;

namespace MusicLibrary.PandaPlayer.Tests.Adviser.Grouping
{
	[TestFixture]
	public class GroupPatternTests
	{
		[Test]
		public void PatternSetter_ForInvalidRegularExpression_ThrowsInvalidOperationException()
		{
			// Arrange

			var target = new GroupPattern();

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Pattern = "Invalid Expression [(");
		}

		[Test]
		public void Matches_IfDiscUriMatchesPattern_ReturnsGroupId()
		{
			// Arrange

			var target = new GroupPattern
			{
				Pattern = "^/SomeCategory/SomeArtist",
				GroupId = "SomeGroupId",
			};

			// Act

			var result = target.Matches(new Uri("/SomeCategory/SomeArtist/SomeAlbum", UriKind.Relative), out var groupId);

			// Assert

			Assert.IsTrue(result);
			Assert.AreEqual("SomeGroupId", groupId);
		}

		[Test]
		public void Matches_WhenGroupIdContainsTemplateValues_BuildsGroupIdCorrectly()
		{
			// Arrange

			var target = new GroupPattern
			{
				Pattern = "^/(SomeCategory)/(SomeArtist)",
				GroupId = "$1 - $2",
			};

			// Act

			target.Matches(new Uri("/SomeCategory/SomeArtist/SomeAlbum", UriKind.Relative), out var groupId);

			// Assert

			Assert.AreEqual("SomeCategory - SomeArtist", groupId);
		}

		[Test]
		public void Matches_IfDiscUriDoesNotMatchPattern_ReturnsFalse()
		{
			// Arrange

			var target = new GroupPattern
			{
				Pattern = "^/(SomeCategory)/(SomeArtist)",
				GroupId = "SomeGroupId",
			};

			// Act

			var result = target.Matches(new Uri("/AnotherCategory/SomeArtist/SomeAlbum", UriKind.Relative), out var _);

			// Assert

			Assert.IsFalse(result);
		}
	}
}
