using System;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class SongListTabViewModel : ViewModelBase, ISongListTabViewModel
	{
		public IDiscSongListViewModel DiscSongListViewModel { get; }

		public IPlaylistViewModel PlaylistViewModel { get; }

		private readonly ILibraryExplorerViewModel libraryExplorerViewModel;

		private ISongListViewModel currentSongListViewModel;

		private ISongListViewModel CurrentSongListViewModel
		{
			get => currentSongListViewModel;
			set
			{
				Set(ref currentSongListViewModel, value);
				RaisePropertyChanged(nameof(IsDiscSongListSelected));
				RaisePropertyChanged(nameof(IsPlaylistSelected));

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
					Messenger.Default.Send(new ActiveDiscChangedEventArgs(activeDisc));
				}
			}
		}

		public ICommand SwitchToDiscSongListCommand { get; }

		public ICommand SwitchToPlaylistCommand { get; }

		public SongListTabViewModel(IDiscSongListViewModel discSongListViewModel, IPlaylistViewModel playlistViewModel, ILibraryExplorerViewModel libraryExplorerViewModel)
		{
			DiscSongListViewModel = discSongListViewModel ?? throw new ArgumentNullException(nameof(discSongListViewModel));
			PlaylistViewModel = playlistViewModel ?? throw new ArgumentNullException(nameof(playlistViewModel));
			this.libraryExplorerViewModel = libraryExplorerViewModel ?? throw new ArgumentNullException(nameof(libraryExplorerViewModel));

			SwitchToDiscSongListCommand = new RelayCommand(SwitchToDiscSongList);
			SwitchToPlaylistCommand = new RelayCommand(SwitchToPlaylist);

			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, _ => Load());
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, _ => SwitchToDiscSongList());
			Messenger.Default.Register<PlaySongsListEventArgs>(this, _ => SwitchToPlaylist());
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, _ => OnPlaylistSongChanged());
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, _ => OnPlaylistSongChanged());

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
