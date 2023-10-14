using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
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
	public class LibraryExplorerViewModel : ObservableObject, ILibraryExplorerViewModel
	{
		private readonly IFoldersService foldersService;

		private readonly IAdviseGroupHelper adviseGroupHelper;

		private readonly IViewNavigator viewNavigator;

		private readonly IWindowService windowService;

		private readonly IMessenger messenger;

		public ILibraryExplorerItemListViewModel ItemListViewModel { get; }

		public DiscModel SelectedDisc => ItemListViewModel.SelectedDisc;

		public IEnumerable<BasicMenuItem> ContextMenuItemsForSelectedItem => ItemListViewModel.SelectedItem?.GetContextMenuItems(this, adviseGroupHelper) ?? Enumerable.Empty<BasicMenuItem>();

		public LibraryExplorerViewModel(ILibraryExplorerItemListViewModel itemListViewModel, IFoldersService foldersService,
			IAdviseGroupHelper adviseGroupHelper, IViewNavigator viewNavigator, IWindowService windowService, IMessenger messenger)
		{
			ItemListViewModel = itemListViewModel ?? throw new ArgumentNullException(nameof(itemListViewModel));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.adviseGroupHelper = adviseGroupHelper ?? throw new ArgumentNullException(nameof(adviseGroupHelper));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			messenger.Register<ApplicationLoadedEventArgs>(this, (_, _) => OnApplicationLoaded(CancellationToken.None));
			messenger.Register<LoadParentFolderEventArgs>(this, (_, e) => OnLoadParentFolder(e));
			messenger.Register<LoadFolderEventArgs>(this, (_, e) => OnLoadFolder(e.Folder));
			messenger.Register<PlaySongsListEventArgs>(this, (_, e) => OnPlaySongsList(e));
			messenger.Register<PlaylistLoadedEventArgs>(this, (_, e) => OnPlaylistLoaded(e, CancellationToken.None));
			messenger.Register<NoPlaylistLoadedEventArgs>(this, (_, _) => OnNoPlaylistLoaded(CancellationToken.None));
			messenger.Register<NavigateLibraryExplorerToDiscEventArgs>(this, (_, e) => OnSwitchToDisc(e.Disc));
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
			messenger.Send(new PlaySongsListEventArgs(disc));
		}

		public void AddDiscToPlaylist(DiscModel disc)
		{
			messenger.Send(new AddingSongsToPlaylistLastEventArgs(disc.ActiveSongs));
		}

		public void EditDiscProperties(DiscModel disc)
		{
			viewNavigator.ShowDiscPropertiesView(disc);
		}

		public void RenameFolder(FolderModel folder)
		{
			viewNavigator.ShowRenameFolderView(folder);
		}

		public async Task DeleteFolder(FolderModel folder, CancellationToken cancellationToken)
		{
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

			await foldersService.DeleteEmptyFolder(folder, cancellationToken);

			await ItemListViewModel.OnFolderDeleted(folder.Id);
		}

		public async Task DeleteDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			if (!viewNavigator.ShowDeleteDiscView(disc))
			{
				return;
			}

			await ItemListViewModel.OnDiscDeleted(disc.Id);
		}

		private void OnLoadParentFolder(LoadParentFolderEventArgs e)
		{
			LoadFolder(e.ParentFolder);

			ItemListViewModel.SelectFolder(e.ChildFolderId);
		}

		private void OnLoadFolder(FolderModel folder)
		{
			LoadFolder(folder);
		}

		private void LoadFolder(FolderModel folder)
		{
			ItemListViewModel.LoadFolderItems(folder);
		}

		private void SwitchToDisc(DiscModel disc)
		{
			LoadFolder(disc.Folder);

			ItemListViewModel.SelectDisc(disc.Id);
		}

		private void OnPlaySongsList(PlaySongsListEventArgs e)
		{
			var disc = e.Disc;
			if (disc != null)
			{
				SwitchToDisc(disc);
			}
		}

		private async void OnPlaylistLoaded(PlaylistLoadedEventArgs e, CancellationToken cancellationToken)
		{
			var disc = e.Disc;
			if (disc != null)
			{
				SwitchToDisc(disc);
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

		private void OnSwitchToDisc(DiscModel disc)
		{
			SwitchToDisc(disc);
		}
	}
}
