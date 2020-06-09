using System.Collections.Generic;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	public interface IEthalonDiscParser
	{
		DiscContent Parse(IEnumerable<string> discContent);
	}
}
