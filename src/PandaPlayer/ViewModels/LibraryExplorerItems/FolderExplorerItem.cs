using System;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class FolderExplorerItem : BasicExplorerItem
	{
		public ItemId FolderId => Folder.Id;

		public ShallowFolderModel Folder { get; }

		public override string Title => Folder.Name;

		public override PackIconKind IconKind => Folder.AdviseGroup != null ? PackIconKind.FolderStar : PackIconKind.Folder;

		public FolderExplorerItem(ShallowFolderModel folder)
		{
			Folder = folder ?? throw new ArgumentNullException(nameof(folder));

			Folder.PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(ShallowFolderModel.AdviseGroup))
				{
					RaisePropertyChanged(nameof(IconKind));
				}
			};
		}
	}
}
