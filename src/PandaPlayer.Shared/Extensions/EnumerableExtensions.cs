using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PandaPlayer.Shared.Extensions
{
	public static class EnumerableExtensions
	{
		public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
		{
			ArgumentNullException.ThrowIfNull(source);

			return new Collection<T>(source.ToList());
		}
	}
}
