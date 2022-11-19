using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.Core.Extensions
{
	public static class EnumerableExtensions
	{
		public static TSource UniqueOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.UniqueOrDefault(EqualityComparer<TSource>.Default);
		}

		public static TSource UniqueOrDefault<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			var sourceList = source.ToList();
			var distinctValues = sourceList.Distinct(comparer);
			return distinctValues.Count() <= 1 ? sourceList.FirstOrDefault() : default;
		}
	}
}
