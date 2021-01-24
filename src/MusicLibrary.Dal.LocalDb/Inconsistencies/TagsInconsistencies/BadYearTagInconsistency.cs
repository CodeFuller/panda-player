using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadYearTagInconsistency : BasicTagInconsistency
	{
		private readonly int? tagYear;

		public override string Description => $"Year tag is inconsistent for song '{SongDisplayTitle}': '{tagYear}' != '{Song.Disc.Year}'";

		public BadYearTagInconsistency(SongModel song, int? tagYear)
			: base(song)
		{
			this.tagYear = tagYear;
		}
	}
}
