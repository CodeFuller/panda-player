using System.Linq;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.AlbumPreprocessor;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	[TestFixture]
	public class EthalonAlbumParserTests
	{
		[Test]
		public void Parse_ForEmptyContent_ThrowsInvalidInputDataException()
		{
			EthalonAlbumParser target = new EthalonAlbumParser(Substitute.For<IEthalonSongParser>());

			Assert.Throws<InvalidInputDataException>(() => target.Parse(Enumerable.Empty<string>()));
		}

		[Test]
		public void Parse_ForValidContent_ReturnsCorrectAlbum()
		{
			//	Arrange

			string[] content =
			{
				"AlbumDirectory",
				"AlbumSongContent1",
				"AlbumSongContent2",
			};

			IEthalonSongParser songParserStub = Substitute.For<IEthalonSongParser>();
			songParserStub.ParseSongTitle("AlbumSongContent1").Returns("Song1");
			songParserStub.ParseSongTitle("AlbumSongContent2").Returns("Song2");
			EthalonAlbumParser target = new EthalonAlbumParser(songParserStub);

			//	Act

			AlbumContent album = target.Parse(content);

			//	Assert

			Assert.AreEqual("AlbumDirectory", album.AlbumDirectory);
			Assert.AreEqual(2, album.Songs.Count);
			Assert.AreEqual("Song1", album.Songs[0].Title);
			Assert.AreEqual("Song2", album.Songs[1].Title);
		}
	}
}
