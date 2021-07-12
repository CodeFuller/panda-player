using PandaPlayer.Core.Models;

namespace PandaPlayer.Core.Comparers
{
	public class GenreEqualityComparer : ItemWithIdEqualityComparer<GenreModel>
	{
		protected override ItemId GetItemId(GenreModel item)
		{
			return item.Id;
		}
	}
}
