using System.Collections.Generic;

namespace CF.MusicLibrary.DiscPreprocessor.ParsingContent
{
	public interface IEthalonDiscParser
	{
		DiscContent Parse(IEnumerable<string> discContent);
	}
}
