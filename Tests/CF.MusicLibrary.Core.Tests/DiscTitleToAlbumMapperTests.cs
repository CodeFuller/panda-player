using CF.MusicLibrary.Tests;
using NUnit.Framework;

namespace CF.MusicLibrary.Core.Tests
{
	[TestFixture]
	public class DiscTitleToAlbumMapperTests
	{
		[Test]
		public void GetAlbumTitleFromDiscTitle_IfDicsTitleMatchesSomePattern_ReturnsCorrectAlbumTitle()
		{
			// Arrange

			var settings = new DiscToAlbumMappingSettings
			{
				AlbumTitlePatterns =
				{
					@"^(.+) \(CD ?\d+\)$",
				},
			};

			var target = new DiscTitleToAlbumMapper(settings.StubOptions());

			// Act

			var albumTitle = target.GetAlbumTitleFromDiscTitle("Broken Crown Halo (CD 1)");

			// Assert

			Assert.AreEqual("Broken Crown Halo", albumTitle);
		}

		[Test]
		public void GetAlbumTitleFromDiscTitle_IfDicsTitleMatchesMultiplePatterns_ReturnsCorrectAlbumTitle()
		{
			// Arrange

			var settings = new DiscToAlbumMappingSettings
			{
				AlbumTitlePatterns =
				{
					@"^(.+) \(CD ?\d+\)$",
					@"^(.+) \(Live\)$",
				},
			};

			var target = new DiscTitleToAlbumMapper(settings.StubOptions());

			// Act

			var albumTitle = target.GetAlbumTitleFromDiscTitle(@"The Classical Conspiracy (Live) (CD 2)");

			// Assert

			Assert.AreEqual("The Classical Conspiracy", albumTitle);
		}

		[TestCase("The Unquestionable Truth (Part 1)")]
		[TestCase("Don't Give Me Names")]
		public void GetAlbumTitleFromDiscTitle_IfDicsTitleDoesNotMatchAnyPattern_ReturnsSameTitle(string discTitle)
		{
			// Arrange

			var settings = new DiscToAlbumMappingSettings
			{
				AlbumTitlePatterns =
				{
					@"^(.+) \(CD ?\d+\)$",
					@"^(.+) \(Live\)$",
				},
			};

			var target = new DiscTitleToAlbumMapper(settings.StubOptions());

			// Act

			var albumTitle = target.GetAlbumTitleFromDiscTitle(discTitle);

			// Assert

			Assert.AreEqual(discTitle, albumTitle);
		}

		[Test]
		public void GetAlbumTitleFromDiscTitle_IfDicsTitleMatchesSomeEmptyAlbumTitlePattern_ReturnsNull()
		{
			// Arrange

			var settings = new DiscToAlbumMappingSettings
			{
				EmptyAlbumTitlePatterns =
				{
					@"^Different$",
				},
			};

			var target = new DiscTitleToAlbumMapper(settings.StubOptions());

			// Act

			var albumTitle = target.GetAlbumTitleFromDiscTitle("Different");

			// Assert

			Assert.IsNull(albumTitle);
		}

		[Test]
		public void GetAlbumTitleFromDiscTitle_IfCorrectedAlbumTitleMatchesSomeEmptyAlbumTitlePattern_ReturnsNull()
		{
			// Arrange

			var settings = new DiscToAlbumMappingSettings
			{
				AlbumTitlePatterns =
				{
					@"^(.+) \(CD ?\d+\)$",
				},

				EmptyAlbumTitlePatterns =
				{
					@"^Different$",
				},
			};

			var target = new DiscTitleToAlbumMapper(settings.StubOptions());

			// Act

			var albumTitle = target.GetAlbumTitleFromDiscTitle("Different (CD 1)");

			// Assert

			Assert.IsNull(albumTitle);
		}

		[Test]
		public void GetAlbumTitleFromDiscTitle_IfDiscTitleIsNull_ReturnsNull()
		{
			// Arrange

			var target = new DiscTitleToAlbumMapper(new DiscToAlbumMappingSettings().StubOptions());

			// Act

			var albumTitle = target.GetAlbumTitleFromDiscTitle(null);

			// Assert

			Assert.IsNull(albumTitle);
		}

		[TestCase("Broken Crown Halo (CD 1)", "Broken Crown Halo")]
		[TestCase("Broken Crown Halo (CD1)", "Broken Crown Halo")]
		[TestCase("Stolzes Herz (Single)", "Stolzes Herz")]
		[TestCase("Exordium (EP)", "Exordium")]
		[TestCase("Origin (Demo)", "Origin")]
		[TestCase("Bualadh Bos (Live)", "Bualadh Bos")]
		[TestCase("See You On The Other Side (Bonus)", "See You On The Other Side")]
		[TestCase("Wishmastour (Bonus CD)", "Wishmastour")]
		[TestCase("Manifesto Of Lacuna Coil (Compilation)", "Manifesto Of Lacuna Coil")]
		[TestCase("Vintage Classix (B-Sides)", "Vintage Classix")]
		[TestCase("The Matrix (Soundtrack)", "The Matrix")]
		[TestCase("The Classical Conspiracy (Live) (CD 2)", "The Classical Conspiracy")]
		public void GetAlbumTitleFromDiscTitle_ForCommonPatterns_ReturnsCorrectAlbumTitle(string discTitle, string expectedAlbumTitle)
		{
			// Arrange

			var settings = new DiscToAlbumMappingSettings
			{
				AlbumTitlePatterns =
				{
					@"^(.+) \(CD ?\d+\)$",
					@"^(.+) \((?:(?:Single)|(?:EP))\)$",
					@"^(.+) \(Demo\)$",
					@"^(.+) \(Live\)$",
					@"^(.+) \(Bonus(?: CD)?\)$",
					@"^(.+) \(Compilation\)$",
					@"^(.+) \(B-Sides\)$",
					@"^(.+) \(Remixes\)$",
					@"^(.+) \(Soundtrack\)$",
				},
			};

			var target = new DiscTitleToAlbumMapper(settings.StubOptions());

			// Act

			var albumTitle = target.GetAlbumTitleFromDiscTitle(discTitle);

			// Assert

			Assert.AreEqual(expectedAlbumTitle, albumTitle);
		}

		[Test]
		public void AlbumTitleIsSuspicious_IfDiscAndAlbumTitlesDiffer_ReturnsTrue()
		{
			// Arrange

			var settings = new DiscToAlbumMappingSettings
			{
				AlbumTitlePatterns =
				{
					@"^(.+) \(CD ?\d+\)$",
				},
			};

			var target = new DiscTitleToAlbumMapper(settings.StubOptions());

			// Act

			var result = target.AlbumTitleIsSuspicious("Broken Crown Halo (CD 1)");

			// Assert

			Assert.IsTrue(result);
		}

		[Test]
		public void AlbumTitleIsSuspicious_IfDiscAndAlbumTitlesMatch_ReturnsFalse()
		{
			// Arrange

			var target = new DiscTitleToAlbumMapper(new DiscToAlbumMappingSettings().StubOptions());

			// Act

			var result = target.AlbumTitleIsSuspicious("Dawn Of The New Athens");

			// Assert

			Assert.IsFalse(result);
		}

		[Test]
		public void AlbumTitleIsSuspicious_IfAlbumTitleIsNull_ReturnsFalse()
		{
			// Arrange

			var target = new DiscTitleToAlbumMapper(new DiscToAlbumMappingSettings().StubOptions());

			// Act

			var result = target.AlbumTitleIsSuspicious(null);

			// Assert

			Assert.IsFalse(result);
		}
	}
}
