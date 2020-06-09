using System.Collections.Generic;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	internal interface IDiscContentParser
	{
		IEnumerable<DiscContent> Parse(string content);
	}
}
