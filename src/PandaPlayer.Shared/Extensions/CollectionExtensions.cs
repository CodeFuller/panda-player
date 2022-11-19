using System.Collections.Generic;

namespace PandaPlayer.Shared.Extensions
{
	public static class CollectionExtensions
	{
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
		{
			foreach (var item in newItems)
			{
				collection.Add(item);
			}
		}
	}
}
