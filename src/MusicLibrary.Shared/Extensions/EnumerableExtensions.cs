using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicLibrary.Shared.Extensions
{
	public static class EnumerableExtensions
	{
		public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			return new Collection<T>(source.ToList());
		}
	}
}
