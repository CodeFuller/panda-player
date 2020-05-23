using System.Collections.Generic;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Comparers
{
	public abstract class ItemWithIdEqualityComparer<T> : IEqualityComparer<T>
	{
		protected abstract ItemId GetItemId(T item);

		public bool Equals(T x, T y)
		{
			if (x is null && y is null)
			{
				return true;
			}

			if (x is null || y is null)
			{
				return false;
			}

			return GetItemId(x) == GetItemId(y);
		}

		public int GetHashCode(T obj)
		{
			return GetItemId(obj).GetHashCode();
		}
	}
}
