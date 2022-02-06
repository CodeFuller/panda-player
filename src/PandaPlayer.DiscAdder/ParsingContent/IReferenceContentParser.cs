using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal interface IReferenceContentParser
	{
		IEnumerable<ReferenceDiscContent> Parse(string rawReferenceContent);
	}
}
