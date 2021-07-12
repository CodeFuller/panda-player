using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Interfaces
{
	internal interface IFolderProvider
	{
		ShallowFolderModel GetFolder(ItemId folderId);
	}
}
