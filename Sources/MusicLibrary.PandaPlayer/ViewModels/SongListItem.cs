﻿using System;
using GalaSoft.MvvmLight;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class SongListItem : ViewModelBase
	{
		public Song Song { get; }

		private bool isCurrentlyPlayed;

		public bool IsCurrentlyPlayed
		{
			get => isCurrentlyPlayed;
			set => Set(ref isCurrentlyPlayed, value);
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