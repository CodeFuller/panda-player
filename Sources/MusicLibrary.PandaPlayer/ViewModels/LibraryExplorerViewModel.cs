using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Enums;
using CF.Library.Core.Extensions;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class LibraryExplorerViewModel : ViewModelBase, ILibraryExplorerViewModel
	{
		private readonly IFoldersService foldersService;

		private readonly IDiscsService discsService;

		private readonly IViewNavigator viewNavigator;

		private readonly IWindowService windowService;

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
					SongListViewModel.SetSongs(selectedDisc.Songs.Where(song => !song.IsDeleted));
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(selectedDisc));
				}
				else
				{
					SongListViewModel.SetSongs(Enumerable.Empty<SongModel>());
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(null));
				}
			}
		}

		public DiscModel SelectedDisc => (selectedItem as DiscExplorerItem)?.Disc;

		public IExplorerSongListViewModel SongListViewModel { get; }

		public ICommand ChangeFolderCommand { get; }

		public ICommand PlayDiscCommand { get; }

		public ICommand DeleteDiscCommand { get; }

		public ICommand JumpToFirstItemCommand { get; }

		public ICommand JumpToLastItemCommand { get; }

		public ICommand EditDiscPropertiesCommand { get; }

		public LibraryExplorerViewModel(IFoldersService foldersService, IDiscsService discsService,
			IExplorerSongListViewModel songListViewModel, IViewNavigator viewNavigator, IWindowService windowService)
		{
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			SongListViewModel = songListViewModel ?? throw new ArgumentNullException(nameof(songListViewModel));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			ChangeFolderCommand = new AsyncRelayCommand(() => ChangeToCurrentlySelectedFolder(CancellationToken.None));
			PlayDiscCommand = new RelayCommand(PlayDisc);
			DeleteDiscCommand = new AsyncRelayCommand(() => DeleteDisc(CancellationToken.None));
			JumpToFirstItemCommand = new RelayCommand(() => SelectedItem = Items.FirstOrDefault());
			JumpToLastItemCommand = new RelayCommand(() => SelectedItem = Items.LastOrDefault());
			EditDiscPropertiesCommand = new RelayCommand(EditDiscProperties);

			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, e => LoadRootFolder());
			Messenger.Default.Register<PlaySongsListEventArgs>(this, OnPlaylistChanged);
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, OnPlaylistChanged);
		}

		// TBD: How to make it async?
		private void LoadRootFolder()
		{
			// TBD: We should not load root folder, if some disc is active, because folder of this disc will be loaded just after that.
			var rootFolderData = foldersService.GetRootFolder(CancellationToken.None).Result;
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
			var folder = await foldersService.GetFolder(folderId, cancellationToken);

			LoadFolder(folder);
		}

		private void LoadFolder(FolderModel folder)
		{
			SetFolderItems(folder);

			SelectedItem = Items.FirstOrDefault();

			ParentFolderId = folder.ParentFolderId;
			LoadedFolderId = folder.Id;
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

		private void SetFolderItems(FolderModel folder)
		{
			Items.Clear();

			if (folder.ParentFolderId != null)
			{
				Items.Add(new ParentFolderExplorerItem());
			}

			var subfolders = folder.Subfolders
				.Select(x => new FolderExplorerItem(x))
				.OrderBy(x => x.Title, StringComparer.OrdinalIgnoreCase);

			var discs = folder.Discs
				.Where(disc => !disc.IsDeleted)
				.Select(x => new DiscExplorerItem(x))
				.OrderBy(x => x.Title, StringComparer.OrdinalIgnoreCase);

			Items.AddRange(subfolders);
			Items.AddRange(discs);
		}

		public void SwitchToDisc(DiscModel disc)
		{
			var discFolder = foldersService.GetDiscFolder(disc.Id, CancellationToken.None).Result;

			LoadFolder(discFolder);

			SelectedItem = Items.OfType<DiscExplorerItem>().FirstOrDefault(x => x.DiscId == disc.Id);
		}

		private void PlayDisc()
		{
			if (!(SelectedItem is DiscExplorerItem discItem))
			{
				return;
			}

			Messenger.Default.Send(new PlaySongsListEventArgs(discItem.Disc));
		}

		private async Task DeleteDisc(CancellationToken cancellationToken)
		{
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
			await discsService.DeleteDisc(discItem.DiscId, cancellationToken);

			Items.Remove(discItem);
		}

		private void EditDiscProperties()
		{
			if (SelectedItem is DiscExplorerItem discItem)
			{
				viewNavigator.ShowDiscPropertiesView(discItem.Disc);
			}
		}

		private void OnPlaylistChanged(BaseSongListEventArgs e)
		{
			if (e.Disc != null)
			{
				SwitchToDisc(e.Disc);
			}
		}
	}
}
