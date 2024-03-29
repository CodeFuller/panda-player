using System;
using System.Collections.Generic;
using System.Threading;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class FolderExplorerItem : BasicExplorerItem
	{
		public ItemId FolderId => Folder.Id;

		public FolderModel Folder { get; }

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

		public FolderExplorerItem(FolderModel folder)
		{
			Folder = folder ?? throw new ArgumentNullException(nameof(folder));

			Folder.PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(Folder.Name))
				{
					OnPropertyChanged(nameof(Title));
				}

				if (args.PropertyName == nameof(Folder.AdviseGroup))
				{
					OnPropertyChanged(nameof(IconKind));
				}
			};
		}

		public override IEnumerable<BasicMenuItem> GetContextMenuItems(ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper)
		{
			if (IsDeleted)
			{
				yield break;
			}

			yield return new ExpandableMenuItem
			{
				Header = "Set Advise Group",
				IconKind = PackIconKind.FolderStar,
				Items = GetAdviseGroupMenuItems(new FolderAdviseGroupHolder(Folder), libraryExplorerViewModel, adviseGroupHelper),
			};

			yield return new CommandMenuItem(() => libraryExplorerViewModel.RenameFolder(Folder))
			{
				Header = "Rename Folder",
				IconKind = PackIconKind.Pencil,
			};

			yield return new CommandMenuItem(() => libraryExplorerViewModel.DeleteFolder(Folder, CancellationToken.None))
			{
				Header = "Delete Folder",
				IconKind = PackIconKind.DeleteForever,
			};
		}
	}
}
