using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using GalaSoft.MvvmLight;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class DiscViewItem : ViewModelBase
	{
		/// <summary>
		/// Disc source path within workshop directory.
		/// </summary>
		public string PathWithinWorkshopStorage { get; }

		/// <summary>
		/// Disc source path.
		/// </summary>
		public string SourcePath { get; }

		public abstract string DiscTypeTitle { get; }

		public abstract Artist Artist { get; set; }
		public abstract bool ArtistIsEditable { get; }
		public abstract bool ArtistIsNotFilled { get; }

		protected IReadOnlyCollection<Artist> AvailableArtists { get; }

		private string title;
		/// <summary>
		/// Disc title.
		/// </summary>
		/// <remarks>
		/// The only case when this title is edited currently, it's setting empty title for 'Best\Foreign' and 'Best\Russian' directories.
		/// </remarks>
		public string Title
		{
			get { return title; }
			set
			{
				Set(ref title, value);
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}

		private string albumTitle;
		public string AlbumTitle
		{
			get { return albumTitle; }
			set
			{
				Set(ref albumTitle, value);
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}

		private int? year;
		public virtual int? Year
		{
			get { return year; }
			set
			{
				Set(ref year, value);
				RaisePropertyChanged(nameof(YearIsNotFilled));
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}
		public bool YearIsNotFilled => !Year.HasValue;

		private Genre genre;
		public Genre Genre
		{
			get { return genre; }
			set
			{
				Set(ref genre, value);
				RaisePropertyChanged(nameof(GenreIsNotFilled));
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}
		public bool GenreIsNotFilled => genre == null;

		public Collection<Genre> AvailableGenres { get; }

		private Uri destinationUri;
		/// <summary>
		/// Disc destination uri.
		/// </summary>
		/// <example>
		/// "/Foreign/Guano Apes" for "Guano Apes".
		/// "/Soundtracks/Pulp Fiction" for "Pulp Fiction"
		/// </example>
		public Uri DestinationUri
		{
			get { return destinationUri; }
			set
			{
				Set(ref destinationUri, value);
				RaisePropertyChanged(nameof(DestinationUriIsNotFilled));
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}
		public abstract bool DestinationUriIsEditable { get; }
		public bool DestinationUriIsNotFilled => DestinationUri == null;

		public abstract IEnumerable<Uri> AvailableDestinationUris { get; }

		public abstract bool RequiredDataIsFilled { get; }

		protected IReadOnlyCollection<AddedSongInfo> SourceSongs { get; }

		public Disc Disc => new Disc
		{
			Year = Year,
			Title = Title,
			AlbumTitle = AlbumTitle,
			Uri = DestinationUri,
		};

		public IEnumerable<AddedSong> Songs
		{
			get
			{
				var disc = Disc;
				var songs = SourceSongs.Select(s => new AddedSong(new Song
				{
					Disc = disc,
					Artist = GetSongArtist(s),
					TrackNumber = s.Track,
					Year = (short?)disc.Year,
					Title = s.Title,
					Genre = Genre,
					Rating = null,
					LastPlaybackTime = null,
					PlaybacksCount = 0,
				}, s.SourcePath)).ToCollection();

				//	We do not fill Disc.Songs collection as MusicLibraryRepository.AddSong() requries

				return songs;
			}
		}

		protected DiscViewItem(string sourcePath, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
		{
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			PathWithinWorkshopStorage = disc.PathWithinStorage;
			SourcePath = sourcePath;
			title = disc.Title;
			albumTitle = DiscTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(disc.Title);
			AvailableArtists = availableArtists.ToList();
			AvailableGenres = availableGenres.ToCollection();
			SourceSongs = disc.Songs.ToList();
		}

		public abstract Artist GetSongArtist(AddedSongInfo song);

		protected Artist LookupArtist(string artistName)
		{
			var artist = AvailableArtists.SingleOrDefault(a => a.Name == artistName);
			if (artist == null)
			{
				throw new InvalidOperationException(Current($"Artist {artistName} does not present in artists list"));
			}

			return artist;
		}
	}
}
