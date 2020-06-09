using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.MusicStorage;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	public sealed class CompilationDiscWithoutArtistInfoViewItem : CompilationDiscViewItem
	{
		public override string DiscTypeTitle => "Compilation without Artists";

		private ArtistModel artist;

		public override ArtistModel Artist
		{
			get => artist;
			set
			{
				Set(ref artist, value);
				RaisePropertyChanged(nameof(ArtistIsNotFilled));
				RaisePropertyChanged(nameof(ArtistIsNew));
			}
		}

		public override bool ArtistIsEditable => true;

		public override bool ArtistIsNotFilled => Artist == null;

		public CompilationDiscWithoutArtistInfoViewItem(AddedDiscInfo discInfo, bool folderExists, IEnumerable<ArtistModel> availableArtists, IEnumerable<GenreModel> availableGenres)
			: base(discInfo, folderExists, availableArtists, availableGenres)
		{
		}

		protected override ArtistModel GetSongArtist(AddedSongInfo song)
		{
			return Artist;
		}
	}
}
