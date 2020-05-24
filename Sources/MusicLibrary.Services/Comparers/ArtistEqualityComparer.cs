using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Comparers
{
	public class ArtistEqualityComparer : ItemWithIdEqualityComparer<ArtistModel>
	{
		protected override ItemId GetItemId(ArtistModel item)
		{
			return item.Id;
		}
	}
}
