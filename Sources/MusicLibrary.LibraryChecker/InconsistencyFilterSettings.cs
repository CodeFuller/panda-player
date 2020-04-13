using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicLibrary.LibraryChecker
{
	public class InconsistencyFilterSettings
	{
		public ICollection<string> SkipDifferentGenresForDiscs { get; } = new Collection<string>();

		public ICollection<AllowedArtistCorrection> AllowedLastFmArtistCorrections { get; } = new Collection<AllowedArtistCorrection>();

		public ICollection<AllowedSongCorrection> AllowedLastFmSongCorrections { get; } = new Collection<AllowedSongCorrection>();

		public ICollection<SongTitleCharacterCorrections> LastFmSongTitleCharacterCorrections { get; } = new Collection<SongTitleCharacterCorrections>();
	}
}
