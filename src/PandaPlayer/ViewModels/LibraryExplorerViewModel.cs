﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Internal;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.ViewModels
{
	public class LibraryExplorerViewModel : ViewModelBase, ILibraryExplorerViewModel
	{
		private readonly IDiscSongListViewModel discSongListViewModel;

		private readonly IFoldersService foldersService;

		private readonly IDiscsService discsService;

		private readonly IViewNavigator viewNavigator;

		private readonly IWindowService windowService;

		public ObservableCollection<BasicExplorerItem> Items { get; } = new();

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
					discSongListViewModel.SetSongs(selectedDisc.ActiveSongs);
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(selectedDisc));
				}
				else
				{
					discSongListViewModel.SetSongs(Enumerable.Empty<SongModel>());
					Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(null));
				}
			}
		}

		public DiscModel SelectedDisc => (selectedItem as DiscExplorerItem)?.Disc;

		public ICommand ChangeFolderCommand { get; }

		public ICommand PlayDiscCommand { get; }

		public ICommand AddDiscToPlaylistCommand { get; }

		public ICommand DeleteDiscCommand { get; }

		public ICommand JumpToFirstItemCommand { get; }

		public ICommand JumpToLastItemCommand { get; }

		public ICommand EditDiscPropertiesCommand { get; }

		public ICommand DeleteFolderCommand { get; }

		public LibraryExplorerViewModel(IDiscSongListViewModel songListListViewModel, IFoldersService foldersService, IDiscsService discsService,
			IViewNavigator viewNavigator, IWindowService windowService)
		{
			this.discSongListViewModel = songListListViewModel ?? throw new ArgumentNullException(nameof(songListListViewModel));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			ChangeFolderCommand = new AsyncRelayCommand(() => ChangeToCurrentlySelectedFolder(CancellationToken.None));
			PlayDiscCommand = new RelayCommand(PlayDisc);
			AddDiscToPlaylistCommand = new RelayCommand(AddDiscToPlaylist);
			DeleteDiscCommand = new AsyncRelayCommand(() => DeleteDisc(CancellationToken.None));
			JumpToFirstItemCommand = new RelayCommand(() => SelectedItem = Items.FirstOrDefault());
			JumpToLastItemCommand = new RelayCommand(() => SelectedItem = Items.LastOrDefault());
			EditDiscPropertiesCommand = new RelayCommand(EditDiscProperties);
			DeleteFolderCommand = new AsyncRelayCommand(() => DeleteFolder(CancellationToken.None));

			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => OnPlaylistChanged(e, CancellationToken.None));
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => OnPlaylistLoaded(e, CancellationToken.None));
			Messenger.Default.Register<NoPlaylistLoadedEventArgs>(this, e => OnNoPlaylistLoaded(CancellationToken.None));
			Messenger.Default.Register<SongChangedEventArgs>(this, e => OnSongChanged(e.Song, e.PropertyName));
			Messenger.Default.Register<DiscChangedEventArgs>(this, e => OnDiscChanged(e.Disc, e.PropertyName));
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => OnDiscImageChanged(e.Disc));
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

			ParentFolderId = folder.ParentFolder?.Id;
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

			if (folder.ParentFolder != null)
			{
				Items.Add(new ParentFolderExplorerItem());
			}

			var subfolders = folder.Subfolders
				.Where(sf => !sf.IsDeleted)
				.Select(sf => new FolderExplorerItem(sf))
				.OrderBy(sf => sf.Title, StringComparer.OrdinalIgnoreCase);

			var discs = folder.Discs
				.Where(disc => !disc.IsDeleted)
				.Select(disc => new DiscExplorerItem(disc))
				.OrderBy(disc => disc.Title, StringComparer.OrdinalIgnoreCase);

			Items.AddRange(subfolders);
			Items.AddRange(discs);
		}

		public async Task SwitchToDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			var discFolder = await foldersService.GetFolder(disc.Folder.Id, cancellationToken);

			LoadFolder(discFolder);

			SelectedItem = Items.OfType<DiscExplorerItem>().FirstOrDefault(x => x.DiscId == disc.Id);
		}

		private void PlayDisc()
		{
			if (SelectedItem is not DiscExplorerItem discItem)
			{
				return;
			}

			Messenger.Default.Send(new PlaySongsListEventArgs(discItem.Disc));
		}

		private void AddDiscToPlaylist()
		{
			if (SelectedItem is not DiscExplorerItem discItem)
			{
				return;
			}

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(discItem.Disc.ActiveSongs));
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

		private async Task DeleteFolder(CancellationToken cancellationToken)
		{
			if (!(SelectedItem is FolderExplorerItem folderItem))
			{
				return;
			}

			var folder = await foldersService.GetFolder(folderItem.FolderId, cancellationToken);

			if (folder.HasContent)
			{
				windowService.ShowMessageBox("You can not delete non-empty directory", "Warning", ShowMessageBoxButton.Ok, ShowMessageBoxIcon.Exclamation);
				return;
			}

			if (windowService.ShowMessageBox($"Do you really want to delete the folder '{folderItem.Title}'?", "Delete folder",
				ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Question) != ShowMessageBoxResult.Yes)
			{
				return;
			}

			await foldersService.DeleteFolder(folderItem.FolderId, cancellationToken);

			Items.Remove(folderItem);
		}

		private async void OnPlaylistChanged(BaseSongListEventArgs e, CancellationToken cancellationToken)
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
			foreach (var disc in GetMatchingDiscs(changedSong.Disc))
			{
				foreach (var song in disc.AllSongs.Where(s => s.Id == changedSong.Id))
				{
					SongUpdater.UpdateSong(changedSong, song, propertyName);
				}
			}
		}

		private void OnDiscChanged(DiscModel changedDisc, string propertyName)
		{
			foreach (var disc in GetMatchingDiscs(changedDisc))
			{
				DiscUpdater.UpdateDisc(changedDisc, disc, propertyName);
			}
		}

		private void OnDiscImageChanged(DiscModel changedDisc)
		{
			foreach (var disc in GetMatchingDiscs(changedDisc))
			{
				disc.Images = changedDisc.Images;
			}
		}

		private IEnumerable<DiscModel> GetMatchingDiscs(DiscModel disc)
		{
			return Items
				.OfType<DiscExplorerItem>()
				.Where(x => x.DiscId == disc.Id)
				.Select(x => x.Disc)
				.Where(d => !Object.ReferenceEquals(d, disc));
		}
	}
}
