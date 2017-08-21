using CF.MusicLibrary.PandaPlayer.Scrobbler;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.Scrobbler
{
	[TestFixture]
	public class TrackTests
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
		[TestCase("The Unquestionable Truth (Part 1)", "The Unquestionable Truth (Part 1)")]
		public void AlbumSet_ForAlbumTitleWithAdditionalInfo_AdjustsAlbumTitleCorrectly(string rawTitle, string expectedAdjustedTitle)
		{
			var track = new Track
			{
				Album = rawTitle
			};

			Assert.AreEqual(expectedAdjustedTitle, track.Album);
		}
	}
}
