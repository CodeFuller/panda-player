using System.Collections.Generic;

namespace MusicLibrary.Services.Settings
{
	public class DiscToAlbumMappingSettings
	{
		public IReadOnlyCollection<string> AlbumTitlePatterns { get; set; }

		public IReadOnlyCollection<string> EmptyAlbumTitlePatterns { get; set; }
	}
}
