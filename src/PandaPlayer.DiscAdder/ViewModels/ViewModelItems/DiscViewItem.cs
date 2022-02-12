using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal abstract class DiscViewItem : ViewModelBase
	{
		public string SourcePath => AddedDiscInfo.SourcePath;

		public IReadOnlyCollection<string> DestinationFolderPath => AddedDiscInfo.DestinationFolderPath;

		public string DestinationFolder => String.Join('/', DestinationFolderPath);

		public abstract DiscModel ExistingDisc { get; }

		public abstract string DiscTypeTitle { get; }

		public abstract bool WarnAboutDiscType { get; }

		public abstract bool WarnAboutFolder { get; }

		private BasicInputArtistItem artist;

		public BasicInputArtistItem Artist
		{
			get => artist;
			set
			{
				Set(ref artist, value);
				RaisePropertyChanged(nameof(WarnAboutArtist));
			}
		}

		public bool WarnAboutArtist => Artist is not SpecificInputArtistItem { IsNew: false };

		public IReadOnlyCollection<BasicInputArtistItem> AvailableArtists { get; }

		public string DiscTitle => AddedDiscInfo.DiscTitle;

		public string TreeTitle => AddedDiscInfo.TreeTitle;

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

		public IReadOnlyCollection<GenreModel> AvailableGenres { get; }

		public abstract bool RequiredDataIsFilled { get; }

		protected AddedDiscInfo AddedDiscInfo { get; }

		public IEnumerable<AddedSongInfo> AddedSongs => AddedDiscInfo.Songs;

		protected DiscViewItem(AddedDiscInfo addedDiscInfo, IEnumerable<BasicInputArtistItem> availableArtists, IEnumerable<GenreModel> availableGenres)
		{
			AddedDiscInfo = addedDiscInfo ?? throw new ArgumentNullException(nameof(addedDiscInfo));
			AvailableArtists = availableArtists.ToCollection();
			AvailableGenres = availableGenres.ToCollection();
		}

		protected BasicInputArtistItem LookupArtist(string artistName)
		{
			if (artistName == null)
			{
				return AvailableArtists.OfType<EmptyInputArtistItem>().Single();
			}

			var matchingArtist = AvailableArtists.OfType<SpecificInputArtistItem>().SingleOrDefault(a => a.Matches(artistName));
			if (matchingArtist == null)
			{
				throw new InvalidOperationException($"Artist {artistName} does not present in artists list");
			}

			return matchingArtist;
		}
	}
}
