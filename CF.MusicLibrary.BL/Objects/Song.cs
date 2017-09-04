using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace CF.MusicLibrary.BL.Objects
{
	public class Song : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		//	CF TEMP: Switch from annotations to fluent configuration

		[Key]
		public int Id { get; set; }

		[Required]
		public Disc Disc { get; set; }

		public Artist Artist { get; set; }

		public short? TrackNumber { get; set; }

		public short? Year { get; set; }

		[Required]
		public string Title { get; set; }

		public Genre Genre { get; set; }

		[NotMapped]
		public TimeSpan Duration { get; set; }

		[Required]
		[Column("Duration")]
		public double DurationInMilliseconds
		{
			get { return Duration.TotalMilliseconds; }
			set { Duration = TimeSpan.FromMilliseconds(value); }
		}

		public Rating? Rating { get; set; }

		public Rating SafeRating => Rating ?? Objects.Rating.R5;

		[NotMapped]
		public Uri Uri { get; set; }

		[Required]
		[Column("Uri")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Support property for Uri field. It's required because Uri type is not supported by used Data Provider.")]
		public string SongUri
		{
			get { return Uri.ToString(); }
			set { Uri = new Uri(value, UriKind.Relative);}
		}

		public int FileSize { get; set; }

		public int? Bitrate { get; set; }

		private DateTime? lastPlaybackTime;
		public DateTime? LastPlaybackTime
		{
			get { return lastPlaybackTime; }
			set
			{
				lastPlaybackTime = value;
				OnPropertyChanged();
			}
		}

		private int playbacksCount;
		public int PlaybacksCount
		{
			get { return playbacksCount; }
			set
			{
				playbacksCount = value;
				OnPropertyChanged();
			}
		}

		public Collection<Playback> Playbacks { get; } = new Collection<Playback>();

		public void AddPlayback(DateTime playbackTime)
		{
			++PlaybacksCount;
			LastPlaybackTime = playbackTime;
			Playbacks.Add(new Playback(this, playbackTime));
			OnPropertyChanged(nameof(Playbacks));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default value is required when parameter has attribute CallerMemberName.")]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
