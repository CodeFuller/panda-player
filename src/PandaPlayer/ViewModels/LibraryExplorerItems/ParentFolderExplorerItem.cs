using System.Collections.Generic;
using System.Linq;
using MaterialDesignThemes.Wpf;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class ParentFolderExplorerItem : BasicExplorerItem
	{
		public override string Title => "..";

		public override PackIconKind IconKind => PackIconKind.ArrowUpBold;

		public override bool IsDeleted => false;

		public override IEnumerable<BasicMenuItem> GetContextMenuItems(ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper)
		{
			return Enumerable.Empty<BasicMenuItem>();
		}
	}
}
