using System.Linq;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Extensions;
using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.Extensions
{
	internal static class DiscModelExtensions
	{
		public static GenreModel GetGenre(this DiscModel disc)
		{
			return (disc.IsDeleted ? disc.AllSongs : disc.ActiveSongs).Select(s => s.Genre)
				.UniqueOrDefault(new GenreEqualityComparer());
		}
	}
}
