using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Comparers
{
	public class GenreEqualityComparer : ItemWithIdEqualityComparer<GenreModel>
	{
		protected override ItemId GetItemId(GenreModel item)
		{
			return item.Id;
		}
	}
}
