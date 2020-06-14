using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.TagsInconsistencies
{
	internal class BadGenreTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagGenre;

		public override string Description => Current($"Genre tag is inconsistent for song '{SongDisplayTitle}': '{tagGenre}' != '{Song.Genre?.Name}'");

		public BadGenreTagInconsistency(SongModel song, string tagGenre)
			: base(song)
		{
			this.tagGenre = tagGenre;
		}
	}
}
