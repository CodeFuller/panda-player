using MusicLibrary.Core.Objects;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class ArtistExtensions
	{
		public static ArtistModel ToModel(this Artist artist)
		{
			return new ArtistModel
			{
				Id = artist.Id.ToItemId(),
				Name = artist.Name,
			};
		}
	}
}
