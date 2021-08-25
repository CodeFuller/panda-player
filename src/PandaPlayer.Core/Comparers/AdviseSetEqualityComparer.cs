using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class AdviseSetEqualityComparer : ItemWithIdEqualityComparer<AdviseSetModel>
	{
		protected override ItemId GetItemId(AdviseSetModel item)
		{
			return item.Id;
		}
	}
}
