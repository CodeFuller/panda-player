using System.Collections.Generic;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	internal interface IReferenceDiscParser
	{
		DiscContent Parse(IEnumerable<string> discContent);
	}
}
