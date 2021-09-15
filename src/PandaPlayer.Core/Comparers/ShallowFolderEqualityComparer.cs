using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class ShallowFolderEqualityComparer : ItemWithIdEqualityComparer<ShallowFolderModel>
	{
		protected override ItemId GetItemId(ShallowFolderModel item)
		{
			return item.Id;
		}
	}
}
