using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class FolderEntityExtensions
	{
		public static FolderModel ToModel(this FolderEntity folder, IContentUriProvider contentUriProvider)
		{
			var folderModel = new FolderModel
			{
				Id = folder.Id.ToItemId(),
				ParentFolderId = folder.ParentFolderId?.ToItemId(),
				ParentFolder = folder.ParentFolder?.ToShallowModel(),
				Name = folder.Name,
				Subfolders = folder.Subfolders.Select(sf => sf.ToShallowModel()).ToList(),
				DeleteDate = folder.DeleteDate,
			};

			folderModel.Discs = folder.Discs.Select(d => d.ToModel(folderModel, contentUriProvider)).ToList();

			return folderModel;
		}

		public static ShallowFolderModel ToShallowModel(this FolderEntity folder)
		{
			return new ShallowFolderModel
			{
				Id = folder.Id.ToItemId(),
				ParentFolderId = folder.ParentFolderId?.ToItemId(),
				Name = folder.Name,
				DeleteDate = folder.DeleteDate,
			};
		}

		public static FolderEntity ToEntity(this ShallowFolderModel folder)
		{
			return new FolderEntity
			{
				Id = folder.Id.ToInt32(),
				ParentFolderId = folder.ParentFolderId?.ToInt32(),
				Name = folder.Name,
				DeleteDate = folder.DeleteDate,
			};
		}
	}
}
