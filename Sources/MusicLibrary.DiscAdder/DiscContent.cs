using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.DiscAdder
{
	public class DiscContent
	{
		public string DiscDirectory { get; set; }

		public IReadOnlyCollection<SongContent> Songs { get; }

		public DiscContent(string discDirectory, IEnumerable<string> songFiles)
		{
			DiscDirectory = discDirectory;
			Songs = songFiles.Select(s => new SongContent(s)).ToList();
		}
	}
}
