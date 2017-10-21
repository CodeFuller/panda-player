using System.Linq;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.DiscPreprocessor;
using CF.MusicLibrary.DiscPreprocessor.ParsingContent;
using CF.MusicLibrary.DiscPreprocessor.ParsingSong;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.ParsingContent
{
	[TestFixture]
	public class EthalonDiscParserTests
	{
		[Test]
		public void Parse_ForEmptyContent_ThrowsInvalidInputDataException()
		{
			EthalonDiscParser target = new EthalonDiscParser(Substitute.For<IEthalonSongParser>());

			Assert.Throws<InvalidInputDataException>(() => target.Parse(Enumerable.Empty<string>()));
		}

		[Test]
		public void Parse_ForValidContent_ReturnsCorrectDiscContent()
		{
			//	Arrange

			string[] content =
			{
				"DiscDirectory",
				"DiscSongContent1",
				"DiscSongContent2",
			};

			IEthalonSongParser songParserStub = Substitute.For<IEthalonSongParser>();
			songParserStub.ParseSongTitle("DiscSongContent1").Returns("Song1");
			songParserStub.ParseSongTitle("DiscSongContent2").Returns("Song2");
			EthalonDiscParser target = new EthalonDiscParser(songParserStub);

			//	Act

			DiscContent disc = target.Parse(content);

			//	Assert

			Assert.AreEqual("DiscDirectory", disc.DiscDirectory);
			var songs = disc.Songs.ToList();
			Assert.AreEqual(2, songs.Count);
			Assert.AreEqual("Song1", songs[0].Title);
			Assert.AreEqual("Song2", songs[1].Title);
		}
	}
}
