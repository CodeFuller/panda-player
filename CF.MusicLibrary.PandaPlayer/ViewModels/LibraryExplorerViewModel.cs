using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class LibraryExplorerViewModel : ViewModelBase, ILibraryExplorerViewModel
	{
		private readonly ILibraryBrowser libraryBrowser;

		private FolderExplorerItem ParentFolder { get; set; }

		private ObservableCollection<FolderExplorerItem> items;
		public ObservableCollection<FolderExplorerItem> Items
		{
			get { return items; }
			private set { Set(ref items, value); }
		}

		private FolderExplorerItem CurrentFolder { get; set; }

		private FolderExplorerItem selectedItem;
		public FolderExplorerItem SelectedItem
		{
			get { return selectedItem; }
			set { Set(ref selectedItem, value); }
		}

		public ICommand ChangeFolderCommand { get; }
		public ICommand PlayDiscCommand { get; }

		public LibraryExplorerViewModel(ILibraryBrowser libraryBrowser)
		{
			if (libraryBrowser == null)
			{
				throw new ArgumentNullException(nameof(libraryBrowser));
			}

			this.libraryBrowser = libraryBrowser;

			ChangeFolderCommand = new RelayCommand(ChangeFolder);
			PlayDiscCommand = new RelayCommand(PlayDisc);
		}

		public void Load()
		{
			ChangeFolder(FolderExplorerItem.Root);
		}

		private void ChangeFolder()
		{
			ChangeFolder(SelectedItem);
		}

		private void PlayDisc()
		{
			var discItem = SelectedItem as DiscExplorerItem;
			if (discItem == null)
			{
				return;
			}

			Messenger.Default.Send(new PlayDiscEventArgs(discItem.Disc));
		}

		private void ChangeFolder(FolderExplorerItem newFolder)
		{
			if (newFolder == null)
			{
				return;
			}

			var childFolderItems = libraryBrowser.GetChildFolderItems(newFolder).ToList();
			if (!childFolderItems.Any())
			{
				return;
			}

			//	Remember current directory if we're moving up
			FolderExplorerItem prevFolder = null;
			if (newFolder.IsParentItem)
			{
				prevFolder = CurrentFolder;
			}
			CurrentFolder = newFolder;

			//	Getting a parent of new folder
			ParentFolder = libraryBrowser.GetParentFolder(newFolder);
			if (ParentFolder != null)
			{
				ParentFolder.IsParentItem = true;
			}

			//	Building new items list
			Items = new ObservableCollection<FolderExplorerItem>(childFolderItems);
			if (ParentFolder != null)
			{
				Items.Insert(0, ParentFolder);
			}

			//	Setting selected item
			FolderExplorerItem newSelectedItem = null;
			if (prevFolder != null)
			{
				newSelectedItem = Items.FirstOrDefault(f => new FolderItemComparer().Equals(f, prevFolder));
			}
			SelectedItem = newSelectedItem ?? Items.FirstOrDefault();
		}
	}
}
