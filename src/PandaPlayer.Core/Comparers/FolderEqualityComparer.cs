using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class FolderEqualityComparer : ItemWithIdEqualityComparer<FolderModel>
	{
		protected override ItemId GetItemId(FolderModel item)
		{
			return item.Id;
		}
	}
}
