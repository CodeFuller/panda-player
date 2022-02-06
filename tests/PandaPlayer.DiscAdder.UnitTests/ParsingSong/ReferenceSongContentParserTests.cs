using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.DiscAdder.ParsingSong;

namespace PandaPlayer.DiscAdder.UnitTests.ParsingSong
{
	[TestClass]
	public class ReferenceSongContentParserTests
	{
		[TestMethod]
		public void Parse_ForSupportedTitlePattern_ReturnsCorrectReferenceSongContent()
		{
			var target = new ReferenceSongContentParser();

			foreach (var pattern in ReferenceSongContentParser.TitlePatterns)
			{
				foreach (var testCase in pattern.TestCases)
				{
					var resultContent = target.Parse(1, testCase.RawContent);

					var expectedContent = new ReferenceSongContent(1, testCase.ExpectedTitle);

					resultContent.Should().BeEquivalentTo(expectedContent);
				}
			}
		}

		[DataTestMethod]
		[DataRow("Test Title	live", "Test Title (Live)")]
		[DataRow("Test Title	featuring George Oosthoek", "Test Title (feat. George Oosthoek)")]
		[DataRow("Test Title	live, featuring Marco Hietala", "Test Title (feat. Marco Hietala) (Live)")]
		public void Parse_ForTitleWithPayloadData_ParsesPayloadDataCorrectly(string rawContent, string expectedTitle)
		{
			// Arrange

			var target = new ReferenceSongContentParser();

			// Act

			var resultContent = target.Parse(1, rawContent);

			// Assert

			var expectedContent = new ReferenceSongContent(1, expectedTitle);

			resultContent.Should().BeEquivalentTo(expectedContent);
		}

		[TestMethod]
		public void Parse_ForTitleInLatin_CapitalizesFirstLetters()
		{
			// Arrange

			var target = new ReferenceSongContentParser();

			// Act

			var resultContent = target.Parse(1, "this is the life");

			// Assert

			var expectedContent = new ReferenceSongContent(1, "This Is The Life");

			resultContent.Should().BeEquivalentTo(expectedContent);
		}

		[DataTestMethod]
		[DataRow("RockStar")]
		[DataRow("Rock'n'Roll")]
		public void Parse_ForTitleInLatin_DoesNotChangeCaseForInsideLetters(string rawContent)
		{
			// Arrange

			var target = new ReferenceSongContentParser();

			// Act

			var resultContent = target.Parse(1, rawContent);

			// Assert

			var expectedContent = new ReferenceSongContent(1, rawContent);

			resultContent.Should().BeEquivalentTo(expectedContent);
		}

		[DataTestMethod]
		[DataRow("The 12th Hour", "The 12th Hour")]
		public void Parse_ForOrdinalNumber_SetsCorrectCase(string rawContent, string expectedTitle)
		{
			// Arrange

			var target = new ReferenceSongContentParser();

			// Act

			var resultContent = target.Parse(1, rawContent);

			// Assert

			var expectedContent = new ReferenceSongContent(1, expectedTitle);

			resultContent.Should().BeEquivalentTo(expectedContent);
		}

		[TestMethod]
		public void Parse_ForTitleInCyrillic_DoesNotCapitalizeFirstLetters()
		{
			// Arrange

			var target = new ReferenceSongContentParser();

			// Act

			var resultContent = target.Parse(1, "Пачка сигарет");

			// Assert

			var expectedContent = new ReferenceSongContent(1, "Пачка сигарет");

			resultContent.Should().BeEquivalentTo(expectedContent);
		}

		[DataTestMethod]
		[DataRow("Don’t Say A Word", "Don't Say A Word")]
		public void Parse_ForTitleWithApostropheCharacters_UnifiesApostropheCharacters(string rawContent, string expectedTitle)
		{
			// Arrange

			var target = new ReferenceSongContentParser();

			// Act

			var resultContent = target.Parse(1, rawContent);

			// Assert

			var expectedContent = new ReferenceSongContent(1, expectedTitle);

			resultContent.Should().BeEquivalentTo(expectedContent);
		}
	}
}
