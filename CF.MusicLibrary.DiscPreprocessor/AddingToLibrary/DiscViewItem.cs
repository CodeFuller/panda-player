using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using GalaSoft.MvvmLight;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class DiscViewItem : ViewModelBase
	{
		private readonly IDiscArtImageFile discArtImageFile;

		public string SourcePath { get; }

		public abstract string DiscTypeTitle { get; }

		public abstract Artist Artist { get; set; }
		public abstract bool ArtistIsEditable { get; }
		public abstract bool ArtistIsNotFilled { get; }
		public bool ArtistIsNew => Artist?.Id == 0;

		public Collection<Artist> AvailableArtists { get; }

		public string DiscTitle { get; }

		private string albumTitle;
		public string AlbumTitle
		{
			get { return albumTitle; }
			set
			{
				Set(ref albumTitle, value);
				RaisePropertyChanged(nameof(AlbumTitleMatchesDiscTitle));
			}
		}

		public bool AlbumTitleMatchesDiscTitle => String.Equals(AlbumTitle, DiscTitle, StringComparison.OrdinalIgnoreCase);

		private short? year;
		public virtual short? Year
		{
			get { return year; }
			set
			{
				Set(ref year, value);
				RaisePropertyChanged(nameof(YearIsNotFilled));
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

		public Uri DestinationUri { get; }

		public bool DiscArtIsValid => discArtImageFile.ImageIsValid;

		public string DiscArtInfo => DiscArtIsValid ? discArtImageFile.ImageProperties : discArtImageFile.ImageStatus;

		public AddedDiscCoverImage AddedDiscCoverImage
		{
			get
			{
				if (!DiscArtIsValid)
				{
					throw new InvalidOperationException("Disc Art is not valid");
				}

				return new AddedDiscCoverImage(Disc, discArtImageFile.ImageFileName);
			}
		}

		public bool RequiredDataIsFilled => !GenreIsNotFilled && DiscArtIsValid;

		protected IReadOnlyCollection<AddedSongInfo> SourceSongs { get; }

		public Disc Disc => new Disc
		{
			Title = DiscTitle,
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
					Year = Year,
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

		protected DiscViewItem(IDiscArtImageFile discArtImageFile, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
		{
			if (discArtImageFile == null)
			{
				throw new ArgumentNullException(nameof(discArtImageFile));
			}

			if (String.IsNullOrWhiteSpace(disc.Title))
			{
				throw new InvalidOperationException("Disc title could not be empty");
			}

			this.discArtImageFile = discArtImageFile;
			this.discArtImageFile.PropertyChanged += DiscArtImageFileOnPropertyChanged;

			SourcePath = disc.SourcePath;
			DiscTitle = disc.Title;
			albumTitle = DiscTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(disc.Title);
			AvailableArtists = availableArtists.OrderBy(a => a.Name).ToCollection();
			AvailableGenres = availableGenres.OrderBy(a => a.Name).ToCollection();
			DestinationUri = disc.UriWithinStorage;
			SourceSongs = disc.Songs.ToList();
		}

		protected abstract Artist GetSongArtist(AddedSongInfo song);

		protected Artist LookupArtist(string artistName)
		{
			var artist = AvailableArtists.SingleOrDefault(a => a.Name == artistName);
			if (artist == null)
			{
				throw new InvalidOperationException(Current($"Artist {artistName} does not present in artists list"));
			}

			return artist;
		}

		public void SetDiscCoverImage(string imageFileName)
		{
			discArtImageFile.Load(imageFileName, false);
		}

		public void UnsetDiscCoverImage()
		{
			discArtImageFile.Unload();
		}

		private void DiscArtImageFileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IDiscArtImageFile.ImageIsValid))
			{
				RaisePropertyChanged(nameof(DiscArtIsValid));
				RaisePropertyChanged(nameof(DiscArtInfo));
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}

			if (e.PropertyName == nameof(IDiscArtImageFile.ImageStatus) || e.PropertyName == nameof(IDiscArtImageFile.ImageProperties))
			{
				RaisePropertyChanged(nameof(DiscArtInfo));
			}
		}
	}
}
