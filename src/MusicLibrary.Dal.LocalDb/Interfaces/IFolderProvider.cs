using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IFolderProvider
	{
		ShallowFolderModel GetFolder(ItemId folderId);
	}
}
