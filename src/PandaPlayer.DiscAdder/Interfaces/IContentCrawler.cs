using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.Interfaces
{
	internal interface IContentCrawler
	{
		IEnumerable<ActualDiscContent> LoadDiscs(string discsDirectory);

		IEnumerable<string> LoadDiscImages(string discDirectory);
	}
}
