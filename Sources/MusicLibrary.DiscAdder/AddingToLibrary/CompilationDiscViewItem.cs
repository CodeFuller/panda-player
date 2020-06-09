using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.MusicStorage;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	public abstract class CompilationDiscViewItem : NewDiscViewItem
	{
		// Seal the method for calling it in constructor.
		public sealed override int? Year
		{
			get => base.Year;
			set => base.Year = value;
		}

		protected CompilationDiscViewItem(AddedDiscInfo disc, bool folderExists, IEnumerable<ArtistModel> availableArtists, IEnumerable<GenreModel> availableGenres)
			: base(disc, folderExists, availableArtists, availableGenres)
		{
			Year = disc.Year;
		}
	}
}
