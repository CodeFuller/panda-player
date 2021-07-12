using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadTitleTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagTitle;

		public override string Description => $"Title tag is inconsistent for song '{SongDisplayTitle}': '{tagTitle}' != '{Song.Title}'";

		public BadTitleTagInconsistency(SongModel song, string tagTitle)
			: base(song)
		{
			this.tagTitle = tagTitle;
		}
	}
}
