using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadAlbumTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagAlbum;

		public override string Description => $"Album tag is inconsistent for song '{SongDisplayTitle}': '{tagAlbum}' != '{Song.Disc.AlbumTitle}'";

		public BadAlbumTagInconsistency(SongModel song, string tagAlbum)
			: base(song)
		{
			this.tagAlbum = tagAlbum;
		}
	}
}
