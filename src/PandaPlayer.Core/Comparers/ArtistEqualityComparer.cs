using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class ArtistEqualityComparer : ItemWithIdEqualityComparer<ArtistModel>
	{
		protected override ItemId GetItemId(ArtistModel item)
		{
			return item.Id;
		}
	}
}
