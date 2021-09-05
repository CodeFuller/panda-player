using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	internal class ApplicationViewModel : ViewModelBase
	{
		private const string DefaultTitle = "Panda Player";

		private readonly IViewNavigator viewNavigator;

		private string title = DefaultTitle;

		public string Title
		{
			get => title;
			private set => Set(ref title, value);
		}

		public ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		public ISongListTabViewModel SongListTabViewModel { get; }

		public IPlaylistAdviserViewModel PlaylistAdviserViewModel { get; }

		public IDiscImageViewModel DiscImageViewModel { get; }

		public IPlaylistPlayerViewModel PlaylistPlayerViewModel { get; }

		public ILoggerViewModel LoggerViewModel { get; }

		public ICommand LoadCommand { get; }

		public ICommand ReversePlayingCommand { get; }

		public ICommand ShowAdviseSetsEditorCommand { get; }

		public ICommand ShowDiscAdderCommand { get; }

		public ICommand ShowLibraryCheckerCommand { get; }

		public ICommand ShowLibraryStatisticsCommand { get; }

		public ApplicationViewModel(ILibraryExplorerViewModel libraryExplorerViewModel, ISongListTabViewModel songListTabViewModel, IPlaylistAdviserViewModel playlistAdviserViewModel,
			IDiscImageViewModel discImageViewModel, IPlaylistPlayerViewModel playlistPlayerViewModel, IViewNavigator viewNavigator, ILoggerViewModel loggerViewModel)
		{
			LibraryExplorerViewModel = libraryExplorerViewModel ?? throw new ArgumentNullException(nameof(libraryExplorerViewModel));
			SongListTabViewModel = songListTabViewModel ?? throw new ArgumentNullException(nameof(songListTabViewModel));
			PlaylistAdviserViewModel = playlistAdviserViewModel ?? throw new ArgumentNullException(nameof(playlistAdviserViewModel));
			DiscImageViewModel = discImageViewModel ?? throw new ArgumentNullException(nameof(discImageViewModel));
			PlaylistPlayerViewModel = playlistPlayerViewModel ?? throw new ArgumentNullException(nameof(playlistPlayerViewModel));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			LoggerViewModel = loggerViewModel ?? throw new ArgumentNullException(nameof(loggerViewModel));

			LoadCommand = new RelayCommand(Load);
			ReversePlayingCommand = new AsyncRelayCommand(() => ReversePlaying(CancellationToken.None));
			ShowAdviseSetsEditorCommand = new AsyncRelayCommand(() => ShowAdviseSetsEditor(CancellationToken.None));
			ShowDiscAdderCommand = new AsyncRelayCommand(() => ShowDiscAdder(CancellationToken.None));
			ShowLibraryCheckerCommand = new AsyncRelayCommand(() => ShowLibraryChecker(CancellationToken.None));
			ShowLibraryStatisticsCommand = new AsyncRelayCommand(() => ShowLibraryStatistics(CancellationToken.None));

			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, OnPlaylistSongChanged);
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, OnPlaylistSongChanged);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, OnPlaylistFinished);
		}

		private static void Load()
		{
			Messenger.Default.Send(new ApplicationLoadedEventArgs());
		}

		private Task ReversePlaying(CancellationToken cancellationToken)
		{
			return PlaylistPlayerViewModel.ReversePlaying(cancellationToken);
		}

		private async Task ShowAdviseSetsEditor(CancellationToken cancellationToken)
		{
			await viewNavigator.ShowAdviseSetsEditorView(cancellationToken);
		}

		private async Task ShowDiscAdder(CancellationToken cancellationToken)
		{
			await viewNavigator.ShowDiscAdderView(cancellationToken);
		}

		private async Task ShowLibraryChecker(CancellationToken cancellationToken)
		{
			await viewNavigator.ShowLibraryCheckerView(cancellationToken);
		}

		private async Task ShowLibraryStatistics(CancellationToken cancellationToken)
		{
			await viewNavigator.ShowLibraryStatisticsView(cancellationToken);
		}

		private void OnPlaylistSongChanged(BasicPlaylistEventArgs e)
		{
			Title = e.CurrentSong != null ? BuildCurrentTitle(e.CurrentSong, e.CurrentSongIndex, e.Songs.Count) : DefaultTitle;
		}

		private static string BuildCurrentTitle(SongModel song, int? playlistSongIndex, int songsCount)
		{
			var songTitle = song.Artist != null ? $"{song.Artist.Name} - {song.Title}" : song.Title;
			return $"{playlistSongIndex + 1}/{songsCount} - {songTitle}";
		}

		private void OnPlaylistFinished(PlaylistFinishedEventArgs e)
		{
			var songs = e.Songs;
			if (songs.All(s => s.Rating != null))
			{
				return;
			}

			viewNavigator.ShowRatePlaylistSongsView(songs);
		}
	}
}
