using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Entities
{
	internal class FolderEntity
	{
		// TODO: Change id to int. Currently folder URI is encoded in id.
		public ItemId Id { get; set; }

		public string Name { get; set; }
	}
}
