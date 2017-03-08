using System;
using CF.MusicLibrary.AlbumPreprocessor;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor
{
	[TestFixture]
	class AlbumContentParserTests
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
