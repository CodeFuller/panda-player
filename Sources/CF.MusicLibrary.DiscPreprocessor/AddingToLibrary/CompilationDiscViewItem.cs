using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class CompilationDiscViewItem : NewDiscViewItem
	{
		// Seal the method for calling it in constructor.
		public sealed override short? Year
		{
			get => base.Year;
			set => base.Year = value;
		}

		protected CompilationDiscViewItem(AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(disc, availableArtists, availableGenres)
		{
			Year = disc.Year;
		}
	}
}
