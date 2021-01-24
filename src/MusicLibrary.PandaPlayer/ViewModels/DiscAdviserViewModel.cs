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
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	internal class DiscAdviserViewModel : ViewModelBase, IDiscAdviserViewModel
	{
		private const int AdvisedPlaylistsNumber = 30;

		private readonly IDiscsService discsService;

		private readonly ICompositePlaylistAdviser playlistAdviser;

		private readonly ILogger<DiscAdviserViewModel> logger;

		private readonly List<AdvisedPlaylist> currAdvises = new List<AdvisedPlaylist>();

		private int currAdviseIndex;

		private int CurrAdviseIndex
		{
			get => currAdviseIndex;
			set
			{
				currAdviseIndex = value;
				OnCurrentAdvisedChanged();
			}
		}

		public AdvisedPlaylist CurrentAdvise => CurrAdviseIndex < currAdvises.Count ? currAdvises[CurrAdviseIndex] : null;

		public string CurrentAdviseAnnouncement => CurrentAdvise?.Title ?? "N/A";

		public ICommand PlayCurrentAdviseCommand { get; }

		public ICommand SwitchToNextAdviseCommand { get; }

		public DiscAdviserViewModel(IDiscsService discsService, ICompositePlaylistAdviser playlistAdviser, ILogger<DiscAdviserViewModel> logger)
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
			++CurrAdviseIndex;
			await RebuildAdvisesIfRequired(cancellationToken);
		}

		private async Task RebuildAdvisesIfRequired(CancellationToken cancellationToken)
		{
			if (CurrAdviseIndex >= currAdvises.Count)
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

			currAdvises.Clear();
			currAdvises.AddRange(advisedPlaylists);
			CurrAdviseIndex = 0;
		}

		private async void OnPlaylistFinished(IEnumerable<SongModel> finishedSongs, CancellationToken cancellationToken)
		{
			var finishedSongsList = finishedSongs.ToList();

			// Removing advises that could be covered by just finished playlist.
			var advisesWereModified = false;
			for (var i = currAdviseIndex; i < currAdvises.Count;)
			{
				if (SongListCoversAdvise(finishedSongsList, currAdvises[i]))
				{
					currAdvises.RemoveAt(i);
					advisesWereModified = advisesWereModified || (i == currAdviseIndex);
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
			RaisePropertyChanged(nameof(CurrentAdvise));
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
