using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicLibrary.Core
{
	public class DiscToAlbumMappingSettings
	{
		public ICollection<string> AlbumTitlePatterns { get; } = new Collection<string>();

		public ICollection<string> EmptyAlbumTitlePatterns { get; } = new Collection<string>();
	}
}
