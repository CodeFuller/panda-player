using System;
using CF.MusicLibrary.DiscPreprocessor.ParsingContent;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.ParsingContent
{
	[TestFixture]
	public class DiscContentParserTests
	{
		[Test]
		public void Constructor_WhenInputContentSplitterIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscContentParser(null, Substitute.For<IEthalonDiscParser>()));
		}

		[Test]
		public void Constructor_WhenEthalonSongParserIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscContentParser(Substitute.For<IInputContentSplitter>(), null));
		}
	}
}
