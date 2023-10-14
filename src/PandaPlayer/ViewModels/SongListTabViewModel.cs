using System;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class SongListTabViewModel : ObservableObject, ISongListTabViewModel
	{
		private readonly ILibraryExplorerViewModel libraryExplorerViewModel;

		private readonly IMessenger messenger;

		public IDiscSongListViewModel DiscSongListViewModel { get; }

		public IPlaylistViewModel PlaylistViewModel { get; }

		private ISongListViewModel currentSongListViewModel;

		private ISongListViewModel CurrentSongListViewModel
		{
			get => currentSongListViewModel;
			set
			{
				SetProperty(ref currentSongListViewModel, value);
				OnPropertyChanged(nameof(IsDiscSongListSelected));
				OnPropertyChanged(nameof(IsPlaylistSelected));

				ActiveDisc = IsDiscSongListSelected ? libraryExplorerViewModel.SelectedDisc : PlaylistCurrentDisc;
			}
		}

		public bool IsDiscSongListSelected => CurrentSongListViewModel == DiscSongListViewModel;

		public bool IsPlaylistSelected => CurrentSongListViewModel == PlaylistViewModel;

		private DiscModel PlaylistCurrentDisc => PlaylistViewModel.CurrentDisc;

		private DiscModel activeDisc;

		private DiscModel ActiveDisc
		{
			set
			{
				if (activeDisc != value)
				{
					activeDisc = value;
					messenger.Send(new ActiveDiscChangedEventArgs(activeDisc));
				}
			}
		}

		public ICommand SwitchToDiscSongListCommand { get; }

		public ICommand SwitchToPlaylistCommand { get; }

		public SongListTabViewModel(IDiscSongListViewModel discSongListViewModel, IPlaylistViewModel playlistViewModel, ILibraryExplorerViewModel libraryExplorerViewModel, IMessenger messenger)
		{
			DiscSongListViewModel = discSongListViewModel ?? throw new ArgumentNullException(nameof(discSongListViewModel));
			PlaylistViewModel = playlistViewModel ?? throw new ArgumentNullException(nameof(playlistViewModel));
			this.libraryExplorerViewModel = libraryExplorerViewModel ?? throw new ArgumentNullException(nameof(libraryExplorerViewModel));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			SwitchToDiscSongListCommand = new RelayCommand(SwitchToDiscSongList);
			SwitchToPlaylistCommand = new RelayCommand(SwitchToPlaylist);

			messenger.Register<ApplicationLoadedEventArgs>(this, (_, _) => Load());
			messenger.Register<LibraryExplorerDiscChangedEventArgs>(this, (_, _) => SwitchToDiscSongList());
			messenger.Register<PlaySongsListEventArgs>(this, (_, _) => SwitchToPlaylist());
			messenger.Register<PlaylistLoadedEventArgs>(this, (_, _) => OnPlaylistSongChanged());
			messenger.Register<PlaylistChangedEventArgs>(this, (_, _) => OnPlaylistSongChanged());

			// Showing DiscSongList by default
			CurrentSongListViewModel = DiscSongListViewModel;
		}

		private void Load()
		{
			// Setting playlist active if some previous playlist was loaded.
			if (PlaylistViewModel.Songs.Any())
			{
				SwitchToPlaylist();
			}
		}

		private void OnPlaylistSongChanged()
		{
			if (IsPlaylistSelected)
			{
				ActiveDisc = PlaylistCurrentDisc;
			}
		}

		private void SwitchToDiscSongList()
		{
			CurrentSongListViewModel = DiscSongListViewModel;
		}

		private void SwitchToPlaylist()
		{
			CurrentSongListViewModel = PlaylistViewModel;
		}
	}
}
