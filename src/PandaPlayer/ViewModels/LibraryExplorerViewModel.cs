using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.LibraryExplorerEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Internal;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class LibraryExplorerViewModel : ViewModelBase, ILibraryExplorerViewModel
	{
		private readonly IFoldersService foldersService;

		private readonly IDiscsService discsService;

		private readonly IViewNavigator viewNavigator;

		private readonly IWindowService windowService;

		public ILibraryExplorerItemListViewModel ItemListViewModel { get; }

		public DiscModel SelectedDisc => ItemListViewModel.SelectedDisc;

		public ICommand PlayDiscCommand { get; }

		public ICommand AddDiscToPlaylistCommand { get; }

		public ICommand EditDiscPropertiesCommand { get; }

		public ICommand DeleteFolderCommand { get; }

		public ICommand DeleteDiscCommand { get; }

		public LibraryExplorerViewModel(ILibraryExplorerItemListViewModel itemListViewModel, IFoldersService foldersService,
			IDiscsService discsService, IViewNavigator viewNavigator, IWindowService windowService)
		{
			ItemListViewModel = itemListViewModel ?? throw new ArgumentNullException(nameof(itemListViewModel));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			PlayDiscCommand = new RelayCommand(PlayDisc);
			AddDiscToPlaylistCommand = new RelayCommand(AddDiscToPlaylist);
			EditDiscPropertiesCommand = new RelayCommand(EditDiscProperties);
			DeleteFolderCommand = new AsyncRelayCommand(() => DeleteFolder(CancellationToken.None));
			DeleteDiscCommand = new AsyncRelayCommand(() => DeleteDisc(CancellationToken.None));

			Messenger.Default.Register<LoadParentFolderEventArgs>(this, e => OnLoadParentFolder(e, CancellationToken.None));
			Messenger.Default.Register<LoadFolderEventArgs>(this, e => OnLoadFolder(e.FolderId, CancellationToken.None));
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => OnPlaySongsList(e, CancellationToken.None));
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => OnPlaylistLoaded(e, CancellationToken.None));
			Messenger.Default.Register<NoPlaylistLoadedEventArgs>(this, _ => OnNoPlaylistLoaded(CancellationToken.None));
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => OnSwitchToDisc(e.Disc, CancellationToken.None));
			Messenger.Default.Register<SongChangedEventArgs>(this, e => OnSongChanged(e.Song, e.PropertyName));
			Messenger.Default.Register<DiscChangedEventArgs>(this, e => OnDiscChanged(e.Disc, e.PropertyName));
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => OnDiscImageChanged(e.Disc));
		}

		private async void OnLoadParentFolder(LoadParentFolderEventArgs e, CancellationToken cancellationToken)
		{
			await LoadFolder(e.ParentFolderId, cancellationToken);

			ItemListViewModel.SelectFolder(e.ChildFolderId);
		}

		private async void OnLoadFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			await LoadFolder(folderId, cancellationToken);
		}

		private async Task LoadFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			var folder = await foldersService.GetFolder(folderId, cancellationToken);

			LoadFolder(folder);
		}

		private void LoadFolder(FolderModel folder)
		{
			ItemListViewModel.LoadFolderItems(folder);
		}

		private async Task SwitchToDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await LoadFolder(disc.Folder.Id, cancellationToken);

			ItemListViewModel.SelectDisc(disc.Id);
		}

		private void PlayDisc()
		{
			if (SelectedDisc != null)
			{
				Messenger.Default.Send(new PlaySongsListEventArgs(SelectedDisc));
			}
		}

		private void AddDiscToPlaylist()
		{
			if (SelectedDisc != null)
			{
				Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(SelectedDisc.ActiveSongs));
			}
		}

		private void EditDiscProperties()
		{
			if (SelectedDisc != null)
			{
				viewNavigator.ShowDiscPropertiesView(SelectedDisc);
			}
		}

		private async Task DeleteFolder(CancellationToken cancellationToken)
		{
			var selectedFolder = ItemListViewModel.SelectedFolder;
			if (selectedFolder == null)
			{
				return;
			}

			var folder = await foldersService.GetFolder(selectedFolder.Id, cancellationToken);

			if (folder.HasContent)
			{
				windowService.ShowMessageBox("You can not delete non-empty directory", "Warning", ShowMessageBoxButton.Ok, ShowMessageBoxIcon.Exclamation);
				return;
			}

			if (windowService.ShowMessageBox($"Do you really want to delete the folder '{folder.Name}'?", "Delete folder",
				ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Question) != ShowMessageBoxResult.Yes)
			{
				return;
			}

			await foldersService.DeleteFolder(folder.Id, cancellationToken);

			ItemListViewModel.RemoveFolder(folder.Id);
		}

		private async Task DeleteDisc(CancellationToken cancellationToken)
		{
			var selectedDisc = SelectedDisc;
			if (selectedDisc == null)
			{
				return;
			}

			if (windowService.ShowMessageBox($"Do you really want to delete the selected disc '{selectedDisc.Title}'?", "Delete disc",
				ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Question) != ShowMessageBoxResult.Yes)
			{
				return;
			}

			// We are sending this event to release any disc images hold by DiscImageViewModel.
			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(null));
			await discsService.DeleteDisc(selectedDisc.Id, cancellationToken);

			ItemListViewModel.RemoveDisc(selectedDisc.Id);
		}

		private async void OnPlaySongsList(BaseSongListEventArgs e, CancellationToken cancellationToken)
		{
			if (e.Disc != null)
			{
				await SwitchToDisc(e.Disc, cancellationToken);
			}
		}

		private async void OnPlaylistLoaded(BaseSongListEventArgs e, CancellationToken cancellationToken)
		{
			if (e.Disc != null)
			{
				await SwitchToDisc(e.Disc, cancellationToken);
			}
			else
			{
				await LoadRootFolder(cancellationToken);
			}
		}

		private async void OnNoPlaylistLoaded(CancellationToken cancellationToken)
		{
			await LoadRootFolder(cancellationToken);
		}

		private async Task LoadRootFolder(CancellationToken cancellationToken)
		{
			var rootFolderData = await foldersService.GetRootFolder(cancellationToken);
			LoadFolder(rootFolderData);
		}

		private void OnSongChanged(SongModel changedSong, string propertyName)
		{
			foreach (var disc in GetDiscsForUpdate(changedSong.Disc))
			{
				var songsForUpdate = disc.AllSongs
					.Where(s => s.Id == changedSong.Id)
					.Where(s => !Object.ReferenceEquals(s, changedSong));

				foreach (var song in songsForUpdate)
				{
					SongUpdater.UpdateSong(changedSong, song, propertyName);
				}
			}
		}

		private void OnDiscChanged(DiscModel changedDisc, string propertyName)
		{
			foreach (var disc in GetDiscsForUpdate(changedDisc))
			{
				DiscUpdater.UpdateDisc(changedDisc, disc, propertyName);
			}
		}

		private void OnDiscImageChanged(DiscModel changedDisc)
		{
			foreach (var disc in GetDiscsForUpdate(changedDisc))
			{
				disc.Images = changedDisc.Images;
			}
		}

		private IEnumerable<DiscModel> GetDiscsForUpdate(DiscModel changedDisc)
		{
			return ItemListViewModel.Discs
				.Where(d => d.Id == changedDisc.Id)
				.Where(d => !Object.ReferenceEquals(d, changedDisc));
		}

		private async void OnSwitchToDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await SwitchToDisc(disc, cancellationToken);
		}
	}
}
