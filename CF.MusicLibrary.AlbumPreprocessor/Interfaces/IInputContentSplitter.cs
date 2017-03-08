using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.MusicLibrary.AlbumPreprocessor.Interfaces
{
	/// <summary>
	/// Splits input content by chunks delimited by empty lines.
	/// </summary>
	public interface IInputContentSplitter
	{
		IEnumerable<IEnumerable<string>> Split(IEnumerable<string> content);
	}
}
