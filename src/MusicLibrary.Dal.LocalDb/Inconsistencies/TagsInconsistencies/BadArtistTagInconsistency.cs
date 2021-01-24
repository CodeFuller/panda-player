using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadArtistTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagArtist;

		public override string Description => Current($"Artist tag is inconsistent for song '{SongDisplayTitle}': '{tagArtist}' != '{Song.Artist?.Name}'");

		public BadArtistTagInconsistency(SongModel song, string tagArtist)
			: base(song)
		{
			this.tagArtist = tagArtist;
		}
	}
}
