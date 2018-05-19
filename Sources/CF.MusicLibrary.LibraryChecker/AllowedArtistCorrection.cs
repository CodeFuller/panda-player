using System.Text.RegularExpressions;

namespace CF.MusicLibrary.LibraryChecker
{
	public class AllowedArtistCorrection
	{
		public string Original { get; set; }

		public string Corrected { get; set; }

		public string CorrectArtistName(string originalArtistName)
		{
			return new Regex(Original).Replace(originalArtistName, Corrected);
		}
	}
}
