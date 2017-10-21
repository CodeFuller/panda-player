using System;
using CF.MusicLibrary.DiscPreprocessor;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor
{
	[TestFixture]
	public class ContentCrawlerTests
	{
		[Test]
		public void Constructor_IfSourceFileTypeResolverArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ContentCrawler(null));
		}
	}
}
