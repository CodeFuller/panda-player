using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.LastFM.Objects;
using CF.MusicLibrary.PandaPlayer.Player;
using CF.MusicLibrary.PandaPlayer.Scrobbler;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class MusicPlayerViewModel : ViewModelBase, IMusicPlayerViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly ITimerFacade timer;
		private readonly IScrobbler scrobbler;

		private readonly MediaPlayer mediaPlayer = new MediaPlayer();

		private bool isPlaying;
		public bool IsPlaying
		{
			get { return isPlaying; }
			set { Set(ref isPlaying, value); }
		}

		private TimeSpan currSongElapsed;
		public TimeSpan CurrSongElapsed
		{
			get { return currSongElapsed; }
			set
			{
				Set(ref currSongElapsed, value);
				RaisePropertyChanged(nameof(CurrSongProgress));
			}
		}

		private TimeSpan currSongLength;
		public TimeSpan CurrSongLength
		{
			get { return currSongLength; }
			set
			{
				Set(ref currSongLength, value);
				RaisePropertyChanged(nameof(CurrSongProgress));
			}
		}

		public int CurrSongProgress => (int)Math.Round(100 * CurrSongElapsed.TotalMilliseconds / CurrSongLength.TotalMilliseconds);

		public ISongPlaylist Playlist { get; }

		public double Volume
		{
			get { return mediaPlayer.Volume; }
			set
			{
				mediaPlayer.Volume = value;
				RaisePropertyChanged();
			}
		}

		public ICommand PlayCommand { get; }
		public ICommand PauseCommand { get; }

		public MusicPlayerViewModel(ISongPlaylist playlist, IMusicLibrary musicLibrary, ITimerFacade timer, IScrobbler scrobbler)
		{
			if (playlist == null)
			{
				throw new ArgumentNullException(nameof(playlist));
			}
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (timer == null)
			{
				throw new ArgumentNullException(nameof(timer));
			}
			if (scrobbler == null)
			{
				throw new ArgumentNullException(nameof(scrobbler));
			}

			Playlist = playlist;
			this.musicLibrary = musicLibrary;
			this.timer = timer;
			this.timer.Elapsed += Timer_Elapsed;
			this.timer.Interval = 200;
			this.scrobbler = scrobbler;

			PlayCommand = new RelayCommand(Resume);
			PauseCommand = new RelayCommand(Pause);

			mediaPlayer.MediaOpened += MediaPlayerOnMediaOpened;
			mediaPlayer.MediaEnded += MediaPlayerOnMediaEnded;
		}

		public async Task Play()
		{
			Song currSong = Playlist.CurrentSong;
			if (currSong == null)
			{
				StopPlayback();
				return;
			}

			await SwitchToNewSong(currSong);
			Resume();
		}

		public void Pause()
		{
			mediaPlayer.Pause();
			StopPlayback();
		}

		public void Resume()
		{
			if (Playlist.CurrentSong != null)
			{
				mediaPlayer.Play();
				StartPlayback();
			}
		}

		public void Stop()
		{
			throw new System.NotImplementedException();
		}

		public void SetCurrentSongProgress(double progress)
		{
			mediaPlayer.Position = TimeSpan.FromMilliseconds(CurrSongLength.TotalMilliseconds * progress);
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				CurrSongElapsed = mediaPlayer.Position;
			});
		}

		private void MediaPlayerOnMediaOpened(object sender, EventArgs eventArgs)
		{
			CurrSongLength = mediaPlayer.NaturalDuration.TimeSpan;
		}

		private async void MediaPlayerOnMediaEnded(object sender, EventArgs eventArgs)
		{
			Song currSong = Playlist.CurrentSong;
			if (currSong != null)
			{
				var playbackDateTime = DateTime.Now;
				currSong.AddPlayback(playbackDateTime);
				await musicLibrary.AddSongPlayback(currSong, playbackDateTime);
				await scrobbler.Scrobble(new TrackScrobble(GetTrackFromSong(currSong), DateTime.Now - currSong.Duration));
			}

			Playlist.SwitchToNextSong();
			await Play();
		}

		private async Task SwitchToNewSong(Song newSong)
		{
			var songFile = await musicLibrary.GetSongFile(newSong);
			mediaPlayer.Open(new Uri(songFile.FullName));
			await scrobbler.UpdateNowPlaying(GetTrackFromSong(newSong));
		}

		private static Track GetTrackFromSong(Song song)
		{
			return new Track
			{
				Number = song.TrackNumber,
				Title = song.Title,
				Artist = song.Artist.Name,
				Album = new Album(song.Artist.Name, song.Disc.AlbumTitle),
				Duration = song.Duration,
			};
		}

		private void StartPlayback()
		{
			if (!timer.Enabled)
			{
				timer.Start();
			}
			IsPlaying = true;
		}

		private void StopPlayback()
		{
			timer.Stop();
			IsPlaying = false;
		}
	}
}
