using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.Extensions
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<(TFirst First, TSecond Second)> OuterZip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
		{
			using var e1 = first.GetEnumerator();
			using var e2 = second.GetEnumerator();

			var firstFinished = false;
			var secondFinished = false;

			while (true)
			{
				firstFinished = firstFinished || !e1.MoveNext();
				secondFinished = secondFinished || !e2.MoveNext();

				if (firstFinished && secondFinished)
				{
					yield break;
				}

				yield return (firstFinished ? default : e1.Current, secondFinished ? default : e2.Current);
			}
		}
	}
}
