using System.Collections.Generic;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public interface IContentCrawler
	{
		IEnumerable<DiscContent> LoadDiscs(string discsDirectory);

		IEnumerable<string> LoadDiscImages(string discDirectory);
	}
}
