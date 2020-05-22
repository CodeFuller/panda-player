using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.PandaPlayer.Extensions
{
	internal static class ReadOnlyCollectionExtensions
	{
		public static T UniqueOrDefault<T, TKey>(this IReadOnlyCollection<T> source, Func<T, TKey> keySelector)
			where T : class
		{
			var distinctValues = source.Select(keySelector).Distinct();
			return (distinctValues.Count() <= 1) ? source.FirstOrDefault() : null;
		}
	}
}
