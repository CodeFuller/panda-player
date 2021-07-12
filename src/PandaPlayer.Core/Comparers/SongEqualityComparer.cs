using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class SongEqualityComparer : ItemWithIdEqualityComparer<SongModel>
	{
		protected override ItemId GetItemId(SongModel item)
		{
			return item.Id;
		}
	}
}
