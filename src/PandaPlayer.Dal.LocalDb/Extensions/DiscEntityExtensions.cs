using System.Linq;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;
using PandaPlayer.Dal.LocalDb.Interfaces;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class DiscEntityExtensions
	{
		public static DiscModel ToModel(this DiscEntity disc, ShallowFolderModel folderModel, IContentUriProvider contentUriProvider)
		{
			var discModel = new DiscModel
			{
				Id = disc.Id.ToItemId(),
				Folder = folderModel,
				AdviseGroup = disc.AdviseGroup?.ToModel(),
				Year = disc.Year,
				Title = disc.Title,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};

			discModel.AllSongs = disc.Songs.Select(s => s.ToModel(discModel, contentUriProvider)).ToList();
			discModel.Images = disc.Images.Select(im => im.ToModel(discModel, contentUriProvider)).ToList();

			return discModel;
		}

		public static DiscModel ToModel(this DiscEntity disc, IContentUriProvider contentUriProvider)
		{
			var folderModel = disc.Folder.ToShallowModel();
			return disc.ToModel(folderModel, contentUriProvider);
		}

		public static DiscEntity ToEntity(this DiscModel disc)
		{
			return new()
			{
				Id = disc.Id?.ToInt32() ?? default,
				FolderId = disc.Folder.Id.ToInt32(),
				AdviseGroupId = disc.AdviseGroup?.Id.ToInt32(),
				Year = disc.Year,
				Title = disc.Title,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};
		}
	}
}
