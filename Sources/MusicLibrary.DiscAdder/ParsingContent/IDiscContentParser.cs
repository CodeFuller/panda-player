using System.Collections.Generic;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	public interface IDiscContentParser
	{
		IEnumerable<DiscContent> Parse(string content);
	}
}
