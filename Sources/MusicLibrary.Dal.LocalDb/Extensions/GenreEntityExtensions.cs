using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class GenreEntityExtensions
	{
		public static GenreModel ToModel(this GenreEntity artist)
		{
			return new GenreModel
			{
				Id = artist.Id.ToItemId(),
				Name = artist.Name,
			};
		}
	}
}
