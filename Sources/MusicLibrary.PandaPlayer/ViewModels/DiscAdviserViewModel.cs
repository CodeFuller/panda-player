using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Logic.Models;
using MusicLibrary.PandaPlayer.Adviser;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class DiscAdviserViewModel : ViewModelBase, IDiscAdviserViewModel
	{
		private const int AdvisedPlaylistsNumber = 30;

		private readonly ICompositePlaylistAdviser playlistAdviser;

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

		public DiscAdviserViewModel(ICompositePlaylistAdviser playlistAdviser)
		{
			this.playlistAdviser = playlistAdviser ?? throw new ArgumentNullException(nameof(playlistAdviser));

			PlayCurrentAdviseCommand = new RelayCommand(PlayCurrentAdvise);
			SwitchToNextAdviseCommand = new RelayCommand(SwitchToNextAdvise);

			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => OnPlaylistFinished(e.Songs));
			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, e => Load());
		}

		private void Load()
		{
			RebuildAdvises();
		}

		internal void PlayCurrentAdvise()
		{
			var advise = CurrentAdvise;
			if (advise != null)
			{
				playlistAdviser.RegisterAdvicePlayback(advise);
				Messenger.Default.Send(new PlaySongsListEventArgs(advise.Songs));
			}
		}

		internal void SwitchToNextAdvise()
		{
			++CurrAdviseIndex;
			RebuildAdvisesIfRequired();
		}

		private void RebuildAdvisesIfRequired()
		{
			if (CurrAdviseIndex >= currAdvises.Count)
			{
				RebuildAdvises();
			}
		}

		private void RebuildAdvises()
		{
			currAdvises.Clear();
			currAdvises.AddRange(playlistAdviser.Advise().Take(AdvisedPlaylistsNumber));
			CurrAdviseIndex = 0;
		}

		private void OnPlaylistFinished(IEnumerable<SongModel> finishedSongs)
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

			RebuildAdvisesIfRequired();
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
