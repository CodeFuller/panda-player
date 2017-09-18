using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class CompilationDiscViewItem : DiscViewItem
	{
		// Seal the method for calling it in constructor.
		public sealed override short? Year
		{
			get { return base.Year; }
			set { base.Year = value; }
		}

		public override bool DestinationUriIsEditable => false;

		public override IEnumerable<Uri> AvailableDestinationUris => Enumerable.Repeat(DestinationUri, 1);

		public override bool RequiredDataIsFilled => Genre != null;

		protected CompilationDiscViewItem(string sourcePath, AddedDiscInfo disc, IEnumerable<Artist> availableArtists,
			Uri destinationUri, IEnumerable<Genre> availableGenres) : base(sourcePath, disc, availableArtists, availableGenres)
		{
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			DestinationUri = destinationUri;
			Year = disc.Year;
		}
	}
}
