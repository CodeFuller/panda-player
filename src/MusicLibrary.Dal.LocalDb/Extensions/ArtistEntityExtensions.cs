using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class ArtistEntityExtensions
	{
		public static ArtistModel ToModel(this ArtistEntity artist)
		{
			return new ArtistModel
			{
				Id = artist.Id.ToItemId(),
				Name = artist.Name,
			};
		}

		public static ArtistEntity ToEntity(this ArtistModel artist)
		{
			return new ArtistEntity
			{
				Id = artist.Id?.ToInt32() ?? default,
				Name = artist.Name,
			};
		}
	}
}
