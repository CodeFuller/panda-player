using System.Collections.Generic;

namespace MusicLibrary.DiscAdder.Interfaces
{
	internal interface IContentCrawler
	{
		IEnumerable<DiscContent> LoadDiscs(string discsDirectory);

		IEnumerable<string> LoadDiscImages(string discDirectory);
	}
}
