using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadGenreTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagGenre;

		public override string Description => $"Genre tag is inconsistent for song '{SongDisplayTitle}': '{tagGenre}' != '{Song.Genre?.Name}'";

		public BadGenreTagInconsistency(SongModel song, string tagGenre)
			: base(song)
		{
			this.tagGenre = tagGenre;
		}
	}
}
