using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
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
	internal class PlaylistAdviserViewModel : ViewModelBase, IPlaylistAdviserViewModel
	{
		private const int AdvisedPlaylistsNumber = 30;

		private readonly IDiscsService discsService;

		private readonly ICompositePlaylistAdviser playlistAdviser;

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

		public PlaylistAdviserViewModel(IDiscsService discsService, ICompositePlaylistAdviser playlistAdviser, ILogger<PlaylistAdviserViewModel> logger)
		{
			this.playlistAdviser = playlistAdviser ?? throw new ArgumentNullException(nameof(playlistAdviser));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));

			PlayCurrentAdviseCommand = new AsyncRelayCommand(() => PlayCurrentAdvise(CancellationToken.None));
			SwitchToNextAdviseCommand = new AsyncRelayCommand(() => SwitchToNextAdvise(CancellationToken.None));

			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => OnPlaylistFinished(e.Songs, CancellationToken.None));
			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, e => Load(CancellationToken.None));
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
				Messenger.Default.Send(new PlaySongsListEventArgs(advise.Songs));
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
			RaisePropertyChanged(nameof(CurrentAdviseAnnouncement));
		}

		private static bool SongListCoversAdvise(List<SongModel> songs, AdvisedPlaylist advise)
		{
			var songListIds = songs.Select(s => s.Id).Distinct();
			var adviseIds = advise.Songs.Select(s => s.Id).Distinct();

			return !adviseIds.Except(songListIds).Any();
		}
	}
}
