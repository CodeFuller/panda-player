using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using GalaSoft.MvvmLight;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	public abstract class DiscViewItem : ViewModelBase
	{
		public string SourcePath { get; }

		public string DestinationFolder => String.Join('/', DestinationFolderPath);

		public IReadOnlyCollection<string> DestinationFolderPath { get; }

		public bool FolderIsNew { get; }

		public abstract string DiscTypeTitle { get; }

		public virtual bool WarnAboutDiscType => false;

		public abstract ArtistModel Artist { get; set; }

		public abstract bool ArtistIsEditable { get; }

		public abstract bool ArtistIsNotFilled { get; }

		public bool ArtistIsNew => Artist?.Id == null;

		public Collection<ArtistModel> AvailableArtists { get; }

		public string DiscTitle => Disc.Title;

		public string TreeTitle => Disc.TreeTitle;

		public abstract string AlbumTitle { get; set; }

		public abstract bool AlbumTitleIsEditable { get; }

		public bool WarnAboutUnequalAlbumTitle => AlbumTitleIsEditable && !String.Equals(AlbumTitle, DiscTitle, StringComparison.Ordinal);

		public virtual int? Year { get; set; }

		public abstract bool YearIsEditable { get; }

		public bool WarnAboutNotFilledYear => YearIsEditable && !Year.HasValue;

		private GenreModel genre;

		public GenreModel Genre
		{
			get => genre;
			set
			{
				Set(ref genre, value);
				RaisePropertyChanged(nameof(GenreIsNotFilled));
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}

		public bool GenreIsNotFilled => Genre == null;

		public Collection<GenreModel> AvailableGenres { get; }

		public abstract bool RequiredDataIsFilled { get; }

		protected IReadOnlyCollection<AddedSongInfo> SourceSongs { get; }

		public DiscModel Disc { get; protected set; }

		public IEnumerable<AddedSong> Songs => SourceSongs.Select(s => new AddedSong(CreateSong(s), s.SourcePath, DestinationFolderPath));

		protected DiscViewItem(AddedDiscInfo disc, bool folderExists, IEnumerable<ArtistModel> availableArtists, IEnumerable<GenreModel> availableGenres)
		{
			SourcePath = disc.SourcePath;
			DestinationFolderPath = disc.DestinationFolderPath;
			FolderIsNew = !folderExists;
			AvailableArtists = availableArtists.ToCollection();
			SourceSongs = disc.Songs.ToList();
			AvailableGenres = availableGenres.ToCollection();
		}

		protected abstract ArtistModel GetSongArtist(AddedSongInfo song);

		protected ArtistModel LookupArtist(string artistName)
		{
			var artist = AvailableArtists.SingleOrDefault(a => a.Name == artistName);
			if (artist == null)
			{
				throw new InvalidOperationException(Current($"Artist {artistName} does not present in artists list"));
			}

			return artist;
		}

		private SongModel CreateSong(AddedSongInfo song)
		{
			return new SongModel
			{
				Title = song.Title,
				TreeTitle = song.TreeTitle,
				TrackNumber = song.Track,
				Disc = Disc,
				Artist = GetSongArtist(song),
				Genre = Genre,
				Rating = null,
				LastPlaybackTime = null,
				PlaybacksCount = 0,
			};
		}
	}
}
