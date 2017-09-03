using CF.MusicLibrary.LibraryChecker;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.LibraryChecker
{
	[TestFixture]
	public class AlbumTitleCheckerTests
	{
		[TestCase("Broken Crown Halo (CD 1)")]
		[TestCase("Stolzes Herz (Single)")]
		[TestCase("Exordium (EP)")]
		[TestCase("Origin (Demo)")]
		[TestCase("Bualadh Bos (Live)")]
		[TestCase("See You On The Other Side (Bonus)")]
		[TestCase("Wishmastour (Bonus CD)")]
		[TestCase("Manifesto Of Lacuna Coil (Compilation)")]
		[TestCase("Vintage Classix (B-Sides)")]
		[TestCase("Комиксы (Remixes)")]
		[TestCase("Фиолетово-чёрный (Сборник)")]
		[TestCase("Отчёт 1983 - 1993 (Трибьют)")]
		[TestCase("Розыгрыш (Soundtrack)")]
		[TestCase("The Classical Conspiracy (Live) (CD 2)")]
		[TestCase("Rarities")]
		public void AlbumTitleIsSuspicious_ForSuspiciousTitle_ReturnsTrue(string albumTitle)
		{
			var isSuspicious = AlbumTitleChecker.AlbumTitleIsSuspicious(albumTitle);
			Assert.IsTrue(isSuspicious);
		}

		[TestCase("The Unquestionable Truth (Part 1)")]
		[TestCase("Don't Give Me Names")]
		public void AlbumTitleIsSuspicious_ForNonSuspiciousTitle_ReturnsFalse(string albumTitle)
		{
			var isSuspicious = AlbumTitleChecker.AlbumTitleIsSuspicious(albumTitle);
			Assert.IsFalse(isSuspicious);
		}
	}
}
