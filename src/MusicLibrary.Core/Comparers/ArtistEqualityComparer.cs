using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Comparers
{
	public class ArtistEqualityComparer : ItemWithIdEqualityComparer<ArtistModel>
	{
		protected override ItemId GetItemId(ArtistModel item)
		{
			return item.Id;
		}
	}
}
