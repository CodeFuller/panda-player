using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class DiscEntityExtensions
	{
		public static DiscModel ToModel(this DiscEntity disc, ShallowFolderModel folderModel, IContentUriProvider contentUriProvider)
		{
			var discModel = new DiscModel
			{
				Id = disc.Id.ToItemId(),
				Folder = folderModel,
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
			return new DiscEntity
			{
				Id = disc.Id?.ToInt32() ?? default,
				FolderId = disc.Folder.Id.ToInt32(),
				Year = disc.Year,
				Title = disc.Title,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};
		}
	}
}
