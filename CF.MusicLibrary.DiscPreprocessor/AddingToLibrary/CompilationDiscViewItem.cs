﻿using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Common.DiscArt;
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

		protected CompilationDiscViewItem(IDiscArtImageFile discArtImageFile, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(discArtImageFile, disc, availableArtists, availableGenres)
		{
			Year = disc.Year;
		}
	}
}
