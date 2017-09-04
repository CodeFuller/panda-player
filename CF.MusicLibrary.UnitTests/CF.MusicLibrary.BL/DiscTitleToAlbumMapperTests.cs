using CF.MusicLibrary.BL;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.BL
{
	[TestFixture]
	public class DiscTitleToAlbumMapperTests
	{
		[TestCase("Broken Crown Halo (CD 1)", "Broken Crown Halo")]
		[TestCase("Stolzes Herz (Single)", "Stolzes Herz")]
		[TestCase("Exordium (EP)", "Exordium")]
		[TestCase("Origin (Demo)", "Origin")]
		[TestCase("Bualadh Bos (Live)", "Bualadh Bos")]
		[TestCase("See You On The Other Side (Bonus)", "See You On The Other Side")]
		[TestCase("Wishmastour (Bonus CD)", "Wishmastour")]
		[TestCase("Manifesto Of Lacuna Coil (Compilation)", "Manifesto Of Lacuna Coil")]
		[TestCase("Vintage Classix (B-Sides)", "Vintage Classix")]
		[TestCase("Комиксы (Remixes)", "Комиксы")]
		[TestCase("Фиолетово-чёрный (Сборник)", "Фиолетово-чёрный")]
		[TestCase("Отчёт 1983 - 1993 (Трибьют)", "Отчёт 1983 - 1993")]
		[TestCase("Розыгрыш (Soundtrack)", "Розыгрыш")]
		[TestCase("The Classical Conspiracy (Live) (CD 2)", "The Classical Conspiracy")]
		[TestCase("Rarities", null)]
		public void GetAlbumTitleFromDiscTitle_WhenCorrectionIsRequired_ReturnsCorrectAlbumTitle(string discTitle, string expectedAlbumTitle)
		{
			string albumTitle = DiscTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(discTitle);
			Assert.AreEqual(expectedAlbumTitle, albumTitle);
		}

		[TestCase("The Unquestionable Truth (Part 1)")]
		[TestCase("Don't Give Me Names")]
		public void GetAlbumTitleFromDiscTitle_WhenCorrectionIsNotRequired_ReturnsOriginalDiscTitle(string discTitle)
		{
			string albumTitle = DiscTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(discTitle);
			Assert.AreEqual(discTitle, albumTitle);
		}
	}
}
