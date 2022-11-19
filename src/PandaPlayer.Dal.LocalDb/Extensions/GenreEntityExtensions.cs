using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
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
