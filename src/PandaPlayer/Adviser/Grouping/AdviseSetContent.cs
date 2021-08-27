using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Adviser.Extensions;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Grouping
{
	internal class AdviseSetContent
	{
		private readonly List<DiscModel> discs = new();

		public string Id { get; }

		// Deleted discs are also included.
		public IReadOnlyCollection<DiscModel> Discs => discs;

		public bool IsDeleted => Discs.All(x => x.IsDeleted);

		public DateTimeOffset? LastPlaybackTime
		{
			get
			{
				var lastPlaybacks = Discs.Select(x => x.GetLastPlaybackTime()).ToList();
				return lastPlaybacks.Any(x => x == null) ? null : lastPlaybacks.Select(x => x).Min();
			}
		}

		public AdviseSetContent(string id)
		{
			Id = id;
		}

		public void AddDisc(DiscModel disc)
		{
			discs.Add(disc);
		}
	}
}
