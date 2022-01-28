﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.LibraryExplorerEvents;
using PandaPlayer.Events.SongListEvents;
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

		public DiscModel SelectedDisc => ItemListViewModel.SelectedDisc;

		public IEnumerable<BasicMenuItem> ContextMenuItemsForSelectedItem => ItemListViewModel.SelectedItem?.GetContextMenuItems(this, adviseGroupHelper) ?? Enumerable.Empty<BasicMenuItem>();

		public LibraryExplorerViewModel(ILibraryExplorerItemListViewModel itemListViewModel, IFoldersService foldersService,
			IAdviseGroupHelper adviseGroupHelper, IViewNavigator viewNavigator, IWindowService windowService)
		{
			ItemListViewModel = itemListViewModel ?? throw new ArgumentNullException(nameof(itemListViewModel));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.adviseGroupHelper = adviseGroupHelper ?? throw new ArgumentNullException(nameof(adviseGroupHelper));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, e => OnApplicationLoaded(CancellationToken.None));
			Messenger.Default.Register<LoadParentFolderEventArgs>(this, e => OnLoadParentFolder(e, CancellationToken.None));
			Messenger.Default.Register<LoadFolderEventArgs>(this, e => OnLoadFolder(e.FolderId, CancellationToken.None));
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => OnPlaySongsList(e, CancellationToken.None));
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => OnPlaylistLoaded(e, CancellationToken.None));
			Messenger.Default.Register<NoPlaylistLoadedEventArgs>(this, _ => OnNoPlaylistLoaded(CancellationToken.None));
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => OnSwitchToDisc(e.Disc, CancellationToken.None));
		}

		private async void OnApplicationLoaded(CancellationToken cancellationToken)
		{
			await adviseGroupHelper.Load(cancellationToken);
		}

		public async Task CreateAdviseGroup(BasicAdviseGroupHolder adviseGroupHolder, CancellationToken cancellationToken)
		{
			var newAdviseGroupName = viewNavigator.ShowCreateAdviseGroupView(adviseGroupHolder.InitialAdviseGroupName, adviseGroupHelper.AdviseGroups.Select(x => x.Name));
			if (newAdviseGroupName == null)
			{
				return;
			}

			await adviseGroupHelper.CreateAdviseGroup(adviseGroupHolder, newAdviseGroupName, cancellationToken);
		}

		public void PlayDisc(DiscModel disc)
		{
			Messenger.Default.Send(new PlaySongsListEventArgs(disc));
		}

		public void AddDiscToPlaylist(DiscModel disc)
		{
			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(disc.ActiveSongs));
		}

		public void EditDiscProperties(DiscModel disc)
		{
			viewNavigator.ShowDiscPropertiesView(disc);
		}

		public void RenameFolder(ShallowFolderModel folder)
		{
			viewNavigator.ShowRenameFolderView(folder);
		}

		public async Task DeleteFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			var folder = await foldersService.GetFolder(folderId, cancellationToken);

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

			await ItemListViewModel.OnFolderDeleted(folder.Id, id => foldersService.GetFolder(id, cancellationToken));
		}

		public async Task DeleteDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			if (!viewNavigator.ShowDeleteDiscView(disc))
			{
				return;
			}

			await ItemListViewModel.OnDiscDeleted(disc.Id, id => foldersService.GetFolder(id, cancellationToken));
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

		private async void OnSwitchToDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await SwitchToDisc(disc, cancellationToken);
		}
	}
}
