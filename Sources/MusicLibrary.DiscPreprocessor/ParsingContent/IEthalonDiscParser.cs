using System.Collections.Generic;

namespace MusicLibrary.DiscPreprocessor.ParsingContent
{
	public interface IEthalonDiscParser
	{
		DiscContent Parse(IEnumerable<string> discContent);
	}
}
