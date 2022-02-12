using System;
using System.Collections.Generic;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.Extensions;
using PandaPlayer.DiscAdder.MusicStorage;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal class ExistingDiscViewItem : DiscViewItem
	{
		public override DiscModel ExistingDisc { get; }

		public override string DiscTypeTitle => "Existing Disc";

		public override bool WarnAboutDiscType => true;

		public override bool WarnAboutFolder => false;

		public override string AlbumTitle
		{
			get => AddedDiscInfo.AlbumTitle;
			set => throw new InvalidOperationException($"Album title could not be changed for disc '{DiscTitle}'");
		}

		public override bool AlbumTitleIsEditable => false;

		public override bool YearIsEditable => false;

		public override bool RequiredDataIsFilled => true;

		public ExistingDiscViewItem(DiscModel existingDisc, AddedDiscInfo disc, IEnumerable<BasicInputArtistItem> availableArtists, IEnumerable<GenreModel> availableGenres)
			: base(disc, availableArtists, availableGenres)
		{
			ExistingDisc = existingDisc ?? throw new ArgumentNullException(nameof(existingDisc));
			Artist = LookupArtist(existingDisc.SoloArtist?.Name);
			Genre = existingDisc.GetGenre();
		}
	}
}
