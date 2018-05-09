using System;
using System.Linq;
using NUnit.Framework;

namespace CF.MusicLibrary.Core.Tests
{
	[TestFixture]
	public class ItemUriPartsTests
	{
		[Test]
		public void Join_ForNonEmptyParts_ReturnsCorrectUri()
		{
			Uri uri = ItemUriParts.Join(new[] {"part1", "part2"});
			Assert.AreEqual("/part1/part2", uri.ToString());
		}

		[Test]
		public void Join_ForEmptyParts_ReturnsCorrectUri()
		{
			Uri uri = ItemUriParts.Join(Enumerable.Empty<string>());
			Assert.AreEqual("/", uri.ToString());
		}

		[Test]
		public void IsBaseOf_ForDirectChildUri_ReturnsTrue()
		{
			//	Arrange

			var parts1 = new ItemUriParts(new Uri("/part1/part2", UriKind.Relative));
			var parts2 = new ItemUriParts(new Uri("/part1/part2/part3", UriKind.Relative));

			//	Act

			var result = parts1.IsBaseOf(parts2);

			//	Assert

			Assert.IsTrue(result);
		}

		[Test]
		public void IsBaseOf_ForNonDirectChildUri_ReturnsTrue()
		{
			//	Arrange

			var parts1 = new ItemUriParts(new Uri("/part1", UriKind.Relative));
			var parts2 = new ItemUriParts(new Uri("/part1/part2/part3", UriKind.Relative));

			//	Act

			var result = parts1.IsBaseOf(parts2);

			//	Assert

			Assert.IsTrue(result);
		}

		[Test]
		public void IsBaseOf_ForNonChildUri_ReturnsTrue()
		{
			//	Arrange

			var parts1 = new ItemUriParts(new Uri("/part0", UriKind.Relative));
			var parts2 = new ItemUriParts(new Uri("/part1/part2/part3", UriKind.Relative));

			//	Act

			var result = parts1.IsBaseOf(parts2);

			//	Assert

			Assert.IsFalse(result);
		}

		[Test]
		public void IsBaseOf_OnRootUri_ReturnsTrue()
		{
			//	Arrange

			var parts1 = new ItemUriParts(new Uri("/", UriKind.Relative));
			var parts2 = new ItemUriParts(new Uri("/part1", UriKind.Relative));

			//	Act

			var result = parts1.IsBaseOf(parts2);

			//	Assert

			Assert.IsTrue(result);
		}
	}
}
