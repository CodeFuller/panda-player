using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;

namespace PandaPlayer.ViewModels
{
	public class SongListItem : ViewModelBase
	{
		// We bind to properties of SongModel directly, without intermediate properties in current class.
		// Otherwise, we must handle Song.PropertyChanged and raise PropertyChanged for intermediate properties.
		public SongModel Song { get; }

		private bool isCurrentlyPlayed;

		public bool IsCurrentlyPlayed
		{
			get => isCurrentlyPlayed;
			set => Set(ref isCurrentlyPlayed, value);
		}

		public SongListItem(SongModel song)
		{
			Song = song ?? throw new ArgumentNullException(nameof(song));

			Song.PropertyChanged += (_, args) =>
			{
				Messenger.Default.Send(new SongChangedEventArgs(Song, args.PropertyName));
			};
		}
	}
}
