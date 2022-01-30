using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal abstract class DiscViewItem : ViewModelBase
	{
		public string SourcePath { get; }

		public IReadOnlyCollection<string> DestinationFolderPath { get; }

		public string DestinationFolder => String.Join('/', DestinationFolderPath);

		public abstract string DiscTypeTitle { get; }

		public abstract bool WarnAboutDiscType { get; }

		public abstract bool WarnAboutFolder { get; }

		private ArtistViewItem artist;

		public ArtistViewItem Artist
		{
			get => artist;
			set
			{
				Set(ref artist, value);
				RaisePropertyChanged(nameof(WarnAboutArtist));
			}
		}

		public bool WarnAboutArtist => !(Artist is SpecificArtistViewItem specificArtist && !specificArtist.IsNew);

		public Collection<ArtistViewItem> AvailableArtists { get; }

		public string DiscTitle => Disc.Title;

		public string TreeTitle => Disc.TreeTitle;

		public abstract string AlbumTitle { get; set; }

		public abstract bool AlbumTitleIsEditable { get; }

		public bool WarnAboutAlbumTitle => AlbumTitleIsEditable && !String.Equals(AlbumTitle, DiscTitle, StringComparison.Ordinal);

		public virtual int? Year { get; set; }

		public abstract bool YearIsEditable { get; }

		public bool WarnAboutYear => YearIsEditable && !Year.HasValue;

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

		private IReadOnlyCollection<AddedSongInfo> SourceSongs { get; }

		public IEnumerable<(SongModel Song, string SourcePath)> Songs => SourceSongs.Select(s => (CreateSong(s), s.SourcePath));

		public DiscModel Disc { get; protected set; }

		protected DiscViewItem(AddedDiscInfo disc, IEnumerable<ArtistViewItem> availableArtists, IEnumerable<GenreModel> availableGenres)
		{
			SourcePath = disc.SourcePath;
			DestinationFolderPath = disc.DestinationFolderPath;
			AvailableArtists = availableArtists.ToCollection();
			SourceSongs = disc.Songs.ToList();
			AvailableGenres = availableGenres.ToCollection();
		}

		protected ArtistViewItem LookupArtist(string artistName)
		{
			if (artistName == null)
			{
				return AvailableArtists.OfType<EmptyArtistViewItem>().Single();
			}

			var matchingArtist = AvailableArtists.OfType<SpecificArtistViewItem>().SingleOrDefault(a => a.Matches(artistName));
			if (matchingArtist == null)
			{
				throw new InvalidOperationException($"Artist {artistName} does not present in artists list");
			}

			return matchingArtist;
		}

		private SongModel CreateSong(AddedSongInfo song)
		{
			var songModel = new SongModel
			{
				Title = song.Title,
				TreeTitle = song.TreeTitle,
				TrackNumber = song.Track,
				Artist = GetSongArtist(song),
				Genre = Genre,
				Rating = null,
				LastPlaybackTime = null,
				PlaybacksCount = 0,
			};

			// TODO: If Songs property is enumerated multiple times, then same song is added multiple times to Disc. Postpone creation of models till AddToLibraryViewModel.
			Disc.AddSong(songModel);

			return songModel;
		}

		private ArtistModel GetSongArtist(AddedSongInfo song)
		{
			if (Artist is EmptyArtistViewItem)
			{
				return null;
			}

			if (Artist is VariousArtistViewItem)
			{
				return LookupArtist(song.Artist)?.ArtistModel;
			}

			if (Artist is SpecificArtistViewItem specificArtist)
			{
				return specificArtist.ArtistModel;
			}

			throw new InvalidOperationException("The artist type is unknown");
		}
	}
}
