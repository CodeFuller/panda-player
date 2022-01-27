using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class FolderEntityExtensions
	{
		public static ShallowFolderModel ToShallowModel(this FolderEntity folder)
		{
			return new()
			{
				Id = folder.Id.ToItemId(),
				ParentFolderId = folder.ParentFolderId?.ToItemId(),
				Name = folder.Name,
				AdviseGroup = folder.AdviseGroup?.ToModel(),
				DeleteDate = folder.DeleteDate,
			};
		}

		public static FolderEntity ToEntity(this ShallowFolderModel folder)
		{
			return new()
			{
				Id = folder.Id?.ToInt32() ?? default,
				ParentFolderId = folder.ParentFolderId?.ToInt32(),
				Name = folder.Name,
				AdviseGroupId = folder.AdviseGroup?.Id.ToInt32(),
				DeleteDate = folder.DeleteDate,
			};
		}
	}
}
