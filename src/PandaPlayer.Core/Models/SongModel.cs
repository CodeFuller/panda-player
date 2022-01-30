using System;
using System.Collections.Generic;
using System.ComponentModel;
using PandaPlayer.Core.Extensions;

namespace PandaPlayer.Core.Models
{
	public class SongModel : INotifyPropertyChanged
	{
		private string title;
		private string treeTitle;
		private short? trackNumber;
		private ArtistModel artist;
		private GenreModel genre;
		private RatingModel? rating;
		private DateTimeOffset? lastPlaybackTime;
		private int playbacksCount;
		private Uri contentUri;
		private DateTimeOffset? deleteDate;

		public ItemId Id { get; set; }

		public string Title
		{
			get => title;
			set => this.SetField(PropertyChanged, ref title, value);
		}

		public string TreeTitle
		{
			get => treeTitle;
			set => this.SetField(PropertyChanged, ref treeTitle, value);
		}

		public short? TrackNumber
		{
			get => trackNumber;
			set => this.SetField(PropertyChanged, ref trackNumber, value);
		}

		public TimeSpan Duration { get; set; }

		public DiscModel Disc { get; internal set; }

		public ArtistModel Artist
		{
			get => artist;
			set => this.SetField(PropertyChanged, ref artist, value);
		}

		public GenreModel Genre
		{
			get => genre;
			set => this.SetField(PropertyChanged, ref genre, value);
		}

		public RatingModel? Rating
		{
			get => rating;
			set => this.SetField(PropertyChanged, ref rating, value);
		}

		public int? BitRate { get; set; }

		public long? Size { get; set; }

		public uint? Checksum { get; set; }

		public DateTimeOffset? LastPlaybackTime
		{
			get => lastPlaybackTime;
			init => lastPlaybackTime = value;
		}

		public int PlaybacksCount
		{
			get => playbacksCount;
			init => playbacksCount = value;
		}

		public IReadOnlyCollection<PlaybackModel> Playbacks { get; init; }

		public Uri ContentUri
		{
			get => contentUri;
			set => this.SetField(PropertyChanged, ref contentUri, value);
		}

		public DateTimeOffset? DeleteDate
		{
			get => deleteDate;
			set => this.SetField(PropertyChanged, ref deleteDate, value);
		}

		public string DeleteComment { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public event PropertyChangedEventHandler PropertyChanged;

		public void AddPlayback(DateTimeOffset playbackTime)
		{
			++playbacksCount;
			lastPlaybackTime = playbackTime;

			this.SetField(PropertyChanged, ref playbacksCount, playbacksCount + 1, propertyName: nameof(PlaybacksCount));
			this.SetField(PropertyChanged, ref lastPlaybackTime, playbackTime, propertyName: nameof(LastPlaybackTime));
		}

		public SongModel CloneShallow()
		{
			return new()
			{
				Id = Id,
				Title = Title,
				TreeTitle = TreeTitle,
				TrackNumber = TrackNumber,
				Duration = Duration,
				Disc = Disc,
				Artist = Artist,
				Genre = Genre,
				Rating = Rating,
				BitRate = BitRate,
				Size = Size,
				Checksum = Checksum,
				LastPlaybackTime = LastPlaybackTime,
				PlaybacksCount = PlaybacksCount,
				Playbacks = Playbacks,
				ContentUri = ContentUri,
				DeleteDate = DeleteDate,
				DeleteComment = DeleteComment,
			};
		}
	}
}
