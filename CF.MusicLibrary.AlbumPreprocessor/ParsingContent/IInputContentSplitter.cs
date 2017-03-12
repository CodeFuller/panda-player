using System.Collections.Generic;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	/// <summary>
	/// Splits input content by chunks delimited by empty lines.
	/// </summary>
	public interface IInputContentSplitter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Such return type is required for the method.")]
		IEnumerable<IEnumerable<string>> Split(IEnumerable<string> content);
	}
}
