using System.Collections.Generic;

namespace CF.MusicLibrary.DiscPreprocessor.ParsingContent
{
	/// <summary>
	/// Splits input content by chunks delimited by empty lines.
	/// </summary>
	public interface IInputContentSplitter
	{
		IEnumerable<IEnumerable<string>> Split(IEnumerable<string> content);
	}
}
