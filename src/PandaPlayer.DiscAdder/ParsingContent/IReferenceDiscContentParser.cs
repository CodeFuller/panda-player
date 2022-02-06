using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal interface IReferenceDiscContentParser
	{
		ReferenceDiscContent Parse(IEnumerable<string> rawReferenceDiscContent);
	}
}
