using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Extensions;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.Abstractions.Dto;
using MusicLibrary.Dal.Abstractions.Dto.Folders;
using MusicLibrary.Dal.Abstractions.Interfaces;
using MusicLibrary.PandaPlayer.ContentUpdate;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class LibraryExplorerViewModel : ViewModelBase, ILibraryExplorerViewModel
	{
		private readonly ILibraryContentUpdater libraryContentUpdater;

		private readonly IViewNavigator viewNavigator;

		private readonly IWindowService windowService;

		private readonly IFolderReader folderReader;

		public ObservableCollection<BasicExplorerItem> Items { get; } = new ObservableCollection<BasicExplorerItem>();

		private ItemId ParentFolderId { get; set; }

		private ItemId LoadedFolderId { get; set; }

		private BasicExplorerItem selectedItem;

		public BasicExplorerItem SelectedItem
		{
			get => selectedItem;
			set
			{
				Set(ref selectedItem, value);
				var selectedDisc = SelectedDisc;
				if (selectedDisc != null)
				{
					SongListViewModel.SetSongs(selectedDisc.Songs);
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(selectedDisc));
				}
				else
				{
					SongListViewModel.SetSongs(Enumerable.Empty<Song>());
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(null));
				}
			}
		}

		public Disc SelectedDisc => (selectedItem as DiscExplorerItem)?.Disc;

		public IExplorerSongListViewModel SongListViewModel { get; }

		public ICommand ChangeFolderCommand { get; }

		public ICommand PlayDiscCommand { get; }

		public ICommand DeleteDiscCommand { get; }

		public ICommand JumpToFirstItemCommand { get; }

		public ICommand JumpToLastItemCommand { get; }

		public ICommand EditDiscPropertiesCommand { get; }

		public LibraryExplorerViewModel(IFolderReader folderReader, IExplorerSongListViewModel songListViewModel,
			ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IWindowService windowService)
		{
			this.folderReader = folderReader ?? throw new ArgumentNullException(nameof(folderReader));
			SongListViewModel = songListViewModel ?? throw new ArgumentNullException(nameof(songListViewModel));
			this.libraryContentUpdater = libraryContentUpdater ?? throw new ArgumentNullException(nameof(libraryContentUpdater));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			ChangeFolderCommand = new AsyncRelayCommand(() => ChangeToCurrentlySelectedFolder(CancellationToken.None));
			PlayDiscCommand = new RelayCommand(PlayDisc);
			DeleteDiscCommand = new AsyncRelayCommand(DeleteDisc);
			JumpToFirstItemCommand = new RelayCommand(() => SelectedItem = Items.FirstOrDefault());
			JumpToLastItemCommand = new RelayCommand(() => SelectedItem = Items.LastOrDefault());
			EditDiscPropertiesCommand = new RelayCommand(EditDiscProperties);

			Messenger.Default.Register<LibraryLoadedEventArgs>(this, e => LoadRootFolder());
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => OnPlaylistChanged(e.Disc));
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => OnPlaylistChanged(e.Disc));
		}

		// TBD: How to make it async?
		private void LoadRootFolder()
		{
			// TBD: We should not load root folder, if some disc is active, because folder of this disc will be loaded just after that.
			var rootFolderData = folderReader.GetRootFolder(false, CancellationToken.None).Result;
			LoadFolder(rootFolderData);
		}

		private async Task ChangeToCurrentlySelectedFolder(CancellationToken cancellationToken)
		{
			switch (SelectedItem)
			{
				case ParentFolderExplorerItem _:
					await LoadParentFolder(cancellationToken);
					break;

				case FolderExplorerItem folderItem:
					await LoadFolder(folderItem.FolderId, cancellationToken);
					break;
			}
		}

		private async Task LoadFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			var folder = await folderReader.GetFolder(folderId, includeDeletedDiscs: false, cancellationToken);

			LoadFolder(folder);
		}

		private void LoadFolder(FolderData folderData)
		{
			SetFolderItems(folderData);

			SelectedItem = Items.FirstOrDefault();

			ParentFolderId = folderData.ParentFolderId;
			LoadedFolderId = folderData.Id;
		}

		private async Task LoadParentFolder(CancellationToken cancellationToken)
		{
			// Remembering current folder before it gets updated.
			var prevFolderId = LoadedFolderId;

			await LoadFolder(ParentFolderId, cancellationToken);

			// Setting previously loaded folder as currently selected item.
			FolderExplorerItem newSelectedItem = null;
			if (prevFolderId != null)
			{
				newSelectedItem = Items.OfType<FolderExplorerItem>().FirstOrDefault(f => f.FolderId == prevFolderId);
			}

			SelectedItem = newSelectedItem ?? Items.FirstOrDefault();
		}

		private void SetFolderItems(FolderData folderData)
		{
			Items.Clear();

			if (folderData.ParentFolderId != null)
			{
				Items.Add(new ParentFolderExplorerItem());
			}

			Items.AddRange(folderData.Subfolders.Select(x => new FolderExplorerItem(x)).OrderBy(x => x.Title, StringComparer.OrdinalIgnoreCase));

			Items.AddRange(folderData.Discs.Select(x => new DiscExplorerItem(x)).OrderBy(x => x.Title, StringComparer.OrdinalIgnoreCase));
		}

		public void SwitchToDisc(Disc disc)
		{
			// TBD: Implement loading of disc folder
			throw new NotImplementedException();

			/*
			var discId = disc.GetDiscId();
			var discFolder = folderReader.GetDiscFolder(discId, CancellationToken.None).Result;

			LoadFolder(discFolder);

			SelectedItem = Items.OfType<DiscExplorerItem>().FirstOrDefault(x => x.DiscId == discId);
			*/
		}

		private void PlayDisc()
		{
			if (!(SelectedItem is DiscExplorerItem discItem))
			{
				return;
			}

			Messenger.Default.Send(new PlaySongsListEventArgs(discItem.Disc));
		}

		public static async Task DeleteDisc()
		{
			// TBD: Implement disc deletion
			await Task.CompletedTask;
			throw new NotImplementedException();
/*
			if (!(SelectedItem is DiscExplorerItem discItem))
			{
				return;
			}

			if (windowService.ShowMessageBox($"Do you really want to delete the selected disc '{discItem.Disc.Title}'?", "Delete disc",
				ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Question) != ShowMessageBoxResult.Yes)
			{
				return;
			}

			// We're sending this event to release any disc images hold by DiscImageViewModel.
			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(null));
			await libraryContentUpdater.DeleteDisc(discItem.Disc);

			Items.Remove(discItem);

			// If current folder does not contain other items anymore (i.e. only '..' item remains),
			// then we want to navigate to upper non-empty folder.
			if (Items.Count == 1)
			{
				var currentFolder = libraryBrowser.GetParentFolder(discItem);

				while (currentFolder != null && !libraryBrowser.GetFolderItems(currentFolder).Any())
				{
					currentFolder = libraryBrowser.GetParentFolder(currentFolder);
				}

				if (currentFolder != null)
				{
					ChangeFolder(currentFolder);
				}
			}
*/
		}

		private void EditDiscProperties()
		{
			if (SelectedItem is DiscExplorerItem discItem)
			{
				viewNavigator.ShowDiscPropertiesView(discItem.Disc);
			}
		}

		private void OnPlaylistChanged(Disc disc)
		{
			if (disc != null)
			{
				SwitchToDisc(disc);
			}
		}
	}
}
