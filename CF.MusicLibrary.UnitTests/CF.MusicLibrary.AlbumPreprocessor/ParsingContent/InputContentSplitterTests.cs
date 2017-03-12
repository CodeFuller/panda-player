using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	[TestFixture]
	public class InputContentSplitterTests
	{
		[Test]
		public void Split_ForEmptyInput_ReturnsNoContent()
		{
			InputContentSplitter target = new InputContentSplitter();

			var content = target.Split(Enumerable.Empty<string>());

			CollectionAssert.IsEmpty(content);
		}

		[Test]
		public void Split_ForContentWithOneChunk_ReturnsCorrectContent()
		{
			//	Arrange

			string[] inputContent =
			{
				"Data1",
				"Data2",
			};

			InputContentSplitter target = new InputContentSplitter();

			//	Act

			var content = target.Split(inputContent).ToList();

			//	Assert

			Assert.AreEqual(1, content.Count);
			CollectionAssert.AreEqual(inputContent, content[0]);
		}

		[Test]
		public void Split_ForContentWithSeveralChunks_ReturnsCorrectContent()
		{
			//	Arrange

			string[] inputContent =
			{
				"Data_11",
				"Data_12",
				"",
				"Data_21",
				"Data_22",
			};

			InputContentSplitter target = new InputContentSplitter();

			//	Act

			var content = target.Split(inputContent).ToList();

			//	Assert

			Assert.AreEqual(2, content.Count);
			CollectionAssert.AreEqual(new[] { "Data_11", "Data_12" }, content[0]);
			CollectionAssert.AreEqual(new[] { "Data_21", "Data_22" }, content[1]);
		}

		[Test]
		public void Split_ForContentWithExtraEmptyLines_SkipsExtraEmptyLines()
		{
			//	Arrange

			string[] inputContent =
			{
				"",
				"Data_1",
				"",
				"",
				"Data_2",
				"",
			};

			InputContentSplitter target = new InputContentSplitter();

			//	Act

			var content = target.Split(inputContent).ToList();

			//	Assert

			Assert.AreEqual(2, content.Count);
			CollectionAssert.AreEqual(new[] { "Data_1" }, content[0]);
			CollectionAssert.AreEqual(new[] { "Data_2" }, content[1]);
		}
	}
}
