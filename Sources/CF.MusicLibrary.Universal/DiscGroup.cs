using System.Collections.ObjectModel;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.Universal
{
	public class DiscGroup
	{
		public string Id { get; }

		public string Title { get; }

		public Collection<Disc> Discs { get; } = new Collection<Disc>();

		public DiscGroup(string idAndName)
			: this(idAndName, idAndName)
		{
		}

		public DiscGroup(string id, string title)
		{
			Id = id;
			Title = title;
		}
	}
}
