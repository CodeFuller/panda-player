using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadArtistTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagArtist;

		public override string Description => $"Artist tag is inconsistent for song '{SongDisplayTitle}': '{tagArtist}' != '{Song.Artist?.Name}'";

		public BadArtistTagInconsistency(SongModel song, string tagArtist)
			: base(song)
		{
			this.tagArtist = tagArtist;
		}
	}
}
