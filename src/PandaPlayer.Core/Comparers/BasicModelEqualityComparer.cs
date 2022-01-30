using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public abstract class BasicModelEqualityComparer<TModel> : IEqualityComparer<TModel>
		where TModel : BasicModel
	{
		public bool Equals(TModel x, TModel y)
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

		public int GetHashCode(TModel obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
