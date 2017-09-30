using System.Collections.Generic;
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

		protected CompilationDiscViewItem(string sourcePath, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(sourcePath, disc, availableArtists, availableGenres)
		{
			Year = disc.Year;
		}
	}
}
