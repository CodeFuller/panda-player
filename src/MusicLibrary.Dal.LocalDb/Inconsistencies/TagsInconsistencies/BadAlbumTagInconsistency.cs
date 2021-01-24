using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadAlbumTagInconsistency : BasicTagInconsistency
	{
		private readonly string tagAlbum;

		public override string Description => Current($"Album tag is inconsistent for song '{SongDisplayTitle}': '{tagAlbum}' != '{Song.Disc.AlbumTitle}'");

		public BadAlbumTagInconsistency(SongModel song, string tagAlbum)
			: base(song)
		{
			this.tagAlbum = tagAlbum;
		}
	}
}
