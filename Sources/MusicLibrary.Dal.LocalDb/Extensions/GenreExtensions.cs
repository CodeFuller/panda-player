using MusicLibrary.Core.Objects;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class GenreExtensions
	{
		public static GenreModel ToModel(this Genre genre)
		{
			return new GenreModel
			{
				Id = genre.Id.ToItemId(),
				Name = genre.Name,
			};
		}
	}
}
