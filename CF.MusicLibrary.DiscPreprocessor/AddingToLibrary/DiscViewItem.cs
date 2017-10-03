using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using GalaSoft.MvvmLight;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class DiscViewItem : ViewModelBase
	{
		public string SourcePath { get; }

		public abstract string DiscTypeTitle { get; }
		public virtual bool WarnAboutDiscType => false;

		public abstract Artist Artist { get; set; }
		public abstract bool ArtistIsEditable { get; }
		public abstract bool ArtistIsNotFilled { get; }
		public bool ArtistIsNew => Artist?.Id == 0;

		public Collection<Artist> AvailableArtists { get; }

		public abstract string DiscTitle { get; }

		public abstract string AlbumTitle { get; set; }
		public abstract bool AlbumTitleIsEditable { get; }
		public bool WarnAboutUnequalAlbumTitle => AlbumTitleIsEditable && !String.Equals(AlbumTitle, DiscTitle, StringComparison.OrdinalIgnoreCase);

		public virtual short? Year { get; set; }
		public abstract bool YearIsEditable { get; }
		public bool WarnAboutNotFilledYear => YearIsEditable && !Year.HasValue;

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

		public bool GenreIsNotFilled => Genre == null;

		public Collection<Genre> AvailableGenres { get; }

		public Uri DestinationUri { get; }

		public abstract bool DiscArtIsValid { get; }

		public abstract string DiscArtInfo { get; }

		public abstract bool RequiredDataIsFilled { get; }

		protected IReadOnlyCollection<AddedSongInfo> SourceSongs { get; }

		protected abstract  Disc Disc { get; }

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

		public abstract AddedDiscCoverImage AddedDiscCoverImage { get; }

		protected DiscViewItem(AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
		{
			SourcePath = disc.SourcePath;
			AvailableArtists = availableArtists.OrderBy(a => a.Name).ToCollection();
			DestinationUri = disc.UriWithinStorage;
			SourceSongs = disc.Songs.ToList();
			AvailableGenres = availableGenres.OrderBy(a => a.Name).ToCollection();
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

		public abstract void SetDiscCoverImage(string imageFileName);

		public abstract void UnsetDiscCoverImage();
	}
}
