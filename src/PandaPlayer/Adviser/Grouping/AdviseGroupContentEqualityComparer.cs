using System;
using System.Collections.Generic;

namespace PandaPlayer.Adviser.Grouping
{
	internal class AdviseGroupContentEqualityComparer : IEqualityComparer<AdviseGroupContent>
	{
		public bool Equals(AdviseGroupContent x, AdviseGroupContent y)
		{
			if (x is null && y is null)
			{
				return true;
			}

			if (x is null || y is null)
			{
				return false;
			}

			return x.Id == y.Id;
		}

		public int GetHashCode(AdviseGroupContent obj)
		{
			return obj.Id.GetHashCode(StringComparison.Ordinal);
		}
	}
}
