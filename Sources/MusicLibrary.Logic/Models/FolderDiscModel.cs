namespace MusicLibrary.Logic.Models
{
	public class FolderDiscModel
	{
		public ItemId Id { get; set; }

		public string TreeTitle { get; set; }

		// TBD: Remove after redesign?
		public DiscModel Disc { get; set; }
	}
}
