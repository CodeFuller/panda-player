using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicLibrary.Core.Models
{
	// TBD: Rename SongModel to Song, DiscModel to Disc, ...
	public class SongModel : INotifyPropertyChanged
	{
		private string title;
		private short? trackNumber;
		private ArtistModel artist;
		private GenreModel genre;
		private RatingModel? rating;
		private DateTimeOffset? lastPlaybackTime;
		private int playbacksCount;

		public ItemId Id { get; set; }

		public string Title
		{
			get => title;
			set => SetField(ref title, value);
		}

		public string TreeTitle { get; set; }

		public short? TrackNumber
		{
			get => trackNumber;
			set => SetField(ref trackNumber, value);
		}

		public TimeSpan Duration { get; set; }

		public DiscModel Disc { get; set; }

		public ArtistModel Artist
		{
			get => artist;
			set => SetField(ref artist, value);
		}

		public GenreModel Genre
		{
			get => genre;
			set => SetField(ref genre, value);
		}

		public RatingModel? Rating
		{
			get => rating;
			set => SetField(ref rating, value);
		}

		public int? BitRate { get; set; }

		public long? Size { get; set; }

		public uint? Checksum { get; set; }

		public DateTimeOffset? LastPlaybackTime
		{
			get => lastPlaybackTime;
			set => SetField(ref lastPlaybackTime, value);
		}

		public int PlaybacksCount
		{
			get => playbacksCount;
			set => SetField(ref playbacksCount, value);
		}

		public IReadOnlyCollection<PlaybackModel> Playbacks { get; set; }

		public Uri ContentUri { get; set; }

		public DateTimeOffset? DeleteDate { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public event PropertyChangedEventHandler PropertyChanged;

		public void AddPlayback(DateTimeOffset playbackTime)
		{
			++PlaybacksCount;
			LastPlaybackTime = playbackTime;
		}

		private void SetField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
			{
				return;
			}

			field = newValue;

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
