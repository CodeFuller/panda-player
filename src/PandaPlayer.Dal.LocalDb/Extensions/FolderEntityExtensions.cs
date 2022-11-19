using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class FolderEntityExtensions
	{
		public static FolderModel ToModel(this FolderEntity folder)
		{
			return new()
			{
				Id = folder.Id.ToItemId(),
				Name = folder.Name,
				DeleteDate = folder.DeleteDate,
			};
		}

		public static FolderEntity ToEntity(this FolderModel folder)
		{
			return new()
			{
				Id = folder.Id?.ToInt32() ?? default,
				ParentFolderId = folder.ParentFolder?.Id.ToInt32(),
				Name = folder.Name,
				AdviseGroupId = folder.AdviseGroup?.Id.ToInt32(),
				DeleteDate = folder.DeleteDate,
			};
		}
	}
}
