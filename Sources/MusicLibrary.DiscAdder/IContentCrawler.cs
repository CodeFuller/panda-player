using System.Collections.Generic;

namespace MusicLibrary.DiscAdder
{
	internal interface IContentCrawler
	{
		IEnumerable<DiscContent> LoadDiscs(string discsDirectory);

		IEnumerable<string> LoadDiscImages(string discDirectory);
	}
}
