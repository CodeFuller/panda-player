using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Comparers
{
	public class DiscEqualityComparer : ItemWithIdEqualityComparer<DiscModel>
	{
		protected override ItemId GetItemId(DiscModel item)
		{
			return item.Id;
		}
	}
}
