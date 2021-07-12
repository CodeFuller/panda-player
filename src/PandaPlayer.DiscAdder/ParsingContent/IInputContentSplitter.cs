using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	/// <summary>
	/// Splits input content by chunks delimited by empty lines.
	/// </summary>
	internal interface IInputContentSplitter
	{
		IEnumerable<IEnumerable<string>> Split(IEnumerable<string> content);
	}
}
