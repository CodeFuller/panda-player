using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public abstract class AddedAlbum : ViewModelBase
	{
		/// <summary>
		/// Album source path within workshop directory.
		/// </summary>
		public string PathWithinWorkshopStorage { get; }

		/// <summary>
		/// Album source path.
		/// </summary>
		public string SourcePath { get; }

		public abstract AlbumType AlbumsType { get; }

		public abstract string AlbumTypeTitle { get; }

		public abstract string Artist { get; set; }
		public abstract bool ArtistIsEditable { get; }
		public abstract bool ArtistIsNotFilled { get; }

		private string title;
		/// <summary>
		/// Album title.
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
		/// Album destination uri.
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

		private readonly List<SongInfo> songs;
		public IReadOnlyCollection<SongInfo> Songs => songs;

		protected AddedAlbum(string sourcePath, AlbumInfo album, IEnumerable<Genre> availableGenres)
		{
			if (album == null)
			{
				throw new ArgumentNullException(nameof(album));
			}

			PathWithinWorkshopStorage = album.PathWithinStorage;
			SourcePath = sourcePath;
			title = album.Title;
			AvailableGenres = availableGenres.ToCollection();
			songs = album.Songs.ToList();
		}

		public abstract string GetSongArtist(SongInfo song);
	}
}
