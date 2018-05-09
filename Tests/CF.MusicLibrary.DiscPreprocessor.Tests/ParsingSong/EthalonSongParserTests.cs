using CF.MusicLibrary.DiscPreprocessor.ParsingSong;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.ParsingSong
{
	[TestFixture]
	public class EthalonSongParserTests
	{
		[Test]
		public void ParseSongTitle_ParsesDataCorrectly()
		{
			EthalonSongParser target = new EthalonSongParser();

			foreach (var pattern in EthalonSongParser.TitlePatterns)
			{
				CollectionAssert.IsNotEmpty(pattern.Tests);
				foreach (var test in pattern.Tests)
				{
					var resultTitle = target.ParseSongTitle(test.InputTitle);
					Assert.AreEqual(test.ExpectedTitle, resultTitle);
				}
			}
		}

		[TestCase("Test Title	live", "Test Title (Live)")]
		[TestCase("Test Title	featuring George Oosthoek", "Test Title (feat. George Oosthoek)")]
		[TestCase("Test Title	live, featuring Marco Hietala", "Test Title (feat. Marco Hietala) (Live)")]
		public void ParseSongTitle_ParsesPayloadDataCorrectly(string rawTitle, string expectedTitle)
		{
			EthalonSongParser target = new EthalonSongParser();

			string resultTitle = target.ParseSongTitle(rawTitle);

			Assert.AreEqual(expectedTitle, resultTitle);
		}

		[Test]
		public void ParseSongTitle_ForTitleInLatin_CapitalizesFirstLetters()
		{
			EthalonSongParser target = new EthalonSongParser();

			string resultTitle = target.ParseSongTitle("this is the life");

			Assert.AreEqual("This Is The Life", resultTitle);
		}

		[TestCase("RockStar")]
		[TestCase("Rock'n'Roll")]
		public void ParseSongTitle_ForTitleInLatin_DoesNotChangeCaseForInsideLetters(string title)
		{
			EthalonSongParser target = new EthalonSongParser();

			string resultTitle = target.ParseSongTitle(title);

			Assert.AreEqual(title, resultTitle);
		}

		[Test]
		public void ParseSongTitle_ForTitleInCyrillic_DoesNotCapitalizeFirstLetters()
		{
			EthalonSongParser target = new EthalonSongParser();

			string resultTitle = target.ParseSongTitle("Пачка сигарет");

			Assert.AreEqual("Пачка сигарет", resultTitle);
		}

		[TestCase("Don’t Say A Word", "Don't Say A Word")]
		public void ParseSongTitle_UnifiesApostropheSymbols(string rawTitle, string expectedTitle)
		{
			EthalonSongParser target = new EthalonSongParser();

			string resultTitle = target.ParseSongTitle(rawTitle);

			Assert.AreEqual(expectedTitle, resultTitle);
		}

		[TestCase("The 12th Hour", "The 12th Hour")]
		public void ParseSongTitle_ForOrdinalNumber_SetsCorrectCase(string rawTitle, string expectedTitle)
		{
			EthalonSongParser target = new EthalonSongParser();

			string resultTitle = target.ParseSongTitle(rawTitle);

			Assert.AreEqual(expectedTitle, resultTitle);
		}
	}
}
