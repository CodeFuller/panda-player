using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.Extensions;
using MusicLibrary.DiscAdder.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.ViewModels.ViewModelItems
{
	internal class ExistingDiscViewItem : DiscViewItem
	{
		public override string DiscTypeTitle => "Existing Disc";

		public override bool WarnAboutDiscType => true;

		public override bool WarnAboutFolder => false;

		public override string AlbumTitle
		{
			get => Disc.AlbumTitle;
			set => throw new InvalidOperationException(Current($"Album title could not be changed for '{DiscTitle}' disc"));
		}

		public override bool AlbumTitleIsEditable => false;

		public override bool YearIsEditable => false;

		public override bool RequiredDataIsFilled => true;

		public ExistingDiscViewItem(DiscModel existingDisc, AddedDiscInfo disc, IEnumerable<ArtistViewItem> availableArtists, IEnumerable<GenreModel> availableGenres)
			: base(disc, availableArtists, availableGenres)
		{
			Disc = existingDisc;
			Artist = LookupArtist(existingDisc.SoloArtist?.Name);
			Genre = existingDisc.GetGenre();
		}
	}
}
