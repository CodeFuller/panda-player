using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	/// <summary>
	/// Splits input content by chunks delimited by empty lines.
	/// </summary>
	internal interface IInputContentSplitter
	{
		IEnumerable<IReadOnlyCollection<string>> Split(IEnumerable<string> content);
	}
}
