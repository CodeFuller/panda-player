using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.TagsInconsistencies
{
	internal class BadYearTagInconsistency : BasicTagInconsistency
	{
		private readonly int? tagYear;

		public override string Description => Current($"Year tag is inconsistent for song '{SongDisplayTitle}': '{tagYear}' != '{Song.Disc.Year}'");

		public BadYearTagInconsistency(SongModel song, int? tagYear)
			: base(song)
		{
			this.tagYear = tagYear;
		}
	}
}
