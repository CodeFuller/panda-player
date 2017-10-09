using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class DiscAdviserViewModel : ViewModelBase, IDiscAdviserViewModel
	{
		private const int AdvisedPlaylistsNumber = 30;

		private readonly DiscLibrary discLibrary;
		private readonly ICompositePlaylistAdviser playlistAdviser;

		private readonly List<AdvisedPlaylist> currAdvises = new List<AdvisedPlaylist>();

		private int currAdviseIndex;
		private int CurrAdviseIndex
		{
			get { return currAdviseIndex; }
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

		public DiscAdviserViewModel(DiscLibrary discLibrary, ICompositePlaylistAdviser playlistAdviser)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}
			if (playlistAdviser == null)
			{
				throw new ArgumentNullException(nameof(playlistAdviser));
			}

			this.discLibrary = discLibrary;
			this.playlistAdviser = playlistAdviser;

			PlayCurrentAdviseCommand = new RelayCommand(PlayCurrentAdvise);
			SwitchToNextAdviseCommand = new RelayCommand(SwitchToNextAdvise);

			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => OnPlaylistFinished(e.Songs));
			Messenger.Default.Register<LibraryLoadedEventArgs>(this, e => Load());
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
			currAdvises.AddRange(playlistAdviser.Advise(discLibrary).Take(AdvisedPlaylistsNumber));
			CurrAdviseIndex = 0;
		}

		private void OnPlaylistFinished(IEnumerable<Song> finishedSongs)
		{
			List<Song> finishedSongsList = finishedSongs.ToList();

			//	Removing advises that could be covered by just finished playlist.
			bool currAdviseChanged = false;
			for (var i = currAdviseIndex; i < currAdvises.Count; )
			{
				if (SongListCoversAdvise(finishedSongsList, currAdvises[i]))
				{
					currAdvises.RemoveAt(i);
					currAdviseChanged = currAdviseChanged || (i == currAdviseIndex);
				}
				else
				{
					++i;
				}
			}

			if (currAdviseChanged)
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

		private static bool SongListCoversAdvise(List<Song> songs, AdvisedPlaylist advise)
		{
			return advise.Songs.All(songs.Contains);
		}
	}
}
