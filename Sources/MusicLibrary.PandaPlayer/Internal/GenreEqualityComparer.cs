using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Internal
{
	internal class GenreEqualityComparer : ItemWithIdEqualityComparer<GenreModel>
	{
		protected override ItemId GetItemId(GenreModel item)
		{
			return item.Id;
		}
	}
}
