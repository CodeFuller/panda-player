using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Player
{
	public class MusicPlayerViewModel : ViewModelBase, IMusicPlayerViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly ISongPlaybacksRegistrator playbacksRegistrator;
		private readonly IAudioPlayer audioPlayer;

		private bool isPlaying;
		public bool IsPlaying
		{
			get { return isPlaying; }
			set { Set(ref isPlaying, value); }
		}

		public TimeSpan CurrSongLength => audioPlayer.CurrSongLength;

		public TimeSpan CurrSongElapsed => audioPlayer.CurrSongPosition;

		public double CurrSongProgress
		{
			get
			{
				TimeSpan currSongElapsed = CurrSongElapsed;
				TimeSpan currSongLength = CurrSongLength;

				if (currSongLength == TimeSpan.Zero)
				{
					return 0;
				}

				return Math.Round(100 * currSongElapsed.TotalMilliseconds / currSongLength.TotalMilliseconds);
			}

			set
			{
				audioPlayer.CurrSongPosition = TimeSpan.FromMilliseconds(CurrSongLength.TotalMilliseconds * value);
				RaisePropertyChanged();
			}
		}

		public double Volume
		{
			get { return audioPlayer.Volume; }
			set
			{
				audioPlayer.Volume = value;
				RaisePropertyChanged();
			}
		}

		public ISongPlaylistViewModel Playlist { get; }

		/// <summary>
		/// The song that is currently loaded into audio player.
		/// </summary>
		public Song CurrentSong { get; private set; }

		public ICommand PlayCommand { get; }
		public ICommand PauseCommand { get; }

		public MusicPlayerViewModel(IMusicLibrary musicLibrary, ISongPlaylistViewModel playlist, IAudioPlayer audioPlayer, ISongPlaybacksRegistrator playbacksRegistrator)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (playlist == null)
			{
				throw new ArgumentNullException(nameof(playlist));
			}
			if (audioPlayer == null)
			{
				throw new ArgumentNullException(nameof(audioPlayer));
			}
			if (playbacksRegistrator == null)
			{
				throw new ArgumentNullException(nameof(playbacksRegistrator));
			}

			this.musicLibrary = musicLibrary;
			this.Playlist = playlist;
			this.audioPlayer = audioPlayer;
			this.playbacksRegistrator = playbacksRegistrator;

			this.audioPlayer.PropertyChanged += AudioPlayer_PropertyChanged;
			this.audioPlayer.SongMediaFinished += AudioPlayer_SongFinished;

			PlayCommand = new AsyncRelayCommand(Play);
			PauseCommand = new RelayCommand(Pause);
		}

		public async Task Play()
		{
			if (CurrentSong == null)
			{
				Song currSong = Playlist.CurrentSong;
				if (currSong == null)
				{
					return;
				}

				await SwitchToNewSong(currSong);
			}

			IsPlaying = true;
			audioPlayer.Play();
		}

		public void Pause()
		{
			IsPlaying = false;
			audioPlayer.Pause();
		}

		public void Stop()
		{
			if (IsPlaying)
			{
				IsPlaying = false;
				audioPlayer.Stop();
				CurrentSong = null;
			}
		}

		private async void AudioPlayer_SongFinished(object sender, SongMediaFinishedEventArgs eventArgs)
		{
			Song currSong = Playlist.CurrentSong;
			if (currSong != null)
			{
				await playbacksRegistrator.RegisterPlaybackFinish(currSong);
			}

			CurrentSong = null;

			Playlist.SwitchToNextSong();
			if (Playlist.CurrentSong == null)
			{
				//	We finished the end of playlist.
				IsPlaying = false;
				Messenger.Default.Send(new PlaylistFinishedEventArgs(Playlist));
				return;
			}

			//	Play next song from the playlist.
			await Play();
		}

		private async Task SwitchToNewSong(Song newSong)
		{
			CurrentSong = newSong;
			var songFileName = await musicLibrary.GetSongFile(newSong);
			audioPlayer.SetCurrentSongFile(songFileName);
			await playbacksRegistrator.RegisterPlaybackStart(newSong);
		}

		private void AudioPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IAudioPlayer.CurrSongLength))
			{
				RaisePropertyChanged(nameof(CurrSongLength));
				RaisePropertyChanged(nameof(CurrSongProgress));
				return;
			}

			if (e.PropertyName == nameof(IAudioPlayer.CurrSongPosition))
			{
				RaisePropertyChanged(nameof(CurrSongElapsed));
				RaisePropertyChanged(nameof(CurrSongProgress));
				return;
			}
		}
	}
}
