using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public class AlbumContent
	{
		public string AlbumDirectory { get; set; }

		public Collection<SongContent> Songs { get; }

		public AlbumContent(string albumDirectory, IEnumerable<string> songContent)
		{
			AlbumDirectory = albumDirectory;
			Songs = songContent.Select(s => new SongContent(s)).ToCollection();
		}
	}
}
