using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.LibraryExplorerEvents;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.ViewModels
{
	public class LibraryExplorerItemListViewModel : ObservableObject, ILibraryExplorerItemListViewModel
	{
		private readonly IMessenger messenger;

		public ObservableCollection<BasicExplorerItem> Items { get; } = new();

		private bool showDeletedContent;

		public bool ShowDeletedContent
		{
			get => showDeletedContent;
			set
			{
				if (SetProperty(ref showDeletedContent, value))
				{
					ReloadCurrentFolder();
				}
			}
		}

		private FolderModel LoadedFolder { get; set; }

		private IEnumerable<FolderExplorerItem> FolderItems => Items.OfType<FolderExplorerItem>();

		private IEnumerable<DiscExplorerItem> DiscItems => Items.OfType<DiscExplorerItem>();

		public IEnumerable<DiscModel> Discs => DiscItems.Select(x => x.Disc);

		public FolderModel SelectedFolder => (SelectedItem as FolderExplorerItem)?.Folder;

		public DiscModel SelectedDisc => (SelectedItem as DiscExplorerItem)?.Disc;

		private BasicExplorerItem selectedItem;

		public BasicExplorerItem SelectedItem
		{
			get => selectedItem;
			set
			{
				if (SetProperty(ref selectedItem, value))
				{
					messenger.Send(new LibraryExplorerDiscChangedEventArgs(SelectedDisc, deletedContentIsShown: ShowDeletedContent));
				}
			}
		}

		public ICommand ChangeFolderCommand { get; }

		public ICommand JumpToFirstItemCommand { get; }

		public ICommand JumpToLastItemCommand { get; }

		public LibraryExplorerItemListViewModel(IMessenger messenger)
		{
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			ChangeFolderCommand = new RelayCommand(ChangeToCurrentlySelectedFolder);
			JumpToFirstItemCommand = new RelayCommand(() => SelectedItem = Items.FirstOrDefault());
			JumpToLastItemCommand = new RelayCommand(() => SelectedItem = Items.LastOrDefault());
		}

		public void LoadFolderItems(FolderModel folder)
		{
			// Remembering selected item for the case when the same folder is reloaded (ShowDeletedContent is changed).
			var previousSelectedItem = SelectedItem;

			Items.Clear();

			if (folder.ParentFolder != null)
			{
				Items.Add(new ParentFolderExplorerItem());
			}

			var subfolders = folder.Subfolders
				.Where(sf => ShowDeletedContent || !sf.IsDeleted)
				.Select(sf => new FolderExplorerItem(sf))
				.OrderBy(sf => sf.Title, StringComparer.InvariantCultureIgnoreCase);

			var discs = folder.Discs
				.Where(disc => ShowDeletedContent || !disc.IsDeleted)
				.Select(disc => new DiscExplorerItem(disc))
				.OrderBy(disc => disc.Title, StringComparer.InvariantCultureIgnoreCase);

			Items.AddRange(subfolders);
			Items.AddRange(discs);

			LoadedFolder = folder;

			switch (previousSelectedItem)
			{
				case FolderExplorerItem selectedFolder:
					SelectFolder(selectedFolder.FolderId);
					break;

				case DiscExplorerItem selectedDisc:
					SelectDisc(selectedDisc.DiscId);
					break;
			}

			SelectedItem ??= Items.FirstOrDefault();
		}

		private void ReloadCurrentFolder()
		{
			LoadFolderItems(LoadedFolder);
		}

		public void SelectFolder(ItemId folderId)
		{
			SelectedItem = GetFolderItem(folderId);
		}

		public void SelectDisc(ItemId discId)
		{
			SelectedItem = GetDiscItem(discId);
		}

		public Task OnFolderDeleted(ItemId folderId)
		{
			OnContentDeleted(GetFolderItem(folderId));
			return Task.CompletedTask;
		}

		public Task OnDiscDeleted(ItemId discId)
		{
			OnContentDeleted(GetDiscItem(discId));
			return Task.CompletedTask;
		}

		private void OnContentDeleted(BasicExplorerItem item)
		{
			if (item == null)
			{
				return;
			}

			if (ShowDeletedContent)
			{
				ReloadCurrentFolder();
			}
			else
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
					messenger.Send(new LoadParentFolderEventArgs(LoadedFolder.ParentFolder, LoadedFolder.Id));
					break;

				case FolderExplorerItem folderItem:
					messenger.Send(new LoadFolderEventArgs(folderItem.Folder));
					break;
			}
		}
	}
}
