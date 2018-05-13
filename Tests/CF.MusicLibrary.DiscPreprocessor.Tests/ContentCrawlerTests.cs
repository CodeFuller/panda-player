﻿using System;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests
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