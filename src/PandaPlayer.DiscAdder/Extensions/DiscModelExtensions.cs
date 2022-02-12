using System.Linq;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;

namespace PandaPlayer.DiscAdder.Extensions
{
	internal static class DiscModelExtensions
	{
		public static GenreModel GetGenre(this DiscModel disc)
		{
			return (disc.IsDeleted ? disc.AllSongs : disc.ActiveSongs)
				.Select(s => s.Genre)
				.UniqueOrDefault(new GenreEqualityComparer());
		}
	}
}
