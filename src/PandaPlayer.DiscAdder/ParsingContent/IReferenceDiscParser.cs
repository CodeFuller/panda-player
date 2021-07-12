using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal interface IReferenceDiscParser
	{
		DiscContent Parse(IEnumerable<string> discContent);
	}
}
