using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class DiscEqualityComparer : ItemWithIdEqualityComparer<DiscModel>
	{
		protected override ItemId GetItemId(DiscModel item)
		{
			return item.Id;
		}
	}
}
