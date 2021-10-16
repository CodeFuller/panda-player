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

		public override PackIconKind IconKind
		{
			get
			{
				if (Folder.AdviseGroup == null)
				{
					return PackIconKind.Folder;
				}

				return Folder.AdviseGroup.IsFavorite ? PackIconKind.FolderHeart : PackIconKind.FolderStar;
			}
		}

		public override bool IsDeleted => Folder.IsDeleted;

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
