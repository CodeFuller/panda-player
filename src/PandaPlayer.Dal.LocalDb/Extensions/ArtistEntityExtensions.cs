using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class ArtistEntityExtensions
	{
		public static ArtistModel ToModel(this ArtistEntity artist)
		{
			return new()
			{
				Id = artist.Id.ToItemId(),
				Name = artist.Name,
			};
		}

		public static ArtistEntity ToEntity(this ArtistModel artist)
		{
			return new()
			{
				Id = artist.Id?.ToInt32() ?? default,
				Name = artist.Name,
			};
		}
	}
}
