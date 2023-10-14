using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	internal class PlaylistAdviserViewModel : ObservableObject, IPlaylistAdviserViewModel
	{
		private const int AdvisedPlaylistsNumber = 30;

		private readonly IDiscsService discsService;

		private readonly ICompositePlaylistAdviser playlistAdviser;

		private readonly IMessenger messenger;

		private readonly ILogger<PlaylistAdviserViewModel> logger;

		private readonly List<AdvisedPlaylist> currentAdvises = new();

		private int currentAdviseIndex;

		private int CurrentAdviseIndex
		{
			get => currentAdviseIndex;
			set
			{
				currentAdviseIndex = value;
				OnCurrentAdvisedChanged();
			}
		}

		private AdvisedPlaylist CurrentAdvise => CurrentAdviseIndex < currentAdvises.Count ? currentAdvises[CurrentAdviseIndex] : null;

		public string CurrentAdviseAnnouncement => CurrentAdvise?.Title ?? "N/A";

		public ICommand PlayCurrentAdviseCommand { get; }

		public ICommand SwitchToNextAdviseCommand { get; }

		public PlaylistAdviserViewModel(IDiscsService discsService, ICompositePlaylistAdviser playlistAdviser, IMessenger messenger, ILogger<PlaylistAdviserViewModel> logger)
		{
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			this.playlistAdviser = playlistAdviser ?? throw new ArgumentNullException(nameof(playlistAdviser));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

			PlayCurrentAdviseCommand = new AsyncRelayCommand(() => PlayCurrentAdvise(CancellationToken.None));
			SwitchToNextAdviseCommand = new AsyncRelayCommand(() => SwitchToNextAdvise(CancellationToken.None));

			messenger.Register<PlaylistFinishedEventArgs>(this, (_, e) => OnPlaylistFinished(e.Songs, CancellationToken.None));
			messenger.Register<ApplicationLoadedEventArgs>(this, (_, _) => Load(CancellationToken.None));
		}

		private async void Load(CancellationToken cancellationToken)
		{
			await RebuildAdvises(cancellationToken);
		}

		private async Task PlayCurrentAdvise(CancellationToken cancellationToken)
		{
			var advise = CurrentAdvise;
			if (advise != null)
			{
				await playlistAdviser.RegisterAdvicePlayback(advise, cancellationToken);
				messenger.Send(new PlaySongsListEventArgs(advise.Songs));
			}
		}

		private async Task SwitchToNextAdvise(CancellationToken cancellationToken)
		{
			++CurrentAdviseIndex;
			await RebuildAdvisesIfRequired(cancellationToken);
		}

		private async Task RebuildAdvisesIfRequired(CancellationToken cancellationToken)
		{
			if (CurrentAdviseIndex >= currentAdvises.Count)
			{
				await RebuildAdvises(cancellationToken);
			}
		}

		private async Task RebuildAdvises(CancellationToken cancellationToken)
		{
			logger.LogInformation("Calculating advised playlists ...");
			var discs = await discsService.GetAllDiscs(cancellationToken);
			var advisedPlaylists = await playlistAdviser.Advise(discs, AdvisedPlaylistsNumber, cancellationToken);
			logger.LogInformation("Finished advised playlists calculation");

			currentAdvises.Clear();
			currentAdvises.AddRange(advisedPlaylists);
			CurrentAdviseIndex = 0;
		}

		private async void OnPlaylistFinished(IEnumerable<SongModel> finishedSongs, CancellationToken cancellationToken)
		{
			var finishedSongsList = finishedSongs.ToList();

			// Removing advises that could be covered by just finished playlist.
			var advisesWereModified = false;
			for (var i = currentAdviseIndex; i < currentAdvises.Count;)
			{
				if (SongListCoversAdvise(finishedSongsList, currentAdvises[i]))
				{
					currentAdvises.RemoveAt(i);
					advisesWereModified = advisesWereModified || (i == currentAdviseIndex);
				}
				else
				{
					++i;
				}
			}

			if (advisesWereModified)
			{
				OnCurrentAdvisedChanged();
			}

			await RebuildAdvisesIfRequired(cancellationToken);
		}

		protected void OnCurrentAdvisedChanged()
		{
			OnPropertyChanged(nameof(CurrentAdviseAnnouncement));
		}

		private static bool SongListCoversAdvise(List<SongModel> songs, AdvisedPlaylist advise)
		{
			var songListIds = songs.Select(s => s.Id).Distinct();
			var adviseIds = advise.Songs.Select(s => s.Id).Distinct();

			return !adviseIds.Except(songListIds).Any();
		}
	}
}
