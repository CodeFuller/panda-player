using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Grouping
{
	internal class AdviseGroupContentEqualityComparer : ItemWithIdEqualityComparer<AdviseGroupContent>
	{
		protected override ItemId GetItemId(AdviseGroupContent item)
		{
			return new(item.Id);
		}
	}
}
