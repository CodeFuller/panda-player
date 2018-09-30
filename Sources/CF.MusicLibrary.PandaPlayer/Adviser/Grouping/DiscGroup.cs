using System.Collections.ObjectModel;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.Grouping
{
	public class DiscGroup
	{
		public string Id { get; }

		public Collection<Disc> Discs { get; } = new Collection<Disc>();

		public DiscGroup(string id)
		{
			Id = id;
		}
	}
}
