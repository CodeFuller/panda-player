using MaterialDesignThemes.Wpf;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class ParentFolderExplorerItem : BasicExplorerItem
	{
		public override string Title => "..";

		public override PackIconKind IconKind => PackIconKind.ArrowUpBold;
	}
}
