﻿using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Shared;

namespace PandaPlayer.ViewModels
{
	public class SongListItem : ViewModelBase
	{
		// We bind to properties of SongModel directly, without intermediate properties in current class.
		// Otherwise, we must handle Song.PropertyChanged and raise PropertyChanged for intermediate properties.
		public SongModel Song { get; }

		public string BitRate => Song.IsDeleted || Song.BitRate == null ? "N/A" : $"{Song.BitRate.Value / 1000:n0}";

		public string FileSize => Song.IsDeleted || Song.Size == null ? "N/A" : FileSizeFormatter.GetFormattedFileSize(Song.Size.Value);

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
