using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class BadTrackTagInconsistency : BasicTagInconsistency
	{
		private readonly int? tagTrack;

		public override string Description => $"Track tag is inconsistent for song '{SongDisplayTitle}': '{tagTrack}' != '{Song.TrackNumber}'";

		public BadTrackTagInconsistency(SongModel song, int? tagTrack)
			: base(song)
		{
			this.tagTrack = tagTrack;
		}
	}
}
