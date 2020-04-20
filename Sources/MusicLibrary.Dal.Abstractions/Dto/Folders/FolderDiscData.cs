using MusicLibrary.Core.Objects;

namespace MusicLibrary.Dal.Abstractions.Dto.Folders
{
	public class FolderDiscData
	{
		public ItemId Id { get; set; }

		public string TreeTitle { get; set; }

		// TBD: Remove after redesign
		public Disc Disc { get; set; }
	}
}
