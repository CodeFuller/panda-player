using System.Collections.Generic;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public interface IDiscCrawler
	{
		IEnumerable<DiscContent> LoadDiscs(string discsDirectory);
	}
}
