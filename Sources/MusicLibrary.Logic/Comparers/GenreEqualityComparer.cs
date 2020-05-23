using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Comparers
{
	public class GenreEqualityComparer : ItemWithIdEqualityComparer<GenreModel>
	{
		protected override ItemId GetItemId(GenreModel item)
		{
			return item.Id;
		}
	}
}
