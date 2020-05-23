using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Comparers
{
	public class ArtistEqualityComparer : ItemWithIdEqualityComparer<ArtistModel>
	{
		protected override ItemId GetItemId(ArtistModel item)
		{
			return item.Id;
		}
	}
}
