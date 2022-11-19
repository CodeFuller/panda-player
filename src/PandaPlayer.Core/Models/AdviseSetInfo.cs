using System;

namespace PandaPlayer.Core.Models
{
	public class AdviseSetInfo
	{
		public AdviseSetModel AdviseSet { get; }

		public int Order { get; }

		public AdviseSetInfo(AdviseSetModel adviseSet, int order)
		{
			AdviseSet = adviseSet ?? throw new ArgumentNullException(nameof(adviseSet));
			Order = order;
		}

		public AdviseSetInfo WithOrder(int newOrder)
		{
			return new(AdviseSet, newOrder);
		}
	}
}
