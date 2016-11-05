using System;

namespace CF.MusicLibrary.Dal
{
	/// <summary>
	/// Class for Playback entity.
	/// </summary>
	public partial class Playback
	{
		/// <summary>
		/// Default constructor required by Entity Framework.
		/// </summary>
		public Playback()
		{
		}

		/// <summary>
		/// Constructs Playback entity from BL.Objects.Playback.
		/// </summary>
		public static Playback CreatePlayback(BL.Objects.Playback playback)
		{
			if (playback == null)
			{
				throw new ArgumentNullException(nameof(playback));
			}

			return new Playback
			{
				PlaybackTime = playback.PlaybackTime,
			};
		}
	}
}
