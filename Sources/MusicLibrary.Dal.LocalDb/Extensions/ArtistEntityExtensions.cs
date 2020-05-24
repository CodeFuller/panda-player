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
	}
}
