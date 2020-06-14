using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.TagsInconsistencies
{
	internal class BadTitleTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagTitle;

		public override string Description => Current($"Title tag is inconsistent for song '{SongDisplayTitle}': '{tagTitle}' != '{Song.Title}'");

		public BadTitleTagInconsistency(SongModel song, string tagTitle)
			: base(song)
		{
			this.tagTitle = tagTitle;
		}
	}
}
