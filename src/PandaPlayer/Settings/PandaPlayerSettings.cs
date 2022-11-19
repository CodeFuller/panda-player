using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PandaPlayer.Settings
{
	public class PandaPlayerSettings
	{
		public ICollection<string> DiscCoverImageLookupPages { get; } = new Collection<string>();
	}
}
