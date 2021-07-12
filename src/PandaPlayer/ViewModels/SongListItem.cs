using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;

namespace PandaPlayer.ViewModels
{
	public class SongListItem : ViewModelBase
	{
		public SongModel Song { get; }

		private bool isCurrentlyPlayed;

		public bool IsCurrentlyPlayed
		{
			get => isCurrentlyPlayed;
			set => Set(ref isCurrentlyPlayed, value);
		}

		public DateTimeOffset? LastPlaybackTime => Song.LastPlaybackTime;

		public int PlaybacksCount => Song.PlaybacksCount;

		public SongListItem(SongModel song)
		{
			Song = song ?? throw new ArgumentNullException(nameof(song));

			Song.PropertyChanged += (sender, args) =>
			{
				Messenger.Default.Send(new SongChangedEventArgs(Song, args.PropertyName));
			};
		}
	}
}
