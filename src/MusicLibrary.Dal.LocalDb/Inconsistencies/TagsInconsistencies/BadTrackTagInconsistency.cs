using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadTrackTagInconsistency : BasicTagInconsistency
	{
		private readonly int? tagTrack;

		public override string Description => Current($"Track tag is inconsistent for song '{SongDisplayTitle}': '{tagTrack}' != '{Song.TrackNumber}'");

		public BadTrackTagInconsistency(SongModel song, int? tagTrack)
			: base(song)
		{
			this.tagTrack = tagTrack;
		}
	}
}
