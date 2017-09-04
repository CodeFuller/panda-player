using System.Collections.Generic;

namespace CF.MusicLibrary.DiscPreprocessor.ParsingContent
{
	public interface IDiscContentParser
	{
		IEnumerable<DiscContent> Parse(string content);
	}
}
