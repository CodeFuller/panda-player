using System.Collections.Generic;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	internal interface IEthalonDiscParser
	{
		DiscContent Parse(IEnumerable<string> discContent);
	}
}
