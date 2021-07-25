using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.LibraryExplorerEvents;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.ViewModels
{
	public class LibraryExplorerItemListViewModel : ViewModelBase, ILibraryExplorerItemListViewModel
	{
		public ObservableCollection<BasicExplorerItem> Items { get; } = new();

		private ShallowFolderModel LoadedFolder { get; set; }

		private IEnumerable<FolderExplorerItem> FolderItems => Items.OfType<FolderExplorerItem>();

		private IEnumerable<DiscExplorerItem> DiscItems => Items.OfType<DiscExplorerItem>();

		public IEnumerable<DiscModel> Discs => DiscItems.Select(x => x.Disc);

		public ShallowFolderModel SelectedFolder => (SelectedItem as FolderExplorerItem)?.Folder;

		public DiscModel SelectedDisc => (selectedItem as DiscExplorerItem)?.Disc;

		private BasicExplorerItem selectedItem;

		public BasicExplorerItem SelectedItem
		{
			get => selectedItem;
			set
			{
				if (Set(ref selectedItem, value))
				{
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(SelectedDisc));
				}
			}
		}

		public ICommand ChangeFolderCommand { get; }

		public ICommand JumpToFirstItemCommand { get; }

		public ICommand JumpToLastItemCommand { get; }

		public LibraryExplorerItemListViewModel()
		{
			ChangeFolderCommand = new RelayCommand(ChangeToCurrentlySelectedFolder);
			JumpToFirstItemCommand = new RelayCommand(() => SelectedItem = Items.FirstOrDefault());
			JumpToLastItemCommand = new RelayCommand(() => SelectedItem = Items.LastOrDefault());
		}

		public void LoadFolderItems(FolderModel folder)
		{
			Items.Clear();

			if (folder.ParentFolder != null)
			{
				Items.Add(new ParentFolderExplorerItem());
			}

			var subfolders = folder.Subfolders
				.Where(sf => !sf.IsDeleted)
				.Select(sf => new FolderExplorerItem(sf))
				.OrderBy(sf => sf.Title, StringComparer.InvariantCultureIgnoreCase);

			var discs = folder.Discs
				.Where(disc => !disc.IsDeleted)
				.Select(disc => new DiscExplorerItem(disc))
				.OrderBy(disc => disc.Title, StringComparer.InvariantCultureIgnoreCase);

			Items.AddRange(subfolders);
			Items.AddRange(discs);

			LoadedFolder = folder;
			SelectedItem = Items.FirstOrDefault();
		}

		public void SelectFolder(ItemId folderId)
		{
			SelectedItem = GetFolderItem(folderId);
		}

		public void SelectDisc(ItemId discId)
		{
			SelectedItem = GetDiscItem(discId);
		}

		public void RemoveFolder(ItemId folderId)
		{
			RemoveItem(GetFolderItem(folderId));
		}

		public void RemoveDisc(ItemId discId)
		{
			RemoveItem(GetDiscItem(discId));
		}

		private void RemoveItem(BasicExplorerItem item)
		{
			if (item != null)
			{
				Items.Remove(item);
			}
		}

		private FolderExplorerItem GetFolderItem(ItemId folderId)
		{
			return FolderItems.FirstOrDefault(x => x.FolderId == folderId);
		}

		private DiscExplorerItem GetDiscItem(ItemId discId)
		{
			return DiscItems.FirstOrDefault(x => x.DiscId == discId);
		}

		private void ChangeToCurrentlySelectedFolder()
		{
			switch (SelectedItem)
			{
				case ParentFolderExplorerItem:
					Messenger.Default.Send(new LoadParentFolderEventArgs(LoadedFolder.ParentFolderId, LoadedFolder.Id));
					break;

				case FolderExplorerItem folderItem:
					Messenger.Default.Send(new LoadFolderEventArgs(folderItem.FolderId));
					break;
			}
		}
	}
}
