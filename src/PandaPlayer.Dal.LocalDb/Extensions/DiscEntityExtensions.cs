using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class DiscEntityExtensions
	{
		public static DiscModel ToModel(this DiscEntity disc)
		{
			return new()
			{
				Id = disc.Id.ToItemId(),
				Year = disc.Year,
				Title = disc.Title,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};
		}

		public static DiscEntity ToEntity(this DiscModel disc)
		{
			return new()
			{
				Id = disc.Id?.ToInt32() ?? default,
				FolderId = disc.Folder.Id.ToInt32(),
				AdviseGroupId = disc.AdviseGroup?.Id.ToInt32(),
				AdviseSetId = disc.AdviseSetInfo?.AdviseSet.Id.ToInt32(),
				AdviseSetOrder = disc.AdviseSetInfo?.Order,
				Year = disc.Year,
				Title = disc.Title,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};
		}
	}
}
