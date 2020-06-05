using System.Linq;
using CF.Library.Core.Extensions;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class DiscEntityExtensions
	{
		public static DiscModel ToModel(this DiscEntity disc, ShallowFolderModel folderModel, IContentUriProvider contentUriProvider)
		{
			// TODO: Add year as disc column in database and remove this logic
			var discYear = disc.Songs
				.Where(song => song.DeleteDate == null)
				.Select(song => song.Year)
				.UniqueOrDefault();

			var discModel = new DiscModel
			{
				Id = disc.Id.ToItemId(),
				Folder = folderModel,
				Year = discYear,
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
				Id = disc.Id.ToInt32(),
				FolderId = disc.Folder.Id.ToInt32(),
				Title = disc.Title,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};
		}
	}
}
