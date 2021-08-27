using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Grouping
{
	internal class AdviseGroupContent
	{
		private readonly List<AdviseSetContent> adviseSets = new();

		public string Id { get; }

		public IReadOnlyCollection<AdviseSetContent> AdviseSets => adviseSets;

		public bool IsDeleted => AdviseSets.All(x => x.IsDeleted);

		public AdviseGroupContent(string id)
		{
			Id = id;
		}

		public void AddDisc(DiscModel disc)
		{
			var adviseSet = new AdviseSetContent($"Disc: {disc.Id}");
			adviseSet.AddDisc(disc);

			adviseSets.Add(adviseSet);
		}
	}
}
