using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.MusicStorage;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class NewDiscViewItem : DiscViewItem
	{
		public override string DiscTypeTitle => "New Disc";

		public override bool WarnAboutDiscType => false;

		public override bool WarnAboutFolder { get; }

		public override string AlbumTitle
		{
			get => Disc.AlbumTitle;
			set
			{
				Disc.AlbumTitle = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(WarnAboutAlbumTitle));
			}
		}

		public override bool AlbumTitleIsEditable => true;

		public override int? Year
		{
			get => Disc.Year;
			set
			{
				Disc.Year = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(WarnAboutYear));
			}
		}

		public override bool YearIsEditable => true;

		public override bool RequiredDataIsFilled => !GenreIsNotFilled;

		public NewDiscViewItem(AddedDiscInfo disc, bool folderExists, IEnumerable<ArtistViewItem> availableArtists, IEnumerable<GenreModel> availableGenres, GenreModel genre)
			: base(disc, availableArtists, availableGenres)
		{
			if (String.IsNullOrWhiteSpace(disc.DiscTitle))
			{
				throw new InvalidOperationException("Disc title could not be empty");
			}

			WarnAboutFolder = !folderExists;

			Artist = LookupNewDiscArtist(disc);
			Genre = LookupGenre(AvailableGenres, genre);

			Disc = new DiscModel
			{
				Year = disc.Year,
				Title = disc.DiscTitle,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};
		}

		private ArtistViewItem LookupNewDiscArtist(AddedDiscInfo disc)
		{
			return disc.HasVariousArtists ? AvailableArtists.OfType<VariousArtistViewItem>().Single() : LookupArtist(disc.Artist);
		}

		private static GenreModel LookupGenre(IEnumerable<GenreModel> availableGenres, GenreModel genreModel)
		{
			if (genreModel == null)
			{
				return null;
			}

			var foundGenre = availableGenres.SingleOrDefault(g => new GenreEqualityComparer().Equals(g, genreModel));
			if (foundGenre == null)
			{
				throw new InvalidOperationException($"Failed to find genre '{genreModel.Name}' in the list of available genres");
			}

			return foundGenre;
		}
	}
}
