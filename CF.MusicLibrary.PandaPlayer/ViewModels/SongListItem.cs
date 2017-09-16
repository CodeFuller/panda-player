using System;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SongListItem : ViewModelBase
	{
		public Song Song { get; }

		private bool isCurrentlyPlayed;
		public bool IsCurrentlyPlayed
		{
			get { return isCurrentlyPlayed; }
			set { Set(ref isCurrentlyPlayed, value); }
		}

		public DateTime? LastPlaybackTime => Song.LastPlaybackTime;

		public int PlaybacksCount => Song.PlaybacksCount;

		public SongListItem(Song song)
		{
			Song = song;
		}

		public void AddPlaybackForCurrentSong(DateTime playbackDateTime)
		{
			Song.AddPlayback(playbackDateTime);
			RaisePropertyChanged(nameof(LastPlaybackTime));
			RaisePropertyChanged(nameof(PlaybacksCount));
		}
	}
}
