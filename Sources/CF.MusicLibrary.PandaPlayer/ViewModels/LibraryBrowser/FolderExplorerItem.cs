using System;
using CF.MusicLibrary.Core;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class FolderExplorerItem : LibraryExplorerItem
	{
		public static FolderExplorerItem Root => new FolderExplorerItem(ItemUriParts.RootUri);

		public override string Name => IsParentItem ? ".." : base.Name;

		public bool IsParentItem { get; set; }

		public FolderExplorerItem(Uri uri) :
			base(uri)
		{
		}
	}
}
