using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.DiscPreprocessor
{
	public class DiscContent
	{
		public string DiscDirectory { get; set; }

		public Collection<SongContent> Songs { get; }

		public DiscContent(string discDirectory, IEnumerable<string> songContent)
		{
			DiscDirectory = discDirectory;
			Songs = songContent.Select(s => new SongContent(s)).ToCollection();
		}
	}
}
