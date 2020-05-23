using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Internal
{
	internal class ArtistEqualityComparer : ItemWithIdEqualityComparer<ArtistModel>
	{
		protected override ItemId GetItemId(ArtistModel item)
		{
			return item.Id;
		}
	}
}
