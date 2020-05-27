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
			set
			{
				title = value;
				OnPropertyChanged();
			}
		}

		public string TreeTitle { get; set; }

		public short? TrackNumber
		{
			get => trackNumber;
			set
			{
				trackNumber = value;
				OnPropertyChanged();
			}
		}

		public TimeSpan Duration { get; set; }

		public DiscModel Disc { get; set; }

		public ArtistModel Artist
		{
			get => artist;
			set
			{
				artist = value;
				OnPropertyChanged();
			}
		}

		public GenreModel Genre
		{
			get => genre;
			set
			{
				genre = value;
				OnPropertyChanged();
			}
		}

		public RatingModel? Rating
		{
			get => rating;
			set
			{
				rating = value;
				OnPropertyChanged();
			}
		}

		public int? BitRate { get; set; }

		public long? Size { get; set; }

		public uint? Checksum { get; set; }

		public DateTimeOffset? LastPlaybackTime
		{
			get => lastPlaybackTime;
			set
			{
				lastPlaybackTime = value;
				OnPropertyChanged();
			}
		}

		public int PlaybacksCount
		{
			get => playbacksCount;
			set
			{
				playbacksCount = value;
				OnPropertyChanged();
			}
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

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
