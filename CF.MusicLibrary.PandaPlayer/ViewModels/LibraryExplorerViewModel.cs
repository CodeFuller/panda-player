using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Enums;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
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

		private readonly ILibraryContentUpdater libraryContentUpdater;

		private readonly IViewNavigator viewNavigator;

		private readonly IWindowService windowService;

		private FolderExplorerItem ParentFolder { get; set; }

		private ObservableCollection<FolderExplorerItem> items;
		public ObservableCollection<FolderExplorerItem> Items
		{
			get { return items; }
			private set { Set(ref items, value); }
		}

		public FolderExplorerItem CurrentFolder { get; private set; }

		private FolderExplorerItem selectedItem;
		public FolderExplorerItem SelectedItem
		{
			get { return selectedItem; }
			set
			{
				Set(ref selectedItem, value);
				var discItem = selectedItem as DiscExplorerItem;
				if (discItem != null)
				{
					SongListViewModel.SetSongs(discItem.Disc.Songs);
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(discItem.Disc));
				}
				else
				{
					SongListViewModel.SetSongs(Enumerable.Empty<Song>());
					Messenger.Default.Send(new LibraryExplorerFolderChangedEventArgs(selectedItem?.Uri));
				}
			}
		}

		public IExplorerSongListViewModel SongListViewModel { get; }

		public ICommand ChangeFolderCommand { get; }
		public ICommand PlayDiscCommand { get; }
		public ICommand DeleteDiscCommand { get; }
		public ICommand JumpToFirstItemCommand { get; }
		public ICommand JumpToLastItemCommand { get; }
		public ICommand EditDiscPropertiesCommand { get; }

		public LibraryExplorerViewModel(ILibraryBrowser libraryBrowser, IExplorerSongListViewModel songListViewModel, ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IWindowService windowService)
		{
			if (libraryBrowser == null)
			{
				throw new ArgumentNullException(nameof(libraryBrowser));
			}
			if (songListViewModel == null)
			{
				throw new ArgumentNullException(nameof(songListViewModel));
			}
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}
			if (viewNavigator == null)
			{
				throw new ArgumentNullException(nameof(viewNavigator));
			}
			if (windowService == null)
			{
				throw new ArgumentNullException(nameof(windowService));
			}

			this.libraryBrowser = libraryBrowser;
			SongListViewModel = songListViewModel;
			this.libraryContentUpdater = libraryContentUpdater;
			this.viewNavigator = viewNavigator;
			this.windowService = windowService;

			ChangeFolderCommand = new RelayCommand(ChangeFolder);
			PlayDiscCommand = new RelayCommand(PlayDisc);
			DeleteDiscCommand = new AsyncRelayCommand(DeleteDisc);
			JumpToFirstItemCommand = new RelayCommand(() => SelectedItem = Items.FirstOrDefault());
			JumpToLastItemCommand = new RelayCommand(() => SelectedItem = Items.LastOrDefault());
			EditDiscPropertiesCommand = new RelayCommand(EditDiscProperties);

			Messenger.Default.Register<LibraryLoadedEventArgs>(this, e => Load());
			Messenger.Default.Register<PlayDiscEventArgs>(this, e => SwitchToDisc(e.Disc));
		}

		public void Load()
		{
			ChangeFolder(FolderExplorerItem.Root);
		}

		public void SwitchToDisc(Disc disc)
		{
			var discItem = libraryBrowser.GetDiscItem(disc);
			if (discItem == null)
			{
				return;
			}

			var discParentFolder = libraryBrowser.GetParentFolder(discItem);
			if (discParentFolder == null)
			{
				return;
			}

			ChangeFolder(discParentFolder);
			SelectedItem = Items.OfType<DiscExplorerItem>().SingleOrDefault(it => it.Disc.Id == disc.Id);
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

		private async Task DeleteDisc()
		{
			var discItem = SelectedItem as DiscExplorerItem;
			if (discItem == null)
			{
				return;
			}

			if (windowService.ShowMessageBox($"Do you really want to delete the selected disc '{discItem.Disc.Title}'?", "Delete disc",
				ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Question) != ShowMessageBoxResult.Yes)
			{
				return;
			}

			await libraryContentUpdater.DeleteDisc(discItem.Disc);

			libraryBrowser.RemoveDiscItem(discItem);
			Items.Remove(discItem);

			//	If only '..' item remains
			if (Items.Count == 1)
			{
				FolderExplorerItem currFolder = discItem;
				do
				{
					currFolder = libraryBrowser.GetParentFolder(currFolder);
				}
				while (currFolder != null && !libraryBrowser.GetChildFolderItems(currFolder).Any()) ;

				if (currFolder != null)
				{
					ChangeFolder(currFolder);
				}
			}
		}

		private void EditDiscProperties()
		{
			var discItem = SelectedItem as DiscExplorerItem;
			if (discItem != null)
			{
				viewNavigator.ShowDiscPropertiesView(discItem.Disc);
			}
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
