using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public sealed class CompilationDiscWithoutArtistInfoViewItem : CompilationDiscViewItem
	{
		public override string DiscTypeTitle => "Compilation without Artists";

		private Artist artist;
		public override Artist Artist
		{
			get { return artist; }
			set
			{
				Set(ref artist, value);
				RaisePropertyChanged(nameof(ArtistIsNotFilled));
				RaisePropertyChanged(nameof(ArtistIsNew));
			}
		}
		public override bool ArtistIsEditable => true;
		public override bool ArtistIsNotFilled => Artist == null;

		public CompilationDiscWithoutArtistInfoViewItem(AddedDiscInfo discInfo, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(discInfo, availableArtists, availableGenres)
		{
		}

		protected override Artist GetSongArtist(AddedSongInfo song)
		{
			return Artist;
		}
	}
}
