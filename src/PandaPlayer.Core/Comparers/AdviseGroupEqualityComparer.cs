using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class AdviseGroupEqualityComparer : ItemWithIdEqualityComparer<AdviseGroupModel>
	{
		protected override ItemId GetItemId(AdviseGroupModel item)
		{
			return item.Id;
		}
	}
}
