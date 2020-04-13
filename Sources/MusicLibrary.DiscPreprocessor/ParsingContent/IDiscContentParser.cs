using System.Collections.Generic;

namespace MusicLibrary.DiscPreprocessor.ParsingContent
{
	public interface IDiscContentParser
	{
		IEnumerable<DiscContent> Parse(string content);
	}
}
