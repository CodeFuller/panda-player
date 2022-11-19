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

		public bool IsFavorite { get; }

		public bool IsDeleted => AdviseSets.All(x => x.IsDeleted);

		public AdviseGroupContent(string id, bool isFavorite)
		{
			Id = id;
			IsFavorite = isFavorite;
		}

		public void AddDisc(DiscModel disc)
		{
			var adviseSetId = disc.AdviseSetInfo?.AdviseSet.Id.Value ?? $"Disc: {disc.Id}";

			var adviseSet = adviseSets.SingleOrDefault(x => x.Id == adviseSetId);
			if (adviseSet == null)
			{
				adviseSet = new AdviseSetContent(adviseSetId);
				adviseSets.Add(adviseSet);
			}

			adviseSet.AddDisc(disc);
		}
	}
}
