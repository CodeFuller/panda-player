using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CF.MusicLibrary.Core.Objects
{
	public class Song : INotifyPropertyChanged
	{
		public static Rating DefaultRating => Objects.Rating.R5;

		public event PropertyChangedEventHandler PropertyChanged;

		public int Id { get; set; }

		public Disc Disc { get; set; }

		public int? ArtistId { get; set; }

		private Artist artist;

		public Artist Artist
		{
			get => artist;
			set
			{
				artist = value;
				ArtistId = artist?.Id;
				OnPropertyChanged();
			}
		}

		private short? trackNumber;

		public short? TrackNumber
		{
			get => trackNumber;
			set
			{
				trackNumber = value;
				OnPropertyChanged();
			}
		}

		private short? year;

		public short? Year
		{
			get => year;
			set
			{
				year = value;
				OnPropertyChanged();
			}
		}

		public string Title { get; set; }

		public int? GenreId { get; set; }

		private Genre genre;

		public Genre Genre
		{
			get => genre;
			set
			{
				genre = value;
				GenreId = genre?.Id;
				OnPropertyChanged();
			}
		}

		public TimeSpan Duration { get; set; }

		public double DurationInMilliseconds
		{
			get => Duration.TotalMilliseconds;
			set => Duration = TimeSpan.FromMilliseconds(value);
		}

		private Rating? rating;

		public Rating? Rating
		{
			get => rating;
			set
			{
				rating = value;
				OnPropertyChanged();
			}
		}

		public Rating SafeRating => Rating ?? DefaultRating;

		public Uri Uri { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Support property for Uri field. It's required because Uri type is not supported by used Data Provider.")]
		public string SongUri
		{
			get => Uri.ToString();
			set => Uri = new Uri(value, UriKind.Relative);
		}

		public int FileSize { get; set; }

		public int? Checksum { get; set; }

		public int? Bitrate { get; set; }

		private DateTime? lastPlaybackTime;

		public DateTime? LastPlaybackTime
		{
			get => lastPlaybackTime;
			set
			{
				lastPlaybackTime = value;
				OnPropertyChanged();
			}
		}

		private int playbacksCount;

		public int PlaybacksCount
		{
			get => playbacksCount;
			set
			{
				playbacksCount = value;
				OnPropertyChanged();
			}
		}

		public Collection<Playback> Playbacks { get; } = new Collection<Playback>();

		public DateTime? DeleteDate { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public void AddPlayback(DateTime playbackTime)
		{
			++PlaybacksCount;
			LastPlaybackTime = playbackTime;
			Playbacks.Add(new Playback(this, playbackTime));
			OnPropertyChanged(nameof(Playbacks));
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
