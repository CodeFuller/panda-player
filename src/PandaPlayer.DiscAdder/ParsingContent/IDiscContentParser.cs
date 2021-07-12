using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal interface IDiscContentParser
	{
		IEnumerable<DiscContent> Parse(string content);
	}
}
