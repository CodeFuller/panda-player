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
		/// Disc source path.
		/// </summary>
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

		public bool RequiredDataIsFilled => !GenreIsNotFilled;

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

		protected DiscViewItem(string sourcePath, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
		{
			if (String.IsNullOrWhiteSpace(disc.Title))
			{
				throw new InvalidOperationException("Disc title could not be empty");
			}

			SourcePath = sourcePath;
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
	}
}
