using System;
using System.Collections.Generic;
using System.ComponentModel;
using MusicLibrary.Core.Extensions;

namespace MusicLibrary.Core.Models
{
	// TODO: Rename SongModel to Song, DiscModel to Disc, ...
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

		public DiscModel Disc { get; set; }

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
			set => this.SetField(PropertyChanged, ref lastPlaybackTime, value);
		}

		public int PlaybacksCount
		{
			get => playbacksCount;
			set => this.SetField(PropertyChanged, ref playbacksCount, value);
		}

		public IReadOnlyCollection<PlaybackModel> Playbacks { get; set; }

		public Uri ContentUri
		{
			get => contentUri;
			set => this.SetField(PropertyChanged, ref contentUri, value);
		}

		public DateTimeOffset? DeleteDate { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public event PropertyChangedEventHandler PropertyChanged;

		public void AddPlayback(DateTimeOffset playbackTime)
		{
			++PlaybacksCount;
			LastPlaybackTime = playbackTime;
		}
	}
}
