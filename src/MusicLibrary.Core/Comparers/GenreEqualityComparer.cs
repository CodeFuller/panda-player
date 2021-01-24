using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Comparers
{
	public class GenreEqualityComparer : ItemWithIdEqualityComparer<GenreModel>
	{
		protected override ItemId GetItemId(GenreModel item)
		{
			return item.Id;
		}
	}
}
