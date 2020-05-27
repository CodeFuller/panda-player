using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Comparers
{
	public class SongEqualityComparer : ItemWithIdEqualityComparer<SongModel>
	{
		protected override ItemId GetItemId(SongModel item)
		{
			return item.Id;
		}
	}
}
