using CF.MusicLibrary.Tagger;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	[TestFixture]
	public class SongTaggerTests
	{
		[Test]
		public void SongTagger_DoesNotUseNumericGenres()
		{
			var tagger = new SongTagger();

			Assert.IsFalse(TagLib.Id3v2.Tag.UseNumericGenres);
		}
	}
}
