using System.Collections.ObjectModel;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser.Grouping
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
