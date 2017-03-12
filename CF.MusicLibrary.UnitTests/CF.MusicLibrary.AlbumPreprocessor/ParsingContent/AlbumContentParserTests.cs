using System;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	[TestFixture]
	public class AlbumContentParserTests
	{
		[Test]
		public void Constructor_WhenInputContentSplitterIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AlbumContentParser(null, Substitute.For<IEthalonAlbumParser>()));
		}

		[Test]
		public void Constructor_WhenEthalonSongParserIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AlbumContentParser(Substitute.For<IInputContentSplitter>(), null));
		}
	}
}
