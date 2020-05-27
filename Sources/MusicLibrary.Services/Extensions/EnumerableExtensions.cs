using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.Services.Extensions
{
	public static class EnumerableExtensions
	{
		public static TSource UniqueOrDefault<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			var sourceList = source.ToList();
			var distinctValues = sourceList.Distinct(comparer);
			return distinctValues.Count() <= 1 ? sourceList.FirstOrDefault() : default;
		}
	}
}
