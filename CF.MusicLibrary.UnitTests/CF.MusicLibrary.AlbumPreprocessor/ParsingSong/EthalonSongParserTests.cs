﻿using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.ParsingSong
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

		[TestCase("this is the life", "This Is The Life")]
		public void ParseSongTitle_CapitalizesFirstLetters(string rawTitle, string expectedTitle)
		{
			EthalonSongParser target = new EthalonSongParser();

			string resultTitle = target.ParseSongTitle(rawTitle);

			Assert.AreEqual(expectedTitle, resultTitle);
		}
	}
}
