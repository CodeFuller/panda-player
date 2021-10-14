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
using PandaPlayer.Events;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.LibraryExplorerEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Internal;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels
{
	public class LibraryExplorerViewModel : ViewModelBase, ILibraryExplorerViewModel
	{
		private readonly IFoldersService foldersService;

		private readonly IAdviseGroupHelper adviseGroupHelper;

		private readonly IViewNavigator viewNavigator;

		private readonly IWindowService windowService;

		public ILibraryExplorerItemListViewModel ItemListViewModel { get; }

		public ShallowFolderModel SelectedFolder => ItemListViewModel.SelectedFolder;

		public DiscModel SelectedDisc => ItemListViewModel.SelectedDisc;

		public IReadOnlyCollection<BasicMenuItem> AdviseGroupMenuItems
		{
			get
			{
				BasicAdviseGroupHolder adviseGroupHolder;

				if (SelectedFolder != null)
				{
					adviseGroupHolder = new FolderAdviseGroupHolder(SelectedFolder);
				}
				else if (SelectedDisc != null)
				{
					adviseGroupHolder = new DiscAdviseGroupHolder(SelectedDisc);
				}
				else
				{
					return new List<BasicMenuItem>();
				}

				var menuItems = new List<BasicMenuItem>
				{
					new NewAdviseGroupMenuItem(ct => CreateAdviseGroup(adviseGroupHolder, ct)),
				};

				if (adviseGroupHolder.CurrentAdviseGroup != null)
				{
					var reverseFavoriteMenuItem = new ReverseFavoriteStatusForAdviseGroupMenuItem(
						adviseGroupHolder.CurrentAdviseGroup, (g, ct) => adviseGroupHelper.ReverseFavoriteStatus(g, ct));

					menuItems.Add(reverseFavoriteMenuItem);
				}

				var adviseGroups = adviseGroupHelper.AdviseGroups;
				if (adviseGroups.Any())
				{
					menuItems.Add(new SeparatorMenuItem());

					var currentAdviseGroupId = adviseGroupHolder.CurrentAdviseGroup?.Id;
					menuItems.AddRange(adviseGroups.Select(x => new SetAdviseGroupMenuItem(x, x.Id == currentAdviseGroupId, ct => adviseGroupHelper.ReverseAdviseGroup(adviseGroupHolder, x, ct))));
				}

				return menuItems;
			}
		}

		public ICommand PlayDiscCommand { get; }

		public ICommand AddDiscToPlaylistCommand { get; }

		public ICommand EditDiscPropertiesCommand { get; }

		public ICommand DeleteFolderCommand { get; }

		public ICommand DeleteDiscCommand { get; }

		public LibraryExplorerViewModel(ILibraryExplorerItemListViewModel itemListViewModel, IFoldersService foldersService,
			IAdviseGroupHelper adviseGroupHelper, IViewNavigator viewNavigator, IWindowService windowService)
		{
			ItemListViewModel = itemListViewModel ?? throw new ArgumentNullException(nameof(itemListViewModel));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.adviseGroupHelper = adviseGroupHelper ?? throw new ArgumentNullException(nameof(adviseGroupHelper));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			PlayDiscCommand = new RelayCommand(PlayDisc);
			AddDiscToPlaylistCommand = new RelayCommand(AddDiscToPlaylist);
			EditDiscPropertiesCommand = new RelayCommand(EditDiscProperties);
			DeleteFolderCommand = new AsyncRelayCommand(() => DeleteFolder(CancellationToken.None));
			DeleteDiscCommand = new RelayCommand(DeleteDisc);

			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, e => OnApplicationLoaded(CancellationToken.None));
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

		private async void OnApplicationLoaded(CancellationToken cancellationToken)
		{
			await adviseGroupHelper.Load(cancellationToken);
		}

		private async Task CreateAdviseGroup(BasicAdviseGroupHolder adviseGroupHolder, CancellationToken cancellationToken)
		{
			var newAdviseGroupName = viewNavigator.ShowCreateAdviseGroupView(adviseGroupHolder.InitialAdviseGroupName, adviseGroupHelper.AdviseGroups.Select(x => x.Name));
			if (newAdviseGroupName == null)
			{
				return;
			}

			await adviseGroupHelper.CreateAdviseGroup(adviseGroupHolder, newAdviseGroupName, cancellationToken);
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
			var selectedFolder = SelectedFolder;
			if (selectedFolder == null)
			{
				return;
			}

			var folder = await foldersService.GetFolder(selectedFolder.Id, cancellationToken);

			if (folder.HasContent)
			{
				windowService.ShowMessageBox("You can not delete non-empty folder", "Warning", ShowMessageBoxButton.Ok, ShowMessageBoxIcon.Exclamation);
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

		private void DeleteDisc()
		{
			var selectedDisc = SelectedDisc;
			if (selectedDisc == null)
			{
				return;
			}

			if (!viewNavigator.ShowDeleteDiscView(selectedDisc))
			{
				return;
			}

			ItemListViewModel.RemoveDisc(selectedDisc.Id);
		}

		private async void OnPlaySongsList(PlaySongsListEventArgs e, CancellationToken cancellationToken)
		{
			var disc = e.Disc;
			if (disc != null)
			{
				await SwitchToDisc(disc, cancellationToken);
			}
		}

		private async void OnPlaylistLoaded(PlaylistLoadedEventArgs e, CancellationToken cancellationToken)
		{
			var disc = e.Disc;
			if (disc != null)
			{
				await SwitchToDisc(disc, cancellationToken);
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
