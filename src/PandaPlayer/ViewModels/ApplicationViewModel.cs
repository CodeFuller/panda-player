using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	internal class ApplicationViewModel : ObservableObject
	{
		private const string DefaultTitle = "Panda Player";

		private readonly IViewNavigator viewNavigator;

		private readonly IReadOnlyCollection<IApplicationInitializer> applicationInitializers;

		private readonly IMessenger messenger;

		private string title = DefaultTitle;

		public string Title
		{
			get => title;
			private set => SetProperty(ref title, value);
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

		public ApplicationViewModel(ILibraryExplorerViewModel libraryExplorerViewModel, ISongListTabViewModel songListTabViewModel,
			IPlaylistAdviserViewModel playlistAdviserViewModel, IDiscImageViewModel discImageViewModel, IPlaylistPlayerViewModel playlistPlayerViewModel,
			IViewNavigator viewNavigator, IEnumerable<IApplicationInitializer> applicationInitializers, ILoggerViewModel loggerViewModel, IMessenger messenger)
		{
			LibraryExplorerViewModel = libraryExplorerViewModel ?? throw new ArgumentNullException(nameof(libraryExplorerViewModel));
			SongListTabViewModel = songListTabViewModel ?? throw new ArgumentNullException(nameof(songListTabViewModel));
			PlaylistAdviserViewModel = playlistAdviserViewModel ?? throw new ArgumentNullException(nameof(playlistAdviserViewModel));
			DiscImageViewModel = discImageViewModel ?? throw new ArgumentNullException(nameof(discImageViewModel));
			PlaylistPlayerViewModel = playlistPlayerViewModel ?? throw new ArgumentNullException(nameof(playlistPlayerViewModel));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.applicationInitializers = applicationInitializers?.ToList() ?? throw new ArgumentNullException(nameof(applicationInitializers));
			LoggerViewModel = loggerViewModel ?? throw new ArgumentNullException(nameof(loggerViewModel));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			LoadCommand = new AsyncRelayCommand(() => Load(CancellationToken.None));
			ReversePlayingCommand = new AsyncRelayCommand(() => ReversePlaying(CancellationToken.None));
			ShowAdviseSetsEditorCommand = new AsyncRelayCommand(() => ShowAdviseSetsEditor(CancellationToken.None));
			ShowDiscAdderCommand = new AsyncRelayCommand(() => ShowDiscAdder(CancellationToken.None));
			ShowLibraryCheckerCommand = new AsyncRelayCommand(() => ShowLibraryChecker(CancellationToken.None));
			ShowLibraryStatisticsCommand = new AsyncRelayCommand(() => ShowLibraryStatistics(CancellationToken.None));

			messenger.Register<PlaylistLoadedEventArgs>(this, (_, e) => OnPlaylistSongChanged(e));
			messenger.Register<PlaylistChangedEventArgs>(this, (_, e) => OnPlaylistSongChanged(e));
			messenger.Register<PlaylistFinishedEventArgs>(this, (_, e) => OnPlaylistFinished(e));
		}

		private async Task Load(CancellationToken cancellationToken)
		{
			foreach (var applicationInitializer in applicationInitializers)
			{
				await applicationInitializer.Initialize(cancellationToken);
			}

			messenger.Send(new ApplicationLoadedEventArgs());
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
