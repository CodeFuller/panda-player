using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicLibrary.PandaPlayer.Settings
{
	public class PandaPlayerSettings
	{
		public ICollection<string> DiscCoverImageLookupPages { get; } = new Collection<string>();
	}
}
